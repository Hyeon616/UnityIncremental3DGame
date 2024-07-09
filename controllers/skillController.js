const pool = require('../config/db');
const { getClient, connectRedis } = require('../config/redis');

async function cacheSkills() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Skills');
        

        if (rows) {
            const skillsArray = Array.isArray(rows) ? rows : [rows];
            await client.set('skills', JSON.stringify(skillsArray), {
                EX: 3600
            });
            console.log(`${skillsArray.length} skills successfully cached in Redis.`);
            return skillsArray;
        } else {
            console.log('No skills found in the database');
            return [];
        }
    } catch (err) {
        console.error('Error in cacheSkills:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

async function getCachedSkills() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    try {
        let skills = await client.get('skills');
        if (skills) {
            console.log('Skills data successfully loaded from Redis.');
            return JSON.parse(skills);
        } else {
            console.log('No skills found in Redis, fetching from DB');
            return await cacheSkills();
        }
    } catch (err) {
        console.error('Error in getCachedSkills:', err);
        return await cacheSkills();
    }
}

exports.getSkills = async (req, res) => {
    try {
        const skills = await getCachedSkills();
        res.json(skills);
    } catch (err) {
        console.error('Error retrieving skills:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.getPlayerSkills = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT ps.*, s.* FROM PlayerSkills ps JOIN Skills s ON ps.skill_id = s.id WHERE ps.player_id = ?', [req.user.userId]);
        
        if (rows && rows.length > 0) {
            res.json(rows);
        } else {
            res.status(404).json({ error: 'No player skills found' });
        }
    } catch (err) {
        console.error('Error retrieving player skills:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

module.exports = {
    getSkills: exports.getSkills,
    getPlayerSkills: exports.getPlayerSkills,
    cacheSkills,
    getCachedSkills
};