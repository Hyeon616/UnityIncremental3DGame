const pool = require('../config/db');
const { getClient, connectRedis } = require('../config/redis');

async function cacheMonsters() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Monsters');
        
        if (rows) {
            const monstersArray = Array.isArray(rows) ? rows : [rows];
            await client.set('monsters', JSON.stringify(monstersArray), {
                EX: 3600
            });
            console.log(`${monstersArray.length} monsters successfully cached in Redis.`);
            return monstersArray;
        } else {
            console.log('No monsters found in the database');
            return [];
        }
    } catch (err) {
        console.error('Error in cacheMonsters:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

async function getCachedMonsters() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    try {
        let monsters = await client.get('monsters');
        if (monsters) {
            console.log('Monsters data successfully loaded from Redis.');
            return JSON.parse(monsters);
        } else {
            console.log('No monsters found in Redis, fetching from DB');
            return await cacheMonsters();
        }
    } catch (err) {
        console.error('Error in getCachedMonsters:', err);
        return await cacheMonsters();
    }
}

exports.getMonsters = async (req, res) => {
    try {
        const monsters = await getCachedMonsters();
        res.json(monsters);
    } catch (err) {
        console.error('Error retrieving monsters:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

module.exports = {
    getMonsters: exports.getMonsters,
    cacheMonsters,
    getCachedMonsters
};