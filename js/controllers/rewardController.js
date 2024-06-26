// 미션 보상
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getRewards = async (req, res) => {
    try {
        let rewards = await getAsync('rewards');
        if (!rewards) {
            let conn;
            try {
                conn = await pool.getConnection();
                const [rows] = await conn.query('SELECT * FROM Rewards');
                
                if (rows && rows.length > 0) {
                    rewards = rows;
                    await setAsync('rewards', JSON.stringify(rows), 'EX', 3600); // 1시간 동안 캐싱
                } else {
                    return res.status(404).json({ error: 'No rewards found' });
                }
            } finally {
                if (conn) conn.release();
            }
        } else {
            rewards = JSON.parse(rewards);
        }
        res.json(rewards);
    } catch (err) {
        console.error('리워드 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.claimReward = async (req, res) => {
    const { rewardId } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT * FROM Rewards WHERE id = ?', [rewardId]);
        
        if (rows && rows.length > 0) {
            await conn.query('DELETE FROM Rewards WHERE id = ?', [rewardId]);
            res.status(200).json({ message: 'Reward claimed successfully' });
        } else {
            res.status(404).json({ error: 'Reward not found' });
        }
    } catch (err) {
        console.error('Error claiming reward:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};