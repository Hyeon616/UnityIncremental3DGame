const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const pool = require('../config/db');
const { getCachedWeapons } = require('../controllers/weaponController');

console.log('DB_HOST:', process.env.DB_HOST);
console.log('DB_USER:', process.env.DB_USER);
console.log('DB_NAME:', process.env.DB_DATABASE);

exports.register = async (req, res) => {
    console.log('Register function called');
    console.log('Request body:', req.body);

    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        console.log('Invalid input detected');
        return res.status(400).json({ error: 'Invalid input' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();

        // Check if username or nickname already exists
        const existingUsers = await conn.query(
            'SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?',
            [username, nickname]
        );
        console.log('Existing user check result:', existingUsers);

        if (Array.isArray(existingUsers) && existingUsers.length > 0) {
            return res.status(400).json({ error: 'Username or nickname already exists' });
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        console.log('Inserting new user with values:', { username, hashedPassword, nickname });
        console.log('이거 null아님 ',username);
        const insertResult = await conn.query(
            'INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)',
            [username, hashedPassword, nickname]
        );
        console.log('Insert result:', insertResult);;

        let playerId;
        if (Array.isArray(insertResult)) {
            playerId = insertResult[0].insertId;
        } else if (insertResult && insertResult.insertId) {
            playerId = insertResult.insertId;
        } else {
            throw new Error('Failed to get new player ID');
        }
        console.log('New player ID:', playerId);

        console.log('Inserting into PlayerAttributes');
        const attributeResult = await conn.query(
            'INSERT INTO PlayerAttributes (player_id) VALUES (?)',
            [playerId]
        );
        console.log('PlayerAttributes insert result:', attributeResult);

        console.log('Inserting into MissionProgress');
        const missionResult = await conn.query(
            'INSERT INTO MissionProgress (player_id) VALUES (?)',
            [playerId]
        );
        console.log('MissionProgress insert result:', missionResult);

        // Get weapons and insert into PlayerWeaponInventory
        const weapons = await getCachedWeapons();
        console.log(`Fetched ${weapons ? weapons.length : 0} weapons for new user`);

        if (Array.isArray(weapons) && weapons.length > 0) {
            const weaponInserts = weapons.map(weapon => [
                playerId,
                weapon.weapon_id,
                0, // count
                weapon.attack_power,
                parseFloat(weapon.crit_rate),
                parseFloat(weapon.crit_damage)
            ]);

            console.log(`Preparing to insert ${weaponInserts.length} weapons for new user`);

            const result = await conn.batch(
                'INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage) VALUES (?, ?, ?, ?, ?, ?)',
                weaponInserts
            );
            console.log(`Inserted ${result.affectedRows} weapons for new user`);
        } else {
            console.log('No weapons available for new user');
        }

        await conn.commit();
        res.status(201).json({ message: 'User registered successfully', playerId });
    } catch (err) {
        if (conn) await conn.rollback();
        console.error('Registration error:', err);
        if (err.code === 'ER_BAD_NULL_ERROR') {
            console.error('Null value detected. Request body:', req.body);
            console.error('SQL query:', err.sql);
        }
        res.status(500).json({ error: 'Registration failed: ' + err.message });
    } finally {
        if (conn) conn.release();
    }
};

exports.login = async (req, res) => {
    console.log('Login function called');
    console.log('Request body:', req.body);

    const { username, password } = req.body;
    if (!username || !password) {
        console.log('Invalid input detected');
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

        let user;
        if (Array.isArray(result) && result.length > 0) {
            user = result[0];
        } else if (result && typeof result === 'object') {
            user = result;
        }

        console.log('Processed user data:', user);

        if (!user || !user.player_password) {
            console.log('Invalid user data');
            return res.status(400).json({ error: 'Invalid username or password' });
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