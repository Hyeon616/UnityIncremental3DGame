const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadWeaponData() {
    try {
        const weaponsCache = await getAsync('weapons');
        if (weaponsCache) {
            return JSON.parse(weaponsCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Weapons');
        conn.release();

        await setAsync('weapons', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Weapons 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadWeaponData;