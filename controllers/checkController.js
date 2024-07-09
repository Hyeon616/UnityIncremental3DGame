const pool = require('../config/db');

exports.checkUsername = async (req, res) => {
    const { username } = req.query;

    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT COUNT(*) as count FROM Players WHERE player_username = ?', [username]);

        if (!Array.isArray(rows) || rows.length === 0) {
            console.error('Unexpected database response:', rows);
            return res.status(500).json({ error: 'Unexpected database response' });
        }

        const count = Number(rows[0].count);

        if (count > 0) {
            res.json({ exists: true });
        } else {
            res.json({ exists: false });
        }
    } catch (err) {
        console.error('Error checking username:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.checkNickname = async (req, res) => {
    const { nickname } = req.query;

    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT COUNT(*) as count FROM Players WHERE player_nickname = ?', [nickname]);

        if (!Array.isArray(rows) || rows.length === 0) {
            console.error('Unexpected database response:', rows);
            return res.status(500).json({ error: 'Unexpected database response' });
        }

        const count = Number(rows[0].count);

        if (count > 0) {
            res.json({ exists: true });
        } else {
            res.json({ exists: false });
        }
    } catch (err) {
        console.error('Error checking nickname:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};
