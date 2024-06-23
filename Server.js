const express = require('express');
const mariadb = require('mariadb');
const dotenv = require('dotenv');
const cors = require('cors');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt');
const fs = require('fs');
const path = require('path');
const xml2js = require('xml2js');

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

// XML 파일 경로
const weaponXmlFilePath = path.join(__dirname, 'weapon.xml');
const blessingsXmlFilePath = path.join(__dirname, 'blessings.xml');
const skillsXmlFilePath = path.join(__dirname, 'Skills.xml');
const missionRewardsXmlFilePath = path.join(__dirname, 'MissionRewards.xml');
let weaponData = [];
let blessingsData = [];
let skillsData = [];
let missionRewardsData = [];

// XML 파일에서 데이터를 읽어오는 함수
function loadXmlData(filePath, callback) {
    const xml = fs.readFileSync(filePath, 'utf-8');
    xml2js.parseString(xml, { explicitArray: false }, (err, result) => {
        if (err) {
            console.error('XML 파싱 중 오류 발생:', err);
        } else {
            callback(result);
            console.log(`${filePath} 데이터가 성공적으로 로드되었습니다.`);
        }
    });
}

function loadWeaponData() {
    loadXmlData(weaponXmlFilePath, (result) => {
        weaponData = result.root.row.map(weapon => ({
            id: parseInt(weapon.id, 10),
            rarity: weapon.rarity,
            grade: weapon.grade,
            base_attack_power: parseInt(weapon.base_attack_power, 10),
            base_critical_chance: parseFloat(weapon.base_critical_chance),
            base_critical_damage: parseFloat(weapon.base_critical_damage),
            base_max_health: parseInt(weapon.base_max_health, 10),
        }));
    });
}

function loadBlessingsData() {
    loadXmlData(blessingsXmlFilePath, (result) => {
        blessingsData = result.root.row.map(blessing => ({
            id: parseInt(blessing.id, 10),
            name: blessing.name,
            element: blessing.element,
            level: parseInt(blessing.level, 10),
            attack_multiplier: parseFloat(blessing.attack_multiplier)
        }));
    });
}

function loadSkillsData() {
    loadXmlData(skillsXmlFilePath, (result) => {
        skillsData = result.root.row.map(skill => ({
            id: parseInt(skill.id, 10),
            name: skill.name,
            element: skill.element,
            rarity: skill.rarity,
            damage_multiplier: parseFloat(skill.damage_multiplier)
        }));
    });
}

function loadMissionRewardsData() {
    loadXmlData(missionRewardsXmlFilePath, (result) => {
        missionRewardsData = result.root.row.map(reward => ({
            id: parseInt(reward.id, 10),
            type: reward.type,
            requirement: parseInt(reward.requirement, 10),
            reward: parseInt(reward.reward, 10)
        }));
    });
}

// 서버 시작 시 데이터 로드
loadWeaponData();
loadBlessingsData();
loadSkillsData();
loadMissionRewardsData();

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

        // PlayerAttributes에 기본값 추가
        await conn.query('INSERT INTO PlayerAttributes (player_id) VALUES (?)', [playerId]);

        // MissionProgress에 기본값 추가
        await conn.query('INSERT INTO MissionProgress (player_id) VALUES (?)', [playerId]);

        // 무기 데이터를 사용하여 PlayerWeaponInventory에 추가
        for (const weapon of weaponData) {
            await conn.query('INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage, max_health) VALUES (?, ?, 0, ?, ?, ?, ?)',
                [playerId, weapon.id, weapon.base_attack_power, weapon.base_critical_chance, weapon.base_critical_damage, weapon.base_max_health]);
        }

        // 가호 기본 데이터 추가
        for (const blessing of blessingsData) {
            await conn.query('INSERT INTO PlayerBlessings (player_id, blessing_id, level, attack_multiplier) VALUES (?, ?, 0, ?)', [playerId, blessing.id, blessing.attack_multiplier]);
        }

        // 스킬 기본 데이터 추가
        for (const skill of skillsData) {
            await conn.query('INSERT INTO PlayerSkills (player_id, skill_id, level) VALUES (?, ?, 0)', [playerId, skill.id]);
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

app.get('/weapons', authenticateToken, async (req, res) => {
    const { rarity } = req.query;

    try {
        let weapons;
        if (rarity) {
            weapons = weaponData.filter(weapon => weapon.rarity === rarity);
        } else {
            weapons = weaponData;
        }
        res.json(weapons);
    } catch (err) {
        console.error('무기 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
});

app.get('/blessings', authenticateToken, async (req, res) => {
    try {
        res.json(blessingsData);
    } catch (err) {
        console.error('가호 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
});


app.get('/skills', authenticateToken, async (req, res) => {
    try {
        res.json(skillsData);
    } catch (err) {
        console.error('스킬 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
});

app.get('/rewards', authenticateToken, async (req, res) => {
    try {
        res.json(missionRewardsData);
    } catch (err) {
        console.error('보상 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
});

app.post('/claimReward', authenticateToken, async (req, res) => {
    const { rewardId } = req.body;

    const reward = missionRewardsData.find(r => r.id === rewardId);
    if (!reward) {
        return res.status(400).json({ error: 'Invalid reward ID' });
    }

    let conn;
    try {
        conn = await pool.getConnection();

        // 보상을 메일로 보내기
        const query = 'INSERT INTO mails (user_id, type, reward, expires_at) VALUES (?, ?, ?, NOW() + INTERVAL 3 DAY)';
        await conn.query(query, [req.user.userId, 'mission', JSON.stringify({ star_dust: reward.reward })]);

        res.status(200).json({ message: 'Reward claimed successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});


app.post('/drawWeapon', authenticateToken, async (req, res) => {
    const { weaponId } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();

        const [weapon] = await conn.query('SELECT * FROM PlayerWeaponInventory WHERE player_id = ? AND weapon_id = ?', [req.user.userId, weaponId]);
        if (!weapon) {
            const weaponInfo = weaponData.find(w => w.id === weaponId);
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

        // Decrease the count of the original weapon
        await conn.query('UPDATE PlayerWeaponInventory SET count = count - 5 WHERE weapon_id = ? AND player_id = ?', [weaponId, req.user.userId]);

        // Get the next weapon ID
        const newWeaponId = getNextWeaponId(weaponId);

        // Increase the count of the new weapon
        await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

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

                await conn.query('UPDATE PlayerWeaponInventory SET count = count + 1 WHERE weapon_id = ? AND player_id = ?', [newWeaponId, req.user.userId]);

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

app.get('/weaponsByRarity', authenticateToken, async (req, res) => {
    const { rarity } = req.query;

    if (!rarity) {
        return res.status(400).json({ error: 'Rarity is required' });
    }

    try {
        const weapons = weaponData.filter(weapon => weapon.rarity === rarity);
        res.json(weapons);
    } catch (err) {
        console.error('무기 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});