const pool = require('../config/db');

async function getStagesFromDB() {
    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT DISTINCT Stage FROM Monsters ORDER BY Stage');
        return Array.isArray(rows) ? rows : [rows];
    } catch (err) {
        console.error('Error fetching stages from database:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

exports.getStages = async (req, res) => {
    try {
        const stages = await getStagesFromDB();
        res.json(stages);
    } catch (err) {
        console.error('Error retrieving stages:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.updateStage = async (req, res) => {
    const { userId, stage } = req.body;

    if (!userId || !stage) {
        return res.status(400).json({ error: 'User ID and stage are required' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const result = await conn.query('UPDATE PlayerAttributes SET current_stage = ? WHERE player_id = ?', [stage, userId]);
        
        if (result.affectedRows > 0) {
            res.status(200).json({ message: 'Stage updated successfully' });
        } else {
            res.status(404).json({ error: 'User not found' });
        }
    } catch (err) {
        console.error('Error updating stage:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.getCurrentStage = async (req, res) => {
    const userId = req.user.userId;
    let conn;
    try {
        conn = await pool.getConnection();
        console.log(`Attempting to fetch current stage for user ID: ${userId}`);
        const rows = await conn.query('SELECT current_stage FROM PlayerAttributes WHERE player_id = ?', [userId]);
        
        console.log('Query result:', JSON.stringify(rows));
        
        if (rows && rows.length > 0) {
            console.log(`Current stage found: ${rows[0].current_stage}`);
            res.json({ current_stage: rows[0].current_stage });
        } else {
            console.log('No current stage found for this user');
            res.status(404).json({ error: 'Player attributes not found' });
        }
    } catch (err) {
        console.error('Error retrieving current stage:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

module.exports = {
    getStages: exports.getStages,
    updateStage: exports.updateStage,
    getCurrentStage: exports.getCurrentStage,
    getStagesFromDB
};