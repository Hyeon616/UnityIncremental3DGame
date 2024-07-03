const pool = require('../config/db');
const { getClient, connectRedis } = require('../config/redis');

// 무기 데이터를 Redis에 캐싱하는 함수
async function cacheWeapons() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const result = await conn.query('SELECT * FROM Weapon');
        
        let rows = Array.isArray(result) ? result : [result];
       
        await client.set('weapons', JSON.stringify(rows), {
            EX: 3600
        });
        console.log(`${rows.length} weapons successfully cached in Redis.`);
        return rows;
    } catch (err) {
        console.error('Error in cacheWeapons:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}


// Redis에서 무기 데이터를 가져오는 함수
async function getCachedWeapons() {
    let client = getClient();
    if (!client || !client.isOpen) {
        client = await connectRedis();
    }

    try {
        let weapons = await client.get('weapons');
        if (weapons) {
            console.log('Weapons data successfully loaded from Redis.');
            return JSON.parse(weapons);
        } else {
            console.log('No weapons found in Redis, fetching from DB');
            return await cacheWeapons();
        }
    } catch (err) {
        console.error('Error in getCachedWeapons:', err);
        // 에러가 발생해도 DB에서 직접 가져오기 시도
        return await cacheWeapons();
    }
}

async function getPlayerWeapons(req, res) {
    try {
        const weapons = await getCachedWeapons();
        const playerWeapons = weapons.filter(weapon => weapon.player_id === req.user.userId);
        res.json(playerWeapons);
    } catch (err) {
        console.error('Error getting player weapons:', err);
        res.status(500).json({ error: 'Internal server error' });
    }
}

// 무기 뽑기 함수
async function drawWeapon(req, res) {
    const { weaponId } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();

        const [weapon] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ? AND weapon_id = ?', [req.user.userId, weaponId]);
        if (!weapon || weapon.length === 0) {
            const weapons = await getCachedWeapons();
            const weaponInfo = weapons.find(w => w.weapon_id === weaponId);
            if (!weaponInfo) {
                return res.status(400).json({ error: 'Invalid weapon ID' });
            }
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage) VALUES (?, ?, 1, ?, ?, ?)',
                [req.user.userId, weaponId, weaponInfo.attack_power, weaponInfo.crit_rate, weaponInfo.crit_damage]);
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
}

// 무기 합성 함수
async function synthesizeWeapon(req, res) {
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

        await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

        res.status(200).json({ message: 'Weapon synthesized successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
}

// 모든 무기 합성 함수
async function synthesizeAllWeapons(req, res) {
    let conn;
    try {
        conn = await pool.getConnection();

        const [weapons] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ?', [req.user.userId]);

        for (const weapon of weapons) {
            while (weapon.count >= 5) {
                await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);

                const newWeaponId = getNextWeaponId(weapon.weapon_id);

                await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

                const [updatedWeaponRows] = await conn.query('SELECT count FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);
                weapon.count = updatedWeaponRows[0].count;
            }
        }

        res.status(200).json({ message: 'All weapons synthesized successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
}

function getNextWeaponId(currentWeaponId) {
    const nextWeaponMap = {
        1: 2, 2: 3, 3: 4, 4: 5,
        5: 6, 6: 7, 7: 8, 8: 9,
        9: 10, 10: 11, 11: 12, 12: 13,
        13: 14, 14: 15, 15: 16, 16: 17,
        17: 18, 18: 19, 19: 20, 20: 21,
        21: 22, 22: 23, 23: 24
    };
    return nextWeaponMap[currentWeaponId] || currentWeaponId;
}

module.exports = {
    drawWeapon,
    synthesizeWeapon,
    synthesizeAllWeapons,
    cacheWeapons,
    getCachedWeapons,
    getPlayerWeapons
};
