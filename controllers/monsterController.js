const pool = require('../config/db');

async function getMonstersFromDB() {
    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM Monsters');
        return Array.isArray(rows) ? rows : [rows];
    } catch (err) {
        console.error('Error fetching monsters from database:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

exports.getMonsters = async (req, res) => {
    try {
        const monsters = await getMonstersFromDB();
        if (monsters.length > 0) {
            res.json(monsters);
        } else {
            console.log('No monsters found in the database');
            res.status(404).json({ error: 'No monsters found' });
        }
    } catch (err) {
        console.error('Error retrieving monsters:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

module.exports = {
    getMonsters: exports.getMonsters,
    getMonstersFromDB
};