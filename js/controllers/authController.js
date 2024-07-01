const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const pool = require('../config/db');
const { getCachedWeapons } = require('../controllers/weaponController');

exports.register = async (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ error: 'Invalid input' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();  // 트랜잭션 시작

        let existingUserResult;
        console.log('Query result:', existingUserResult);
        try {
            [existingUserResult] = await conn.query('SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?', [username, nickname]);
        } catch (queryError) {
            console.error('Query error:', queryError);
            throw new Error('Failed to execute SELECT query');
        }

        // 쿼리 결과 로그
        console.log('Query result:', existingUserResult);

        if (!Array.isArray(existingUserResult)) {
            throw new Error('Unexpected query result format');
        }

        if (existingUserResult.length > 0) {
            return res.status(400).json({ error: 'Username or nickname already exists' });
        }

        let insertResult;
        try {
            [insertResult] = await conn.query('INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)', [username, await bcrypt.hash(password, 10), nickname]);
        } catch (queryError) {
            console.error('Insert error:', queryError);
            throw new Error('Failed to insert into Players');
        }

        console.log('Insert result:', insertResult);

        if (!insertResult || !insertResult.insertId) {
            throw new Error('Failed to insert into Players');
        }

        const playerId = insertResult.insertId;

        try {
            await conn.query('INSERT INTO PlayerAttributes (player_id) VALUES (?)', [playerId]);
            await conn.query('INSERT INTO MissionProgress (player_id) VALUES (?)', [playerId]);
        } catch (queryError) {
            console.error('Insert attribute or mission error:', queryError);
            throw new Error('Failed to insert into PlayerAttributes or MissionProgress');
        }

        const weapons = await getCachedWeapons();
        console.log('Fetched weapons:', weapons);

        if (!Array.isArray(weapons)) {
            throw new Error('Weapons data is not an array');
        }

        const weaponInserts = weapons.map(weapon => [
            playerId, weapon.weapon_id, 0, weapon.attack_power, weapon.crit_rate, weapon.crit_damage
        ]);

        console.log('Weapon inserts:', weaponInserts);

        if (weaponInserts.length > 0) {
            try {
                await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage) VALUES ?', [weaponInserts]);
            } catch (queryError) {
                console.error('Weapon insert error:', queryError);
                throw new Error('Failed to insert into PlayerWeaponInventory');
            }
        }

        await conn.commit();  // 모든 작업이 성공적으로 완료되면 커밋
        res.status(201).json({ message: 'User registered successfully' });
    } catch (err) {
        if (conn) await conn.rollback();  // 오류 발생 시 롤백
        console.error('Registration error:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};
exports.login = async (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ error: 'Invalid username or password' });
    }

    let conn;
    try {
        console.log('Attempting to connect to database...');
        conn = await pool.getConnection();
        console.log('Database connection established.');

        console.log(`Executing query for username: ${username}`);
        const result = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        console.log('Query executed. Result:', result);

        // 결과 구조 로깅
        console.log('Result structure:', JSON.stringify(result, null, 2));

        let user;
        if (Array.isArray(result)) {
            user = result[0];
        } else if (result && typeof result === 'object') {
            user = result;
        }

        console.log('Processed user data:', user);

        if (!user || !user.player_password) {
            console.log('Invalid user data');
            return res.status(400).json({ error: 'Invalid user data' });
        }

        console.log('Comparing passwords...');
        const validPassword = await bcrypt.compare(password, user.player_password);
        console.log('Password comparison result:', validPassword);

        if (!validPassword) {
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const token = jwt.sign({ userId: user.player_id }, process.env.JWT_SECRET, { expiresIn: '1h' });
        const playerData = {
            token: token,
            userId: user.player_id,
            current_stage: user.current_stage || '1-1'
        };

        console.log('Sending response:', playerData);
        res.json(playerData);
    } catch (err) {
        console.error('Login error:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};