const pool = require('../config/db');

exports.getRewards = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();
        const [rows] = await conn.query('SELECT * FROM Rewards');
        
        if (rows && rows.length > 0) {
            res.json(rows);
        } else {
            res.status(404).json({ error: 'No rewards found' });
        }
    } catch (err) {
        console.error('Error retrieving rewards:', err);
        res.status(500).json({ error: 'Server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.claimReward = async (req, res) => {
    const { rewardId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();

        const [reward] = await conn.query('SELECT * FROM Rewards WHERE id = ?', [rewardId]);
        if (!reward || reward.length === 0) {
            await conn.rollback();
            return res.status(404).json({ error: 'Reward not found' });
        }

        const [progress] = await conn.query('SELECT * FROM MissionProgress WHERE player_id = ?', [playerId]);
        if (!progress || progress.length === 0) {
            await conn.rollback();
            return res.status(404).json({ error: 'Mission progress not found' });
        }

        const progressField = `${reward[0].Type}_progress`;
        if (progress[0][progressField] < reward[0].Requirement) {
            await conn.rollback();
            return res.status(400).json({ error: 'Reward conditions not met' });
        }

        await conn.query('UPDATE Players SET currency = currency + ? WHERE player_id = ?', [reward[0].Reward, playerId]);
        await conn.query(`UPDATE MissionProgress SET ${progressField} = ${progressField} - ? WHERE player_id = ?`, [reward[0].Requirement, playerId]);

        await conn.commit();
        res.status(200).json({ message: 'Reward claimed successfully' });
    } catch (err) {
        if (conn) await conn.rollback();
        console.error('Error claiming reward:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};


module.exports = {
    getRewards: exports.getRewards,
    claimReward: exports.claimReward
};