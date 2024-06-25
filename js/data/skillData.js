const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

async function loadSkillsData() {
    try {
        const skillsCache = await getAsync('skills');
        if (skillsCache) {
            return JSON.parse(skillsCache);
        }

        const conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Skills');
        conn.release();

        await setAsync('skills', JSON.stringify(rows), 'EX', 3600);
        return rows;
    } catch (err) {
        console.error('Skills 데이터를 불러오는 중 오류 발생:', err);
        throw err;
    }
}

module.exports = loadSkillsData;