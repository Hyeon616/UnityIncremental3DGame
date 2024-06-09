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

// authenticateToken 미들웨어 정의
function authenticateToken(req, res, next) {
    const authHeader = req.headers.authorization;
    const token = authHeader && authHeader.split(' ')[1];

    if (token == null) return res.sendStatus(401);

    jwt.verify(token, process.env.JWT_SECRET, (err, user) => {
        if (err) return res.sendStatus(403);
        req.user = user;
        next();
    });
}

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

        const [weapon] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ? AND weapon_id = ?', [decoded.userId, weaponId]);
        if (!weapon) {
            const [weaponInfo] = await conn.query('SELECT * FROM WeaponDB WHERE id = ?', [weaponId]);
            if (!weaponInfo) {
                return res.status(400).json({ error: 'Invalid weapon ID' });
            }
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)',
                [decoded.userId, weaponId, weaponInfo.base_attack_power, weaponInfo.base_critical_chance, weaponInfo.base_critical_damage, weaponInfo.base_max_health]);
        } else {
            const updateResult = await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE player_id = ? AND weapon_id = ?', [decoded.userId, weaponId]);
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

app.post('/synthesizeWeapon', authenticateToken, async (req, res) => {
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

        const newWeaponId = getNextWeaponId(weaponId); // 무기 ID 합성 로직은 별도 구현 필요

        // 기존에 동일한 무기가 있는지 확인
        const existingWeapon = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

        if (existingWeapon.length > 0) {
            // 기존 무기가 있으면 count 증가
            await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);
        } else {
            // 기존 무기가 없으면 새 행 추가
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)',
                [req.user.userId, newWeaponId, weapon[0].attack_power, weapon[0].critical_chance, weapon[0].critical_damage, weapon[0].max_health]);
        }

        res.status(200).json({ message: 'Weapon synthesized successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});

app.post('/synthesizeAllWeapons', authenticateToken, async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();

        const weapons = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ?', [req.user.userId]);

        for (const weapon of weapons) {
            while (weapon.count >= 5) {
                await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weapon.weapon_id, req.user.userId]);

                const newWeaponId = getNextWeaponId(weapon.weapon_id);

                const existingWeapon = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

                if (existingWeapon.length > 0) {
                    await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);
                } else {
                    await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 1, ?, ?, ?, ?)',
                        [req.user.userId, newWeaponId, weapon.attack_power, weapon.critical_chance, weapon.critical_damage, weapon.max_health]);
                }

                // Fetch updated count
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
});
function getNextWeaponId(currentWeaponId) {
    // 무기 ID 합성 로직 구현
    const nextWeaponMap = {
        // 일반 등급
        1: 2,   // 일반 하급 -> 일반 중급
        2: 3,   // 일반 중급 -> 일반 상급
        3: 4,   // 일반 상급 -> 일반 최상급
        4: 5,   // 일반 최상급 -> 고급 하급

        // 고급 등급
        5: 6,   // 고급 하급 -> 고급 중급
        6: 7,   // 고급 중급 -> 고급 상급
        7: 8,   // 고급 상급 -> 고급 최상급
        8: 9,   // 고급 최상급 -> 매직 하급

        // 매직 등급
        9: 10,  // 매직 하급 -> 매직 중급
        10: 11, // 매직 중급 -> 매직 상급
        11: 12, // 매직 상급 -> 매직 최상급
        12: 13, // 매직 최상급 -> 유물 하급

        // 유물 등급
        13: 14, // 유물 하급 -> 유물 중급
        14: 15, // 유물 중급 -> 유물 상급
        15: 16, // 유물 상급 -> 유물 최상급
        16: 17, // 유물 최상급 -> 영웅 하급

        // 영웅 등급
        17: 18, // 영웅 하급 -> 영웅 중급
        18: 19, // 영웅 중급 -> 영웅 상급
        19: 20, // 영웅 상급 -> 영웅 최상급
        20: 21, // 영웅 최상급 -> 에픽 하급

        // 에픽 등급
        21: 22, // 에픽 하급 -> 에픽 중급
        22: 23, // 에픽 중급 -> 에픽 상급
        23: 24, // 에픽 상급 -> 에픽 최상급
        24: 25, // 에픽 최상급 -> 고대 하급

        // 고대 등급
        25: 26, // 고대 하급 -> 고대 중급
        26: 27, // 고대 중급 -> 고대 상급
        27: 28, // 고대 상급 -> 고대 최상급
        28: 29, // 고대 최상급 -> 신화 하급

        // 신화 등급
        29: 30, // 신화 하급 -> 신화 중급
        30: 31, // 신화 중급 -> 신화 상급
        31: 32, // 신화 상급 -> 신화 최상급
        // 신화 최상급은 최상위 등급이므로 변환 없음
    };

    return nextWeaponMap[currentWeaponId] || currentWeaponId;
}

app.get('/fetchWeaponByRarity', authenticateToken, async (req, res) => {
    const { rarity } = req.query;

    if (!rarity) {
        return res.status(400).json({ error: 'Rarity is required' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        const weapon = await conn.query('SELECT * FROM WeaponDB WHERE rarity = ? ORDER BY RAND() LIMIT 1', [rarity]);
        if (weapon.length === 0) {
            return res.status(404).json({ error: 'No weapon found for the given rarity' });
        }

        res.status(200).json(weapon[0]);
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
