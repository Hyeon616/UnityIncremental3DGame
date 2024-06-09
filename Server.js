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

        const allWeapons = await conn.query('SELECT id, base_attack_power, base_critical_chance, base_critical_damage, base_max_health FROM WeaponDB');
        for (const weapon of allWeapons) {
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)',
                [playerId, weapon.id, weapon.base_attack_power, weapon.base_critical_chance, weapon.base_critical_damage, weapon.base_max_health]);
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
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ error: 'Invalid username or password' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const user = await conn.query('SELECT * FROM Players WHERE player_username = ?', [username]);
        if (user.length === 0) {
            return res.status(400).json({ error: 'Invalid username or password' });
        }

        const validPassword = await bcrypt.compare(password, user[0].player_password);
        if (!validPassword) {
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

    let conn;
    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        console.log('디코딩된 토큰:', decoded); // 토큰 디코딩 결과 확인
        conn = await pool.getConnection();
        const weapons = await conn.query(`
            SELECT pw.weapon_id as id, 
                   IFNULL(pw.level, 1) as level, 
                   IFNULL(pw.count, 0) as count, 
                   IFNULL(pw.attack_power, wd.base_attack_power) as attack_power, 
                   IFNULL(pw.critical_chance, wd.base_critical_chance) as critical_chance, 
                   IFNULL(pw.critical_damage, wd.base_critical_damage) as critical_damage, 
                   IFNULL(pw.max_health, wd.base_max_health) as max_health, 
                   wd.rarity, 
                   wd.grade 
            FROM PlayerWeaponInventory pw 
            JOIN WeaponDB wd ON pw.weapon_id = wd.id 
            WHERE pw.player_id = ?`, [decoded.userId]);
        console.log('Fetched weapons from DB:', weapons); // DB에서 가져온 무기 데이터 로그 출력
        res.json(weapons);
    } catch (err) {
        console.error('토큰 검증 중 오류 발생:', err); // 오류 메시지 개선
        res.status(403).json({ error: 'Invalid token' });
    } finally {
        if (conn) conn.release();
    }
});

app.post('/drawWeapon', async (req, res) => {
    const { weaponId } = req.body;
    const authHeader = req.headers.authorization;
    const token = authHeader && authHeader.split(' ')[1];
    if (!token) {
        return res.status(401).json({ error: 'No token provided' });
    }

    let conn;
    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        conn = await pool.getConnection();

        const updateResult = await conn.query(`
            UPDATE PlayerWeaponInventory 
            SET count = count + 1 
            WHERE player_id = ? AND weapon_id = ?`, [decoded.userId, weaponId]);

        if (updateResult.affectedRows === 0) {
            res.status(400).json({ error: 'Failed to update weapon count' });
        } else {
            res.status(200).json({ message: 'Weapon drawn successfully' });
        }
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
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
