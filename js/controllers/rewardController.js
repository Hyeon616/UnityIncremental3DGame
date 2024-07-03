const pool = require('../config/db');
const redis = require('../config/redis');

async function getRewardsCache() {
    const client = redis.getClient();
    const rewards = await client.get('rewards');
    return rewards ? JSON.parse(rewards) : null;
}

exports.getRewards = async (req, res) => {
    try {
        // Redis 캐시에서 rewards 가져오기
        const cachedRewards = await getRewardsCache();
        
        if (cachedRewards) {
            return res.json(cachedRewards);
        }
        
        // 캐시에 없으면 DB에서 가져오기
        let conn;
        try {
            conn = await pool.getConnection();
            const [rows] = await conn.query('SELECT * FROM Rewards');
            
            if (rows && rows.length > 0) {
                // Redis에 캐시 저장
                const client = redis.getClient();
                await client.set('rewards', JSON.stringify(rows), 'EX', 3600); // 1시간 동안 캐시
                
                res.json(rows);
            } else {
                res.status(404).json({ error: 'No rewards found' });
            }
        } finally {
            if (conn) conn.release();
        }
    } catch (err) {
        console.error('Error retrieving rewards:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.claimReward = async (req, res) => {
    const { rewardId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();

        const [reward] = await conn.query('SELECT * FROM Rewards WHERE id = ?', [rewardId]);
        if (!reward || reward.length === 0) {
            await conn.rollback();
            return res.status(404).json({ error: 'Reward not found' });
        }

        const [progress] = await conn.query('SELECT * FROM MissionProgress WHERE player_id = ?', [playerId]);
        if (!progress || progress.length === 0) {
            await conn.rollback();
            return res.status(404).json({ error: 'Mission progress not found' });
        }

        const progressField = `${reward[0].Type}_progress`;
        if (progress[0][progressField] < reward[0].Requirement) {
            await conn.rollback();
            return res.status(400).json({ error: 'Reward conditions not met' });
        }

        await conn.query('UPDATE Players SET currency = currency + ? WHERE player_id = ?', [reward[0].Reward, playerId]);
        await conn.query(`UPDATE MissionProgress SET ${progressField} = ${progressField} - ? WHERE player_id = ?`, [reward[0].Requirement, playerId]);

        await conn.commit();
        res.status(200).json({ message: 'Reward claimed successfully' });
    } catch (err) {
        if (conn) await conn.rollback();
        console.error('Error claiming reward:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.cacheRewards = async () => {
    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT * FROM Rewards');
        
        if (rows && rows.length > 0) {
            const client = redis.getClient();
            await client.set('rewards', JSON.stringify(rows), 'EX', 3600); // 1시간 동안 캐시
            console.log('Rewards cached successfully');
        }
    } catch (err) {
        console.error('Error caching rewards:', err);
    } finally {
        if (conn) conn.release();
    }
};