const pool = require('../config/db');
const redis = require('../config/redis');

exports.getPlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    
    try {
        const conn = await pool.getConnection();
        
        const result = await conn.query(
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
        
        if (result.length > 0) {
            res.json(result[0]);
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

    // combat_power를 업데이트 대상에서 제외
    delete updatedData.combat_power;

    try {
        const conn = await pool.getConnection();
        
        // SQL 쿼리를 직접 구성합니다.
        const updateQuery = 'UPDATE PlayerAttributes SET ' +
            Object.keys(updatedData).map(key => `${key} = ?`).join(', ') +
            ' WHERE player_id = ?';
        
        const updateValues = [...Object.values(updatedData), playerId];

        await conn.query(updateQuery, updateValues);

        // 업데이트된 데이터를 다시 가져옵니다.
        const updatedPlayer = await conn.query(
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

        if (updatedPlayer.length > 0) {
            // 순위 재계산
            await updatePlayerRank(playerId);

            res.json(updatedPlayer[0]);
        } else {
            res.status(404).json({ message: "Player not found" });
        }

    } catch (error) {
        console.error('Error updating player data:', error);
        res.status(500).json({ message: 'Internal server error', error: error.message });
    }
};

async function updatePlayerRank(playerId) {
    const redisClient = redis.getClient();
    const conn = await pool.getConnection();

    try {
        const [playerData] = await conn.query(
            'SELECT combat_power FROM PlayerAttributes WHERE player_id = ?',
            [playerId]
        );

        if (playerData && playerData.length > 0) {
            const { combat_power } = playerData[0];
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

exports.resetAbilities = async (req, res) => {
    const playerId = parseInt(req.params.id);
    
    try {
        const conn = await pool.getConnection();
        
        // 새로운 능력치 생성
        const newAbilities = generateAbilities();
        
        // 데이터베이스 업데이트
        await conn.query(
            'UPDATE PlayerAttributes SET Ability1 = ?, Ability2 = ?, Ability3 = ? WHERE player_id = ?',
            [newAbilities[0], newAbilities[1], newAbilities[2], playerId]
        );
        
        // 업데이트된 플레이어 데이터 가져오기
        const [updatedPlayer] = await conn.query(
            'SELECT * FROM Players p JOIN PlayerAttributes pa ON p.player_id = pa.player_id WHERE p.player_id = ?',
            [playerId]
        );
        
        conn.release();
        
        if (updatedPlayer.length > 0) {
            // 플레이어 데이터에 새 능력치 적용
            applyAbilities(updatedPlayer[0]);
            
            res.json(updatedPlayer[0]);
        } else {
            res.status(404).json({ message: "Player not found" });
        }
    } catch (error) {
        console.error('Error resetting abilities:', error);
        res.status(500).json({ message: 'Internal server error', error: error.message });
    }
};

function generateAbilities() {
    const stats = ['attack_power', 'max_health', 'critical_chance', 'critical_damage'];
    const percentages = [3, 6, 9, 12];
    
    return Array(3).fill().map(() => {
        const stat = stats[Math.floor(Math.random() * stats.length)];
        const percentage = percentages[Math.floor(Math.random() * percentages.length)];
        return `${stat}:${percentage}`;
    });
}

function applyAbilities(player) {
    const abilities = [player.Ability1, player.Ability2, player.Ability3].filter(a => a);
    const bonuses = {
        attack_power: 0,
        max_health: 0,
        critical_chance: 0,
        critical_damage: 0
    };
    
    abilities.forEach(ability => {
        const [stat, percentage] = ability.split(':');
        bonuses[stat] += parseInt(percentage);
    });
    
    Object.keys(bonuses).forEach(stat => {
        player[stat] *= (1 + bonuses[stat] / 100);
    });
    
    // combat_power 재계산 (데이터베이스의 generated column과 일치하도록 해야 함)
    player.combat_power = Math.floor(
        (player.attack_power + player.critical_chance + player.max_health + player.critical_damage) * 
        (player.awakening === 0 ? 1 : player.awakening * 10) * 
        (player.level * 0.1)
    );
}

// 5분마다 순위 동기화
setInterval(syncRanksToDB, 5 * 60 * 1000);