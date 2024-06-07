const express = require('express');
const mariadb = require('mariadb');
const dotenv = require('dotenv');
const cors = require('cors');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt');

dotenv.config();

const app = express();
const port = process.env.PORT || 3000;

const pool = mariadb.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_DATABASE,
    port: process.env.DB_PORT,
    connectionLimit: 5
});

app.use(cors());
app.use(express.json());

app.post('/register', async (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ error: 'Invalid input' });
    }

    try {
        const hashedPassword = await bcrypt.hash(password, 10);  // 비밀번호 해싱
        let conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?', [username, nickname]);
        if (existingUser.length > 0) {
            return res.status(400).json({ error: 'Username or nickname already exists' });
        }

        await conn.query('INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)', [username, hashedPassword, nickname]);
        res.status(201).json({ message: 'User registered successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    }
});

app.post('/login', async (req, res) => {
    const { username, password } = req.body;
    console.log(`Login attempt with username: ${username}`);
    if (!username || !password) {
        console.log('Invalid username or password');
        return res.status(400).json({ error: 'Invalid username or password' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const user = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        if (user.length === 0 || !await bcrypt.compare(password, user[0].player_password)) {
            console.log('Invalid username or password');
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const token = jwt.sign({ userId: user[0].player_id }, process.env.JWT_SECRET, { expiresIn: '1h' });
        res.json({ token });
        console.log('Login successful');
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    }
});

app.get('/weapons', async (req, res) => {
    const authHeader = req.headers.authorization;
    const token = authHeader && authHeader.split(' ')[1];
    if (!token) {
        return res.status(401).json({ error: 'No token provided' });
    }

    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        let conn = await pool.getConnection();
        const weapons = await conn.query('SELECT * FROM PlayerWeapon WHERE player_id = ?', [decoded.userId]);
        res.json(weapons);
    } catch (err) {
        console.error(err);
        res.status(403).json({ error: 'Invalid token' });
    }
});

app.get('/check-username', async (req, res) => {
    const { username } = req.query;
    if (!username) {
        return res.status(400).json({ error: 'Username is required' });
    }

    try {
        let conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        if (existingUser.length > 0) {
            return res.status(200).json({ exists: true });
        }
        res.status(200).json({ exists: false });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    }
});

app.get('/check-nickname', async (req, res) => {
    const { nickname } = req.query;
    if (!nickname) {
        return res.status(400).json({ error: 'Nickname is required' });
    }

    try {
        let conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_nickname = ?', [nickname]);
        if (existingUser.length > 0) {
            return res.status(200).json({ exists: true });
        }
        res.status(200).json({ exists: false });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    }
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});
