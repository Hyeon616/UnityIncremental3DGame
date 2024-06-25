const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadBlessingsData() {
    try {
        const blessingsCache = await getAsync('blessings');
        if (blessingsCache) {
            return JSON.parse(blessingsCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Blessings');
        conn.release();

        await setAsync('blessings', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Blessings 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadBlessingsData;