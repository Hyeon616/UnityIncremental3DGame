// 메인 시작 서버
const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
dotenv.config({ path: '../.env' });

const { connectRedis } = require('./config/redis');
const pool = require('./config/db');

const authRoutes = require('./routes/authRoutes');
const weaponRoutes = require('./routes/weaponRoutes');
const blessingRoutes = require('./routes/blessingRoutes');
const skillRoutes = require('./routes/skillRoutes');
const rewardRoutes = require('./routes/rewardRoutes');
const stageRoutes = require('./routes/stageRoutes');
const monsterRoutes = require('./routes/monsterRoutes');
const guildRoutes = require('./routes/guildRoutes');
const friendRoutes = require('./routes/friendRoutes');
const checkRoutes = require('./routes/checkRoutes');
const playerRoutes = require('./routes/playerRoutes');
const mailRoutes = require('./routes/mailRoutes');



const app = express();
const port = process.env.PORT || 3000;

app.use(cors());
app.use(express.json());

app.use('/auth', authRoutes);
app.use('/weapons', weaponRoutes);
app.use('/blessings', blessingRoutes);
app.use('/skills', skillRoutes);
app.use('/rewards', rewardRoutes);
app.use('/stages', stageRoutes);
app.use('/monsters', monsterRoutes);
app.use('/guilds', guildRoutes);
app.use('/friends', friendRoutes);
app.use('/checks', checkRoutes);
app.use('/player', playerRoutes);
app.use('/mails',mailRoutes);

async function startServer() {
    try {
        await connectRedis();
        await testDbConnection();
        
        app.listen(port, () => {
            console.log(`Server running on port ${port}`);
        });
    } catch (err) {
        console.error('Failed to start server:', err);
        process.exit(1);
    }
}

async function testDbConnection() {
    let conn;
    try {
        conn = await pool.getConnection();
        console.log("DB 연결 성공");
    } catch (err) {
        console.error("Error connecting to the database:", err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

startServer();

process.on('SIGINT', async () => {
    const redisClient = require('./config/redis').getClient();
    if (redisClient) {
        console.log('Closing Redis client connection...');
        await redisClient.quit();
    }
    process.exit();
});