// 가호
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getBlessings = async (req, res) => {
    try {
        let blessings = await getAsync('blessings');
        if (!blessings) {
            let conn;
            try {
                conn = await pool.getConnection();
                const [rows] = await conn.query('SELECT * FROM Blessings');
                blessings = JSON.stringify(rows);
                await setAsync('blessings', blessings, 'EX', 3600); // 1시간 동안 캐싱
                console.log('Blessings 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
            } finally {
                if (conn) conn.release();
            }
        } else {
            console.log('Blessings 데이터가 성공적으로 Redis에서 로드되었습니다.');
        }
        res.json(JSON.parse(blessings));
    } catch (err) {
        console.error('축복 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};