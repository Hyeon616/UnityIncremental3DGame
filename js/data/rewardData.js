const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadRewardsData() {
    try {
        const rewardsCache = await getAsync('rewards');
        if (rewardsCache) {
            return JSON.parse(rewardsCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Rewards');
        conn.release();

        await setAsync('rewards', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Rewards 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadRewardsData;