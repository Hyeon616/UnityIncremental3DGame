const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadStageData() {
    try {
        const stagesCache = await getAsync('stages');
        if (stagesCache) {
            return JSON.parse(stagesCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Stages');
        conn.release();

        await setAsync('stages', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Stages 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadStageData;