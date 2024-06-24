// 미션 보상
const pool = require('../config/db');
const { rewardData } = require('../data/rewardData');

exports.getRewards = (req, res) => {
    try {
        res.json(rewardData);
    } catch (err) {
        console.error('보상 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.claimReward = async (req, res) => {
    const { rewardId } = req.body;

    const reward = rewardData.find(r => r.id === rewardId);
    if (!reward) {
        return res.status(400).json({ error: 'Invalid reward ID' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const query = 'INSERT INTO mails (user_id, type, reward, expires_at) VALUES (?, ?, ?, NOW() + INTERVAL 3 DAY)';
        await conn.query(query, [req.user.userId, 'mission', JSON.stringify({ star_dust: reward.reward })]);

        res.status(200).json({ message: 'Reward claimed successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};
