// 몬스터 정보
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getMonsters = async (req, res) => {
    try {
        let monsters = await getAsync('monsters');
        if (!monsters) {
            let conn;
            try {
                conn = await pool.getConnection();
                const [rows] = await conn.query('SELECT * FROM Monsters');
                monsters = JSON.stringify(rows);
                await setAsync('monsters', monsters, 'EX', 3600); // 1시간 동안 캐싱
                console.log('Monsters 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
            } finally {
                if (conn) conn.release();
            }
        } else {
            console.log('Monsters 데이터가 성공적으로 Redis에서 로드되었습니다.');
        }
        res.json(JSON.parse(monsters));
    } catch (err) {
        console.error('몬스터 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};