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
});

app.use(cors());
app.use(express.json());

const weaponList = [
    { id: 1, rarity: '일반', grade: '하급', base_attack_power: 2, base_critical_chance: 0, base_critical_damage: 0.1, base_max_health: 10 },
    { id: 2, rarity: '일반', grade: '중급', base_attack_power: 50, base_critical_chance: 0, base_critical_damage: 0.1, base_max_health: 250 },
    { id: 3, rarity: '일반', grade: '상급', base_attack_power: 100, base_critical_chance: 0, base_critical_damage: 0.1, base_max_health: 500 },
    { id: 4, rarity: '일반', grade: '최상급', base_attack_power: 150, base_critical_chance: 0, base_critical_damage: 0.1, base_max_health: 750 },
    { id: 5, rarity: '고급', grade: '하급', base_attack_power: 200, base_critical_chance: 0, base_critical_damage: 0.2, base_max_health: 1000 },
    { id: 6, rarity: '고급', grade: '중급', base_attack_power: 250, base_critical_chance: 0, base_critical_damage: 0.2, base_max_health: 1250 },
    { id: 7, rarity: '고급', grade: '상급', base_attack_power: 300, base_critical_chance: 0, base_critical_damage: 0.2, base_max_health: 1500 },
    { id: 8, rarity: '고급', grade: '최상급', base_attack_power: 350, base_critical_chance: 0, base_critical_damage: 0.2, base_max_health: 1750 },
    { id: 9, rarity: '매직', grade: '하급', base_attack_power: 400, base_critical_chance: 0, base_critical_damage: 0.4, base_max_health: 2000 },
    { id: 10, rarity: '매직', grade: '중급', base_attack_power: 450, base_critical_chance: 0, base_critical_damage: 0.4, base_max_health: 2250 },
    { id: 11, rarity: '매직', grade: '상급', base_attack_power: 500, base_critical_chance: 0, base_critical_damage: 0.4, base_max_health: 2500 },
    { id: 12, rarity: '매직', grade: '최상급', base_attack_power: 550, base_critical_chance: 0, base_critical_damage: 0.4, base_max_health: 2750 },
    { id: 13, rarity: '유물', grade: '하급', base_attack_power: 600, base_critical_chance: 0.2, base_critical_damage: 0.8, base_max_health: 3000 },
    { id: 14, rarity: '유물', grade: '중급', base_attack_power: 650, base_critical_chance: 0.2, base_critical_damage: 0.8, base_max_health: 3250 },
    { id: 15, rarity: '유물', grade: '상급', base_attack_power: 700, base_critical_chance: 0.2, base_critical_damage: 0.8, base_max_health: 3500 },
    { id: 16, rarity: '유물', grade: '최상급', base_attack_power: 750, base_critical_chance: 0.2, base_critical_damage: 0.8, base_max_health: 3750 },
    { id: 17, rarity: '영웅', grade: '하급', base_attack_power: 800, base_critical_chance: 0.4, base_critical_damage: 1.6, base_max_health: 4000 },
    { id: 18, rarity: '영웅', grade: '중급', base_attack_power: 850, base_critical_chance: 0.4, base_critical_damage: 1.6, base_max_health: 4250 },
    { id: 19, rarity: '영웅', grade: '상급', base_attack_power: 900, base_critical_chance: 0.4, base_critical_damage: 1.6, base_max_health: 4500 },
    { id: 20, rarity: '영웅', grade: '최상급', base_attack_power: 950, base_critical_chance: 0.4, base_critical_damage: 1.6, base_max_health: 4750 },
    { id: 21, rarity: '에픽', grade: '하급', base_attack_power: 1000, base_critical_chance: 0.6, base_critical_damage: 3.2, base_max_health: 5000 },
    { id: 22, rarity: '에픽', grade: '중급', base_attack_power: 1050, base_critical_chance: 0.6, base_critical_damage: 3.2, base_max_health: 5250 },
    { id: 23, rarity: '에픽', grade: '상급', base_attack_power: 1100, base_critical_chance: 0.6, base_critical_damage: 3.2, base_max_health: 5500 },
    { id: 24, rarity: '에픽', grade: '최상급', base_attack_power: 1150, base_critical_chance: 0.6, base_critical_damage: 3.2, base_max_health: 5750 },
    { id: 25, rarity: '고대', grade: '하급', base_attack_power: 1200, base_critical_chance: 0.8, base_critical_damage: 6.4, base_max_health: 6000 },
    { id: 26, rarity: '고대', grade: '중급', base_attack_power: 1250, base_critical_chance: 0.8, base_critical_damage: 6.4, base_max_health: 6250 },
    { id: 27, rarity: '고대', grade: '상급', base_attack_power: 1300, base_critical_chance: 0.8, base_critical_damage: 6.4, base_max_health: 6500 },
    { id: 28, rarity: '고대', grade: '최상급', base_attack_power: 1350, base_critical_chance: 0.8, base_critical_damage: 6.4, base_max_health: 6750 },
    { id: 29, rarity: '신화', grade: '하급', base_attack_power: 1400, base_critical_chance: 1.0, base_critical_damage: 12.8, base_max_health: 7000 },
    { id: 30, rarity: '신화', grade: '중급', base_attack_power: 1450, base_critical_chance: 1.0, base_critical_damage: 12.8, base_max_health: 7250 },
    { id: 31, rarity: '신화', grade: '상급', base_attack_power: 1500, base_critical_chance: 1.0, base_critical_damage: 12.8, base_max_health: 7500 },
    { id: 32, rarity: '신화', grade: '최상급', base_attack_power: 1550, base_critical_chance: 1.0, base_critical_damage: 12.8, base_max_health: 7750 },
];

app.post('/register', async (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ error: 'Invalid input' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?', [username, nickname]);
        if (existingUser.length > 0) {
            return res.status(400).json({ error: 'Username or nickname already exists' });
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        const result = await conn.query('INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)', [username, hashedPassword, nickname]);

        const playerId = result.insertId;

        // Add 10 random weapons
        for (let i = 0; i < 10; i++) {
            const randomWeapon = weaponList[Math.floor(Math.random() * weaponList.length)];
            await conn.query(
                'INSERT INTO PlayerWeapon (player_id, weapon_id, level, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, ?, ?, ?, ?, ?, ?)',
                [playerId, randomWeapon.id, 1, 1, randomWeapon.base_attack_power, randomWeapon.base_critical_chance, randomWeapon.base_critical_damage, randomWeapon.base_max_health]
            );
        }

        res.status(201).json({ message: 'User registered successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});

app.post('/login', async (req, res) => {
    console.log('Login endpoint hit'); // 로그 추가
    const { username, password } = req.body;
    if (!username || !password) {
        console.log('Invalid username or password:', req.body); // 로그 추가
        return res.status(400).json({ error: 'Invalid username or password' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const user = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        if (user.length === 0) {
            console.log('Invalid username or password'); // 로그 추가
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const validPassword = await bcrypt.compare(password, user[0].player_password);
        if (!validPassword) {
            console.log('Invalid username or password'); // 로그 추가
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const token = jwt.sign({ userId: user[0].player_id }, process.env.JWT_SECRET, { expiresIn: '1h' });
        res.json({ token });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
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
        const weapons = await conn.query('SELECT pw.level, pw.count, pw.attack_power, pw.critical_chance, pw.critical_damage, pw.max_health, wd.rarity, wd.grade FROM PlayerWeapon pw JOIN WeaponDB wd ON pw.weapon_id = wd.id WHERE pw.player_id = ?', [decoded.userId]);
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

    let conn;
    try {
        conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        if (existingUser.length > 0) {
            return res.status(200).json({ exists: true });
        }
        res.status(200).json({ exists: false });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});

// 추가: /check-nickname 경로 처리
app.get('/check-nickname', async (req, res) => {
    const { nickname } = req.query;
    if (!nickname) {
        return res.status(400).json({ error: 'Nickname is required' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const existingUser = await conn.query('SELECT * FROM Players WHERE player_nickname = ?', [nickname]);
        if (existingUser.length > 0) {
            return res.status(200).json({ exists: true });
        }
        res.status(200).json({ exists: false });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});
