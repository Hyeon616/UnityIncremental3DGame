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
                rewards = JSON.stringify(rows);
                await setAsync('rewards', rewards, 'EX', 3600); // 1시간 동안 캐싱
                console.log('Rewards 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
            } finally {
                if (conn) conn.release();
            }
        } else {
            console.log('Rewards 데이터가 성공적으로 Redis에서 로드되었습니다.');
        }
        res.json(JSON.parse(rewards));
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
        const query = 'INSERT INTO mails (user_id, type, reward, expires_at) VALUES (?, ?, ?, NOW() + INTERVAL 3 DAY)';
        await conn.query(query, [req.user.userId, 'mission', JSON.stringify({ star_dust: reward.reward })]);
        console.log(`User ID ${req.user.userId}의 리워드 ${rewardId}가 성공적으로 클레임되었습니다.`);
        res.status(200).json({ message: 'Reward claimed successfully' });
    } catch (err) {
        console.error('리워드 클레임 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};