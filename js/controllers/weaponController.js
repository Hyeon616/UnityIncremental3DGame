// 무기 뽑기, 합성
const pool = require('../config/db');
const { weaponData } = require('../data/weaponData');

exports.drawWeapon = async (req, res) => {
    const { weaponId } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();

        const [weapon] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ? AND weapon_id = ?', [req.user.userId, weaponId]);
        if (!weapon) {
            const weaponInfo = weaponData.find(w => w.id === weaponId);
            if (!weaponInfo) {
                return res.status(400).json({ error: 'Invalid weapon ID' });
            }
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)',
                [req.user.userId, weaponId, weaponInfo.base_attack_power, weaponInfo.base_critical_chance, weaponInfo.base_critical_damage, weaponInfo.base_max_health]);
        } else {
            const updateResult = await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE player_id = ? AND weapon_id = ?', [req.user.userId, weaponId]);
            if (updateResult.affectedRows === 0) {
                return res.status(400).json({ error: 'Failed to update weapon count' });
            }
        }

        res.status(200).json({ message: 'Weapon drawn successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.synthesizeWeapon = async (req, res) => {
    const { weaponId } = req.body;

    if (!weaponId) {
        return res.status(400).json({ error: 'Weapon ID is required' });
    }

    let conn;
    try {
        conn = await pool.getConnection();

        const weapon = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [weaponId, req.user.userId]);
        if (weapon.length === 0 || weapon[0].count < 5) {
            return res.status(400).json({ error: 'Not enough weapons to synthesize' });
        }

        await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weaponId, req.user.userId]);

        const newWeaponId = getNextWeaponId(weaponId);

        await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

        res.status(200).json({ message: 'Weapon synthesized successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.synthesizeAllWeapons = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();

        const weapons = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ?', [req.user.userId]);

        for (const weapon of weapons) {
            while (weapon.count >= 5) {
                await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);

                const newWeaponId = getNextWeaponId(weapon.weapon_id);

                await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

                weapon.count = (await conn.query('SELECT count FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]))[0].count;
            }
        }

        res.status(200).json({ message: 'All weapons synthesized successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

function getNextWeaponId(currentWeaponId) {
    const nextWeaponMap = {
        1: 2, 2: 3, 3: 4, 4: 5,
        5: 6, 6: 7, 7: 8, 8: 9,
        9: 10, 10: 11, 11: 12, 12: 13,
        13: 14, 14: 15, 15: 16, 16: 17,
        17: 18, 18: 19, 19: 20, 20: 21,
        21: 22, 22: 23, 23: 24, 24: 25,
        25: 26, 26: 27, 27: 28, 28: 29,
        29: 30, 30: 31, 31: 32
    };
    return nextWeaponMap[currentWeaponId] || currentWeaponId;
}
