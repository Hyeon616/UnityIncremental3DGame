const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const pool = require("../config/db");
const { getCachedWeapons } = require("../controllers/weaponController");
const { getCachedSkills } = require("../controllers/skillController");
const redis = require("../config/redis");
const safeStringify = require("../utils/safeStringify");
exports.register = async (req, res) => {
  const { username, password, nickname } = req.body;
  if (!username || !password || !nickname) {
    console.log("Invalid input detected");
    return res.status(400).json({ error: "Invalid input" });
  }

  let conn;
  try {
    conn = await pool.getConnection();
    await conn.beginTransaction();

    const existingUsers = await conn.query(
      "SELECT * FROM Players WHERE player_username = ? OR player_nickname = ?",
      [username, nickname]
    );

    if (Array.isArray(existingUsers) && existingUsers.length > 0) {
      return res
        .status(400)
        .json({ error: "Username or nickname already exists" });
    }

    const hashedPassword = await bcrypt.hash(password, 10);
    const insertResult = await conn.query(
      "INSERT INTO Players (player_username, player_password, player_nickname) VALUES (?, ?, ?)",
      [username, hashedPassword, nickname]
    );

    console.log("Player inserted:", insertResult);

    let playerId;
    if (Array.isArray(insertResult)) {
      playerId = insertResult[0].insertId;
    } else if (insertResult && insertResult.insertId) {
      playerId = insertResult.insertId;
    } else {
      throw new Error("Failed to get new player ID");
    }
    const attributeResult = await conn.query(
      "INSERT INTO PlayerAttributes (player_id) VALUES (?)",
      [playerId]
    );
    console.log("PlayerAttributes inserted:", attributeResult);
    const missionResult = await conn.query(
      "INSERT INTO MissionProgress (player_id, level_progress, combat_power_progress, awakening_progress, online_time_progress, weapon_level_sum_progress) VALUES (?, 0, 0, 0, 0, 0)",
      [playerId]
    );

    const weapons = await getCachedWeapons();

    if (Array.isArray(weapons) && weapons.length > 0) {
      const weaponInserts = weapons.map((weapon) => [
        playerId,
        weapon.weapon_id,
        0, // count
        weapon.attack_power,
        parseFloat(weapon.crit_rate),
        parseFloat(weapon.crit_damage),
      ]);

      const result = await conn.batch(
        "INSERT INTO PlayerWeaponInventory (player_id, weapon_id, count, attack_power, critical_chance, critical_damage) VALUES (?, ?, ?, ?, ?, ?)",
        weaponInserts
      );
    } else {
      console.log("No weapons available for new user");
    }

    const skills = await getCachedSkills();
    if (skills && skills.length > 0) {
      const skillInserts = skills.map((skill) => [
        playerId,
        skill.id,
        1, // 초기 레벨
      ]);

      console.log(
        `Preparing to insert ${skillInserts.length} skills for new user`
      );

      const skillResult = await conn.batch(
        "INSERT INTO PlayerSkills (player_id, skill_id, level) VALUES (?, ?, ?)",
        skillInserts
      );
      console.log(`Inserted ${skillResult.affectedRows} skills for new user`);
    } else {
      console.log("No skills available for new user");
    }

    // 새 플레이어의 순위 업데이트
    await conn.query("CALL add_new_player_rank(?)", [playerId]);

    await conn.commit();

    // Redis 순위 업데이트
    const redisClient = redis.getClient();
    await conn.query("CALL add_new_player_rank(?)", [playerId]);

    const [playerData] = await conn.query(
      "SELECT combat_power FROM PlayerAttributes WHERE player_id = ?",
      [playerId]
    );

    await redisClient.zAdd("player_ranks", {
      score: playerData.combat_power,
      value: playerId.toString(),
    });
    res
      .status(201)
      .json(
        JSON.parse(
          safeStringify({ message: "User registered successfully", playerId })
        )
      );
  } catch (err) {
    if (conn) await conn.rollback();
    console.error("Registration error:", err);
    if (err.code === "ER_BAD_NULL_ERROR") {
      console.error("Null value detected. Request body:", req.body);
      console.error("SQL query:", err.sql);
    }
    res.status(500).json({ error: "Registration failed: " + err.message });
  } finally {
    if (conn) conn.release();
  }
};

exports.login = async (req, res) => {
  const { username, password } = req.body;
  if (!username || !password) {
    console.log("Invalid input detected");
    return res.status(400).json({ error: "Invalid username or password" });
  }

  let conn;
  try {
    conn = await pool.getConnection();

    const result = await conn.query(
      "SELECT * FROM Players WHERE player_username = ?",
      [username]
    );

    let user;
    if (Array.isArray(result) && result.length > 0) {
      user = result[0];
    } else if (result && typeof result === "object") {
      user = result;
    }

    if (!user || !user.player_password) {
      console.log("Invalid user data");
      return res.status(400).json({ error: "Invalid username or password" });
    }

    const validPassword = await bcrypt.compare(password, user.player_password);

    if (!validPassword) {
      return res.status(400).json({ error: "Invalid username or password" });
    }

    const token = jwt.sign({ userId: user.player_id }, process.env.JWT_SECRET, {
      expiresIn: "1h",
    });
    const playerData = {
      token: token,
      userId: user.player_id,
      current_stage: user.current_stage || "1-1",
    };

    res.json(JSON.parse(safeStringify(playerData)));
  } catch (err) {
    console.error("Login error:", err);
    res.status(500).json({ error: "Database error" });
  } finally {
    if (conn) conn.release();
  }
};
