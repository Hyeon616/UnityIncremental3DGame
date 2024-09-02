// 메인 시작 서버
const express = require("express");
const cors = require("cors");
const dotenv = require("dotenv");
const path = require("path");
const { connectToMongo } = require("./config/mongodb");

dotenv.config({ path: path.join(__dirname, "config.env") });

const pool = require("./config/db");

const authRoutes = require("./routes/authRoutes");
const weaponRoutes = require("./routes/weaponRoutes");
const blessingRoutes = require("./routes/blessingRoutes");
const skillRoutes = require("./routes/skillRoutes");
const rewardRoutes = require("./routes/rewardRoutes");
const stageRoutes = require("./routes/stageRoutes");
const monsterRoutes = require("./routes/monsterRoutes");
const guildRoutes = require("./routes/guildRoutes");
const friendRoutes = require("./routes/friendRoutes");
const checkRoutes = require("./routes/checkRoutes");
const playerRoutes = require("./routes/playerRoutes");
const mailRoutes = require("./routes/mailRoutes");
const missionRoutes = require("./routes/missionRoutes");
const rankingRoutes = require("./routes/rankingRoutes");

const app = express();
const port = process.env.PORT || 3000;

app.use(cors());
app.use(express.json());

app.use("/auth", authRoutes);
app.use("/weapons", weaponRoutes);
app.use("/blessings", blessingRoutes);
app.use("/skills", skillRoutes);
app.use("/rewards", rewardRoutes);
app.use("/stages", stageRoutes);
app.use("/monsters", monsterRoutes);
app.use("/guilds", guildRoutes);
app.use("/friends", friendRoutes);
app.use("/checks", checkRoutes);
app.use("/player", playerRoutes);
app.use("/mails", mailRoutes);
app.use("/mission", missionRoutes);
app.use("/rankings", rankingRoutes);


async function startServer() {
  try {
    const db = await connectToMongo();
    app.locals.db = db;

    app.listen(port, "0.0.0.0", () => {
      console.log(`Server running on http://0.0.0.0:${port}`);
    });
  } catch (err) {
    console.error("Failed to start server:", err);
    process.exit(1);
  }
}

startServer();

process.on("SIGINT", async () => {
  try {
    await app.locals.db.client.close();
    console.log("MongoDB connection closed");
  } catch (error) {
    console.error("Error closing MongoDB connection:", error);
  } finally {
    process.exit();
  }
});