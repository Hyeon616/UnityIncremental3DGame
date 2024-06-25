const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadMonsterData() {
    try {
        const monstersCache = await getAsync('monsters');
        if (monstersCache) {
            return JSON.parse(monstersCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Monsters');
        conn.release();

        await setAsync('monsters', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Monsters 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadMonsterData;