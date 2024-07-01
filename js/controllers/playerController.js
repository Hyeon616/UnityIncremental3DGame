// controllers/playerController.js
const pool = require('../config/db');

exports.getPlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    console.log(`Attempting to fetch data for player ID: ${playerId}`);
    
    let conn;
    try {
        conn = await pool.getConnection();
        
        const [result] = await conn.query(
            'SELECT p.player_id, p.player_username, p.player_nickname, ' +
            'pa.element_stone, pa.skill_summon_tickets, pa.money, pa.attack_power, ' +
            'pa.max_health, pa.critical_chance, pa.critical_damage, pa.current_stage, ' +
            'pa.level, pa.awakening, pa.guild_id, pa.combat_power, pa.rank, ' +
            'pa.equipped_skill1_id, pa.equipped_skill2_id, pa.equipped_skill3_id ' +
            'FROM Players p ' +
            'LEFT JOIN PlayerAttributes pa ON p.player_id = pa.player_id ' +
            'WHERE p.player_id = ?', 
            [playerId]
        );
        
        let rows = Array.isArray(result) ? result : [result];
        
        if (rows && rows.length > 0) {
            res.json(rows[0]);
        } else {
            console.log(`No player found with ID: ${playerId}`);
            res.status(404).json({ message: "Player not found" });
        }
    } catch (error) {
        console.error('Error fetching player data:', error);
        res.status(500).json({ message: 'Internal server error', error: error.message });
    } finally {
        if (conn) conn.release();
    }
};



exports.updatePlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    const updatedData = req.body;

    let conn;
    try {
        conn = await pool.getConnection();
        
        await conn.query(
            'UPDATE PlayerAttributes SET ? WHERE player_id = ?',
            [updatedData, playerId]
        );

        res.json({ message: 'Player data updated successfully' });
    } catch (error) {
        console.error('Error updating player data:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.equipSkill = async (req, res) => {
    const playerId = parseInt(req.params.id);
    const { skillId, slotNumber } = req.body;

    if (slotNumber < 1 || slotNumber > 3) {
        return res.status(400).json({ message: 'Invalid slot number' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        
        const [playerSkill] = await conn.query(
            'SELECT * FROM PlayerSkills WHERE player_id = ? AND skill_id = ?', 
            [playerId, skillId]
        );
        
        if (playerSkill.length === 0) {
            return res.status(400).json({ message: 'Player does not have this skill' });
        }

        await conn.query(
            `UPDATE PlayerAttributes SET equipped_skill${slotNumber}_id = ? WHERE player_id = ?`,
            [skillId, playerId]
        );

        res.json({ message: 'Skill equipped successfully' });
    } catch (error) {
        console.error('Error equipping skill:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};