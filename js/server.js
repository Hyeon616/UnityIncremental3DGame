// 메인 시작 서버
const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
dotenv.config({ path: '../.env' });

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
const pool = require('./config/db');


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

// MySQL 연결 테스트 함수
async function testConnection() {
    let conn;
    try {
        conn = await pool.getConnection();
        console.log("DB 연결 성공");
    } catch (err) {
        console.error("Error connecting to the database:", err);
    } finally {
        if (conn) conn.release();
    }
}

// 서버 시작 시 MySQL 연결 테스트
testConnection();

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});

