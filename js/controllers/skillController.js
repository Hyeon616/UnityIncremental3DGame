// 스킬
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getSkills = async (req, res) => {
    try {
        let skills = await getAsync('skills');
        if (!skills) {
            let conn;
            try {
                conn = await pool.getConnection();
                const [rows] = await conn.query('SELECT * FROM Skills');
                skills = JSON.stringify(rows);
                await setAsync('skills', skills, 'EX', 3600); // 1시간 동안 캐싱
                console.log('Skills 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
            } finally {
                if (conn) conn.release();
            }
        } else {
            console.log('Skills 데이터가 성공적으로 Redis에서 로드되었습니다.');
        }
        res.json(JSON.parse(skills));
    } catch (err) {
        console.error('스킬 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};