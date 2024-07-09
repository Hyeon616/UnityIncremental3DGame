const pool = require('../config/db');
const redis = require('../config/redis');

exports.getPlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    
    try {
        const redisClient = redis.getClient();
        const cachedData = await redisClient.get(`player:${playerId}`);
        
        if (cachedData) {
            return res.json(JSON.parse(cachedData));
        }

        const conn = await pool.getConnection();
        
        const [result] = await conn.query(
            'SELECT p.player_id, p.player_username, p.player_nickname, ' +
            'pa.element_stone, pa.skill_summon_tickets, pa.money, pa.attack_power, ' +
            'pa.max_health, pa.critical_chance, pa.critical_damage, pa.current_stage, ' +
            'pa.level, pa.awakening, pa.guild_id, pa.combat_power, pa.rank, ' +
            'pa.equipped_skill1_id, pa.equipped_skill2_id, pa.equipped_skill3_id ' +
            'FROM Players p ' +
            'LEFT JOIN PlayerAttributes pa ON p.player_id = pa.player_id ' +
            'WHERE p.player_id = ?', 
            [playerId]
        );
        
        conn.release();
        
        if (result) {
            await redisClient.set(`player:${playerId}`, JSON.stringify(result), 'EX', 300); // 5분 캐시
            res.json(result);
        } else {
            res.status(404).json({ message: "Player not found" });
        }
    } catch (error) {
        console.error('Error fetching player data:', error);
        res.status(500).json({ message: 'Internal server error', error: error.message });
    }
};

exports.updatePlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    const updatedData = req.body;

    try {
        const conn = await pool.getConnection();
        
        await conn.query(
            'UPDATE PlayerAttributes SET ? WHERE player_id = ?',
            [updatedData, playerId]
        );

        conn.release();

        // Redis 캐시 업데이트
        const redisClient = redis.getClient();
        await redisClient.del(`player:${playerId}`);

        // 순위 재계산
        await updatePlayerRank(playerId);

        res.json({ message: 'Player data updated successfully' });
    } catch (error) {
        console.error('Error updating player data:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
};

async function updatePlayerRank(playerId) {
    const redisClient = getClient();
    const conn = await pool.getConnection();

    try {
        const [playerData] = await conn.query(
            'SELECT combat_power FROM PlayerAttributes WHERE player_id = ?',
            [playerId]
        );

        if (playerData) {
            const { combat_power } = playerData;
            await redisClient.zAdd('player_ranks', { score: combat_power, value: playerId.toString() });
            
            const rank = await redisClient.zRevRank('player_ranks', playerId.toString());
            await conn.query('UPDATE PlayerAttributes SET rank = ? WHERE player_id = ?', [rank + 1, playerId]);
        }
    } catch (error) {
        console.error('Error updating player rank:', error);
    } finally {
        conn.release();
    }
}

exports.equipSkill = async (req, res) => {
    const playerId = parseInt(req.params.id);
    const { skillId, slotNumber } = req.body;

    if (slotNumber < 1 || slotNumber > 3) {
        return res.status(400).json({ message: 'Invalid slot number' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        
        const [playerSkill] = await conn.query(
            'SELECT * FROM PlayerSkills WHERE player_id = ? AND skill_id = ?', 
            [playerId, skillId]
        );
        
        if (playerSkill.length === 0) {
            return res.status(400).json({ message: 'Player does not have this skill' });
        }

        await conn.query(
            `UPDATE PlayerAttributes SET equipped_skill${slotNumber}_id = ? WHERE player_id = ?`,
            [skillId, playerId]
        );

        res.json({ message: 'Skill equipped successfully' });
    } catch (error) {
        console.error('Error equipping skill:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};

// 주기적으로 Redis의 순위를 DB에 동기화하는 함수
async function syncRanksToDB() {
    const redisClient = redis.getClient();
    const conn = await pool.getConnection();

    try {
        // 'REV' 옵션을 사용하여 역순으로 정렬
        const players = await redisClient.zRangeWithScores('player_ranks', 0, -1, { REV: true });
        
        for (let i = 0; i < players.length; i++) {
            const { score, value: playerId } = players[i];
            await conn.query('UPDATE PlayerAttributes SET rank = ?, combat_power = ? WHERE player_id = ?', 
                             [i + 1, score, playerId]);
        }
    } finally {
        conn.release();
    }
}

// 5분마다 순위 동기화
setInterval(syncRanksToDB, 5 * 60 * 1000);