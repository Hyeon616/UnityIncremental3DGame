const pool = require("../config/db");
const redis = require("../config/redis");
const safeStringify = require("../utils/safeStringify");

exports.getPlayerData = async (req, res) => {
  const playerId = parseInt(req.params.id);

  try {
    const conn = await pool.getConnection();

    const query = [
      "SELECT p.player_id, p.player_username, p.player_nickname,",
      "pa.element_stone, pa.skill_summon_tickets, pa.money, pa.attack_power,",
      "pa.max_health, pa.critical_chance, pa.critical_damage, pa.current_stage,",
      "pa.level, pa.awakening, pa.guild_id, pa.combat_power,",
      "pa.equipped_skill1_id, pa.equipped_skill2_id, pa.equipped_skill3_id,",
      "pa.Ability1, pa.Ability2, pa.Ability3",
      "FROM Players p",
      "JOIN PlayerAttributes pa ON p.player_id = pa.player_id",
      "WHERE p.player_id = ?"
    ].join(" ");
    
    const result = await conn.query(query, [playerId]);

    conn.release();

    if (result.length > 0) {
      const playerData = result[0];
      if (typeof playerData.combat_power === "bigint") {
        playerData.combat_power = playerData.combat_power.toString();
      }

      const realTimeRank = await getPlayerRank(playerId);
      playerData.rank = realTimeRank;

      console.log("Player data retrieved:", safeStringify(playerData));
      res.json(JSON.parse(safeStringify(playerData)));
    } else {
      res.status(404).json({ message: "Player not found" });
    }
  } catch (error) {
    console.error("Error fetching player data:", error);
    res
      .status(500)
      .json({ message: "Internal server error", error: error.message });
  }
};

exports.updatePlayerData = async (req, res) => {
  const playerId = parseInt(req.params.id);
  const updatedData = req.body;

  try {
    const conn = await pool.getConnection();

    const allowedFields = ['money', 'element_stone', 'attack_power', 'max_health', 'critical_chance', 'critical_damage', 'level', 'equipped_skill1_id', 'equipped_skill2_id', 'equipped_skill3_id'];
    const updateFields = Object.keys(updatedData).filter(key => allowedFields.includes(key));

    const updateQuery =
      "UPDATE PlayerAttributes SET " +
      updateFields.map((key) => `${key} = ?`).join(", ") +
      " WHERE player_id = ?";

    const updateValues = [...updateFields.map(key => updatedData[key]), playerId];

    await conn.query(updateQuery, updateValues);

    const [updatedPlayer] = await conn.query(
      "SELECT * FROM PlayerAttributes WHERE player_id = ?",
      [playerId]
    );

    if (updatedPlayer) {
      if (typeof updatedPlayer.combat_power === "bigint") {
        updatedPlayer.combat_power = updatedPlayer.combat_power.toString();
      }
      
      await updatePlayerRank(playerId, updatedPlayer.combat_power);

      const realTimeRank = await getPlayerRank(playerId);
      updatedPlayer.rank = realTimeRank;

      console.log("Updated player data:", safeStringify(updatedPlayer));
      res.json(JSON.parse(safeStringify(updatedPlayer)));
    } else {
      res.status(404).json({ message: "Player not found" });
    }
  } catch (error) {
    console.error("Error updating player data:", error);
    res
      .status(500)
      .json({ message: "Internal server error", error: error.message });
  } finally {
    if (conn) conn.release();
  }
};

async function getPlayerRank(playerId) {
  const redisClient = redis.getClient();
  const rank = await redisClient.zRevRank("player_ranks", playerId.toString());
  return rank !== null ? rank + 1 : null;
}

async function updatePlayerRank(playerId, combatPower) {
  const redisClient = redis.getClient();

  try {
    await redisClient.zAdd("player_ranks", {
      score: combatPower,
      value: playerId.toString(),
    });

    const rank = await redisClient.zRevRank(
      "player_ranks",
      playerId.toString()
    );

    console.log(`Updated rank for player ${playerId}: ${rank + 1}`);
  } catch (error) {
    console.error("Error updating player rank:", error);
  }
}

exports.equipSkill = async (req, res) => {
  const playerId = parseInt(req.params.id);
  const { skillId, slotNumber } = req.body;

  if (slotNumber < 1 || slotNumber > 3) {
    return res.status(400).json({ message: "Invalid slot number" });
  }

  let conn;
  try {
    conn = await pool.getConnection();

    const [playerSkill] = await conn.query(
      "SELECT * FROM PlayerSkills WHERE player_id = ? AND skill_id = ?",
      [playerId, skillId]
    );

    if (playerSkill.length === 0) {
      return res
        .status(400)
        .json({ message: "Player does not have this skill" });
    }

    await conn.query(
      `UPDATE PlayerAttributes SET equipped_skill${slotNumber}_id = ? WHERE player_id = ?`,
      [skillId, playerId]
    );

    res.json({ message: "Skill equipped successfully" });
  } catch (error) {
    console.error("Error equipping skill:", error);
    res.status(500).json({ message: "Internal server error" });
  } finally {
    if (conn) conn.release();
  }
};

async function syncRanksToDB() {
  const redisClient = redis.getClient();
  const conn = await pool.getConnection();

  try {
    const players = await redisClient.zRangeWithScores("player_ranks", 0, -1, {
      REV: true,
    });

    for (let i = 0; i < players.length; i++) {
      const { score: combatPower, value: playerId } = players[i];
      await conn.query(
        "UPDATE PlayerAttributes SET rank = ?, combat_power = ? WHERE player_id = ?",
        [i + 1, combatPower, playerId]
      );
    }
  } finally {
    conn.release();
  }
}

exports.resetAbilities = async (req, res) => {
  const playerId = parseInt(req.body.playerId);
  console.log(`Resetting abilities for player ID: ${playerId}`);

  try {
    const conn = await pool.getConnection();

    // 플레이어 존재 여부 확인
    const playerResult = await conn.query(
      "SELECT * FROM Players WHERE player_id = ?",
      [playerId]
    );

    console.log("Player query result:", playerResult);

    let player;
    if (Array.isArray(playerResult)) {
      // 결과가 배열인 경우
      player = playerResult[0];
    } else if (typeof playerResult === "object") {
      // 결과가 객체인 경우
      player = playerResult;
    }

    if (!player || (Array.isArray(player) && player.length === 0)) {
      console.log(`Player not found in Players table for ID: ${playerId}`);
      conn.release();
      return res.status(404).json({ message: "Player not found" });
    }

    // 새로운 능력치 생성
    const newAbilities = generateAbilities();
    console.log(`Generated new abilities: ${JSON.stringify(newAbilities)}`);

    // 데이터베이스 업데이트
    await conn.query(
      "UPDATE PlayerAttributes SET Ability1 = ?, Ability2 = ?, Ability3 = ? WHERE player_id = ?",
      [newAbilities[0], newAbilities[1], newAbilities[2], playerId]
    );

    // 업데이트된 플레이어 데이터 가져오기
    const updatedPlayerResult = await conn.query(
      "SELECT p.player_id, p.player_username, p.player_nickname, " +
        "pa.element_stone, pa.skill_summon_tickets, pa.money, pa.attack_power, " +
        "pa.max_health, pa.critical_chance, pa.critical_damage, pa.current_stage, " +
        "pa.level, pa.awakening, pa.guild_id, pa.combat_power, " +
        "pa.equipped_skill1_id, pa.equipped_skill2_id, pa.equipped_skill3_id, " +
        "pa.Ability1, pa.Ability2, pa.Ability3 " +
        "FROM Players p " +
        "JOIN PlayerAttributes pa ON p.player_id = pa.player_id " +
        "WHERE p.player_id = ?",
      [playerId]
    );

    conn.release();

    let updatedPlayer;
    if (Array.isArray(updatedPlayerResult)) {
      updatedPlayer = updatedPlayerResult[0];
    } else if (typeof updatedPlayerResult === "object") {
      updatedPlayer = updatedPlayerResult;
    }

    if (
      updatedPlayer &&
      (!Array.isArray(updatedPlayer) || updatedPlayer.length > 0)
    ) {
      console.log(
        `Sending updated player data: ${JSON.stringify(updatedPlayer)}`
      );
      res.json(updatedPlayer);
    } else {
      console.log(`Player not found after update for ID: ${playerId}`);
      res.status(404).json({ message: "Player not found after update" });
    }
  } catch (error) {
    console.error("Error resetting abilities:", error);
    res
      .status(500)
      .json({ message: "Internal server error", error: error.message });
  }
};

function generateAbilities() {
  const stats = [
    "attack_power",
    "max_health",
    "critical_chance",
    "critical_damage",
  ];
  const percentages = [3, 6, 9, 12];

  return Array(3)
    .fill()
    .map(() => {
      const stat = stats[Math.floor(Math.random() * stats.length)];
      const percentage =
        percentages[Math.floor(Math.random() * percentages.length)];
      return `${stat}:${percentage}`;
    });
}

function applyAbilities(player) {
  const abilities = [player.Ability1, player.Ability2, player.Ability3].filter(
    (a) => a
  );
  const bonuses = {
    attack_power: 0,
    max_health: 0,
    critical_chance: 0,
    critical_damage: 0,
  };

  abilities.forEach((ability) => {
    const [stat, percentage] = ability.split(":");
    bonuses[stat] += parseInt(percentage);
  });

  Object.keys(bonuses).forEach((stat) => {
    player[stat] *= 1 + bonuses[stat] / 100;
  });

  // combat_power 재계산 (데이터베이스의 generated column과 일치하도록 해야 함)
  player.combat_power = Math.floor(
    (player.attack_power +
      player.critical_chance +
      player.max_health +
      player.critical_damage) *
      (player.awakening === 0 ? 1 : player.awakening * 10) *
      (player.level * 0.1)
  );
}

// 5분마다 순위 동기화
setInterval(syncRanksToDB, 5 * 60 * 1000);
