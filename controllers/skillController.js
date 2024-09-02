const pool = require('../config/db');

async function getSkillsFromDB() {
    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Skills');
        return Array.isArray(rows) ? rows : [rows];
    } catch (err) {
        console.error('Error fetching skills from database:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

exports.getSkills = async (req, res) => {
    try {
        const skills = await getSkillsFromDB();
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
    getSkillsFromDB
};