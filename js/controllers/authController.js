const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const pool = require('../config/db');

exports.register = async (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ error: 'Invalid input' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const existingUserResult = await conn.query('SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?', [username, nickname]);
        const existingUser = existingUserResult[0];

        if (existingUser && existingUser.length > 0) {
            return res.status(400).json({ error: 'Username or nickname already exists' });
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        const result = await conn.query('INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)', [username, hashedPassword, nickname]);

        const playerId = result.insertId;

        await conn.query('INSERT INTO PlayerAttributes (player_id) VALUES (?)', [playerId]);
        await conn.query('INSERT INTO MissionProgress (player_id) VALUES (?)', [playerId]);

        res.status(201).json({ message: 'User registered successfully' });
    } catch (err) {
        console.error(err);
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