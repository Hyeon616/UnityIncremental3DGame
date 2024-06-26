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
        conn = await pool.getConnection();
        const [userResult] = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);

        // 쿼리 결과를 로그에 출력
        console.log('userResult:', userResult);

        if (!userResult || userResult.length === 0) {
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const user = userResult;

        const validPassword = await bcrypt.compare(password, user.player_password);
        if (!validPassword) {
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const token = jwt.sign({ userId: user.player_id }, process.env.JWT_SECRET, { expiresIn: '1h' });

        const playerData = {
            token: token,
            current_stage: user.current_stage
        };

        res.json(playerData);
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};