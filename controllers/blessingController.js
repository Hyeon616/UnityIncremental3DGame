// 가호
const pool = require('../config/db');

exports.getBlessings = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT * FROM Blessings');
        
        if (rows && rows.length > 0) {
            res.json(rows);
        } else {
            res.status(404).json({ error: 'No blessings found' });
        }
    } catch (err) {
        console.error('Error retrieving blessings:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};