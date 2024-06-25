// 무기 뽑기, 합성
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

// 무기 데이터를 Redis에 캐싱하는 함수
async function cacheWeapons() {
    const conn = await pool.getConnection();
    try {
        const [weapons] = await conn.query('SELECT * FROM Weapons');
        await setAsync('weapons', JSON.stringify(weapons), 'EX', 3600);
        console.log('Weapons 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
        console.log('캐싱된 Weapons 데이터:', weapons); 
		return weapons;
    } finally {
        if (conn) conn.release();
    }
}

// Redis에서 무기 데이터를 가져오는 함수
async function getCachedWeapons() {
    let weapons = await getAsync('weapons');
    if (weapons) {
        console.log('Weapons 데이터가 성공적으로 Redis에서 로드되었습니다.');
		console.log('Redis에서 로드된 Weapons 데이터:', JSON.parse(weapons));
        return JSON.parse(weapons);
    } else {
        return await cacheWeapons();
    }
}

exports.drawWeapon = async (req, res) => {
    const { weaponId } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();

        const [existingWeapons] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ? AND weapon_id = ?', [req.user.userId, weaponId]);
        if (existingWeapons.length === 0) {
            const weapons = await getCachedWeapons();
            const weaponInfo = weapons.find(w => w.weapon_id === weaponId);
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
        console.error('무기 뽑기 중 오류 발생:', err);
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

        const [rows] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [weaponId, req.user.userId]);
        const weapon = rows[0];
        if (!weapon || weapon.count < 5) {
            return res.status(400).json({ error: 'Not enough weapons to synthesize' });
        }

        await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weaponId, req.user.userId]);

        const newWeaponId = getNextWeaponId(weaponId);

        await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)', 
            [req.user.userId, newWeaponId, weapon.attack_power, weapon.critical_chance, weapon.critical_damage, weapon.max_health]);

        console.log(`User ID ${req.user.userId}의 무기 ${weaponId}가 성공적으로 합성되었습니다.`);
        res.status(200).json({ message: 'Weapon synthesized successfully' });
    } catch (err) {
        console.error('무기 합성 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.synthesizeAllWeapons = async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();

        const [weapons] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ?', [req.user.userId]);

        for (const weapon of weapons) {
            while (weapon.count >= 5) {
                await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);

                const newWeaponId = getNextWeaponId(weapon.weapon_id);

                await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)', 
                    [req.user.userId, newWeaponId, weapon.attack_power, weapon.critical_chance, weapon.critical_damage, weapon.max_health]);

                const [updatedWeaponRows] = await conn.query('SELECT count FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);
                weapon.count = updatedWeaponRows[0].count;
                console.log(`User ID ${req.user.userId}의 무기 ${weapon.weapon_id}가 성공적으로 합성되었습니다.`);
            }
        }

        res.status(200).json({ message: 'All weapons synthesized successfully' });
    } catch (err) {
        console.error('무기 전체 합성 중 오류 발생:', err);
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
