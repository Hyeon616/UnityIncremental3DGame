// 메인 시작 서버
const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
dotenv.config();

const authRoutes = require('./routes/authRoutes');
const weaponRoutes = require('./routes/weaponRoutes');
const blessingRoutes = require('./routes/blessingRoutes');
const skillRoutes = require('./routes/skillRoutes');
const rewardRoutes = require('./routes/rewardRoutes');
const stageRoutes = require('./routes/stageRoutes');
const monsterRoutes = require('./routes/monsterRoutes');
const guildRoutes = require('./routes/guildRoutes');
const friendRoutes = require('./routes/friendRoutes');

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

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
