// controllers/playerController.js
const pool = require('../config/db');

exports.getPlayerData = async (req, res) => {
    const playerId = parseInt(req.params.id);
    console.log(`Received request for player ID: ${playerId}`);

    let conn;
    try {
        conn = await pool.getConnection();
        
        const [rows] = await conn.query(
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

        console.log('Query result:', rows); // 디버깅을 위해 추가

        if (rows && rows.length > 0) {
            const playerData = rows[0];
            
            // 장착된 스킬 정보 가져오기
            const [equippedSkills] = await conn.query(
                'SELECT s.id, s.name, s.description, s.image ' +
                'FROM Skills s ' +
                'WHERE s.id IN (?, ?, ?)',
                [playerData.equipped_skill1_id, playerData.equipped_skill2_id, playerData.equipped_skill3_id]
            );

            playerData.equippedSkills = equippedSkills;

            res.json(playerData);
        } else {
            res.status(404).json({ message: 'Player not found' });
        }
    } catch (error) {
        console.error('Error fetching player data:', error);
        res.status(500).json({ message: 'Internal server error' });
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