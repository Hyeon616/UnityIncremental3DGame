const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

// 길드 데이터를 Redis에 캐싱하는 함수
async function cacheGuilds() {
    const conn = await pool.getConnection();
    try {
        const [guilds] = await conn.query('SELECT * FROM Guilds');
        await setAsync('guilds', JSON.stringify(guilds), 'EX', 3600);
        console.log('길드 목록이 성공적으로 캐싱되었습니다.');
        return guilds;
    } finally {
        if (conn) conn.release();
    }
}

// Redis에서 길드 목록을 가져오는 함수
async function getCachedGuilds() {
    let guilds = await getAsync('guilds');
    if (guilds) {
        console.log('길드 목록이 성공적으로 Redis에서 로드되었습니다.');
        return JSON.parse(guilds);
    } else {
        return await cacheGuilds();
    }
}

exports.getGuilds = async (req, res) => {
    try {
        const guilds = await getCachedGuilds();
        res.json(guilds);
    } catch (err) {
        console.error('길드 목록 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.createGuild = async (req, res) => {
    const { guildName } = req.body;
    const guildLeader = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        const result = await conn.query('INSERT INTO Guilds (guild_name, guild_leader) VALUES (?, ?)', [guildName, guildLeader]);
        const guildId = result.insertId;
        await conn.query('INSERT INTO PlayerGuilds (player_id, guild_id) VALUES (?, ?)', [guildLeader, guildId]);
        await conn.query('UPDATE PlayerAttributes SET guild_id = ? WHERE player_id = ?', [guildId, guildLeader]);

        await cacheGuilds();
        res.status(200).json({ message: 'Guild created successfully', guildId });
    } catch (err) {
        console.error('길드 생성 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.joinGuild = async (req, res) => {
    const { guildId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.query('INSERT INTO PlayerGuilds (player_id, guild_id) VALUES (?, ?)', [playerId, guildId]);
        await conn.query('UPDATE PlayerAttributes SET guild_id = ? WHERE player_id = ?', [guildId, playerId]);

        await cacheGuilds();
        res.status(200).json({ message: 'Joined guild successfully' });
    } catch (err) {
        console.error('길드 가입 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.leaveGuild = async (req, res) => {
    const { guildId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.query('DELETE FROM PlayerGuilds WHERE player_id = ? AND guild_id = ?', [playerId, guildId]);
        await conn.query('UPDATE PlayerAttributes SET guild_id = NULL WHERE player_id = ?', [playerId]);

        await cacheGuilds();
        res.status(200).json({ message: 'Left guild successfully' });
    } catch (err) {
        console.error('길드 탈퇴 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.kickMember = async (req, res) => {
    const { guildId, memberId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        const [guilds] = await conn.query('SELECT guild_leader FROM Guilds WHERE guild_id = ?', [guildId]);
        if (guilds.length === 0 || guilds[0].guild_leader !== playerId) {
            return res.status(403).json({ error: 'Only the guild leader can kick members' });
        }

        await conn.query('DELETE FROM PlayerGuilds WHERE player_id = ? AND guild_id = ?', [memberId, guildId]);
        await conn.query('UPDATE PlayerAttributes SET guild_id = NULL WHERE player_id = ?', [memberId]);

        await cacheGuilds();
        res.status(200).json({ message: 'Member kicked successfully' });
    } catch (err) {
        console.error('길드원 강퇴 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.disbandGuild = async (req, res) => {
    const { guildId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        const [guilds] = await conn.query('SELECT guild_leader FROM Guilds WHERE guild_id = ?', [guildId]);
        if (guilds.length === 0 || guilds[0].guild_leader !== playerId) {
            return res.status(403).json({ error: 'Only the guild leader can disband the guild' });
        }

        await conn.query('DELETE FROM PlayerGuilds WHERE guild_id = ?', [guildId]);
        await conn.query('UPDATE PlayerAttributes SET guild_id = NULL WHERE guild_id = ?', [guildId]);
        await conn.query('DELETE FROM Guilds WHERE guild_id = ?', [guildId]);

        await cacheGuilds();
        res.status(200).json({ message: 'Guild disbanded successfully' });
    } catch (err) {
        console.error('길드 해체 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.updateGuildName = async (req, res) => {
    const { guildId, newName } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();
        const [guilds] = await conn.query('SELECT guild_leader FROM Guilds WHERE guild_id = ?', [guildId]);
        if (guilds.length === 0 || guilds[0].guild_leader !== playerId) {
            return res.status(403).json({ error: 'Only the guild leader can update the guild name' });
        }

        await conn.query('UPDATE Guilds SET guild_name = ? WHERE guild_id = ?', [newName, guildId]);

        await cacheGuilds();
        res.status(200).json({ message: 'Guild name updated successfully' });
    } catch (err) {
        console.error('길드 이름 변경 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};
