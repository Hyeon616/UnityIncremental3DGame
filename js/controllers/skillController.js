// 스킬
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getSkills = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT * FROM Skills');
        
        if (rows && rows.length > 0) {
            res.json(rows);
        } else {
            res.status(404).json({ error: 'No skills found' });
        }
    } catch (err) {
        console.error('Error retrieving skills:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};