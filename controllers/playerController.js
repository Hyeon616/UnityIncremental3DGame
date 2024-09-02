const pool = require("../config/db");
const safeStringify = require("../utils/safeStringify");
const { updatePlayerRanking, getPlayerRankCached } = require('./rankingController');

exports.getPlayerData = async (req, res) => {
  const playerId = parseInt(req.params.id);

  try {
    const conn = await pool.getConnection();
    const db = req.app.locals.db;

    const query = [
      "SELECT p.player_id, p.player_username, p.player_nickname,",
      "pa.base_element_stone, pa.base_skill_summon_tickets, pa.base_money,",
      "pa.base_attack_power, pa.base_max_health, pa.base_critical_chance, pa.base_critical_damage,",
      "pa.element_stone, pa.skill_summon_tickets, pa.money,",
      "pa.attack_power, pa.max_health, pa.critical_chance, pa.critical_damage,",
      "pa.current_stage, pa.level, pa.awakening, pa.guild_id, pa.combat_power,",
      "pa.equipped_skill1_id, pa.equipped_skill2_id, pa.equipped_skill3_id,",
      "pa.Ability1, pa.Ability2, pa.Ability3, pa.Ability4, pa.Ability5, pa.Ability6,",
      "pa.Ability7, pa.Ability8, pa.Ability9, pa.Ability10, pa.Ability11, pa.Ability12",
      "FROM Players p",
      "JOIN PlayerAttributes pa ON p.player_id = pa.player_id",
      "WHERE p.player_id = ?"
    ].join(" ");
    
    const result = await conn.query(query, [playerId]);

    if (result.length > 0) {
      const playerData = result[0];
      if (typeof playerData.combat_power === "bigint") {
        playerData.combat_power = playerData.combat_power.toString();
      }

      const realTimeRank = await getPlayerRank(db, playerId);
      playerData.rank = realTimeRank;

      res.json(JSON.parse(safeStringify(playerData)));
    } else {
      res.status(404).json({ message: "Player not found" });
    }
  } catch (error) {
    console.error("Error fetching player data:", error);
    res.status(500).json({ message: "Internal server error", error: error.message });
  } finally {
    if (conn) conn.release();
  }
};

exports.updatePlayerData = async (req, res) => {
  const playerId = parseInt(req.params.id);
  const updatedData = req.body;

  try {
    const conn = await pool.getConnection();
    const db = req.app.locals.db;

    const allowedFields = ['base_money', 'base_element_stone', 'base_attack_power', 'base_max_health', 'base_critical_chance', 'base_critical_damage', 'level', 'equipped_skill1_id', 'equipped_skill2_id', 'equipped_skill3_id'];
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
      await updatePlayerRanking(db, playerId, updatedPlayer.combat_power);

      const rank = await getPlayerRank(db, playerId);
      updatedPlayer.rank = rank;

      res.json(updatedPlayer);
    } else {
      res.status(404).json({ message: "Player not found" });
    }
  } catch (error) {
    console.error("Error updating player data:", error);
    res.status(500).json({ message: "Internal server error", error: error.message });
  } finally {
    if (conn) conn.release();
  }
};

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


exports.resetAbilities = async (req, res) => {
  const playerId = parseInt(req.body.playerId);
  const abilitySetIndex = parseInt(req.body.abilityIndex);
  console.log(`Resetting abilities for player ID: ${playerId}, set index: ${abilitySetIndex}`);

  try {
    const conn = await pool.getConnection();
    const db = req.app.locals.db;
    await conn.beginTransaction();

    const newAbilities = generateAbilities();

    const updateQuery = `
      UPDATE PlayerAttributes 
      SET Ability${abilitySetIndex * 3 + 1} = ?, 
          Ability${abilitySetIndex * 3 + 2} = ?, 
          Ability${abilitySetIndex * 3 + 3} = ? 
      WHERE player_id = ?;
    `;

    await conn.query(updateQuery, [...newAbilities, playerId]);

    const selectQuery = `
      SELECT 
        attack_power, max_health, critical_chance, critical_damage, combat_power,
        Ability1, Ability2, Ability3, Ability4, Ability5, Ability6,
        Ability7, Ability8, Ability9, Ability10, Ability11, Ability12
      FROM PlayerAttributes
      WHERE player_id = ?;
    `;

    const [updatedPlayer] = await conn.query(selectQuery, [playerId]);

    await conn.commit();

    if (updatedPlayer) {
      await updatePlayerRanking(db, playerId, updatedPlayer.combat_power);
      const realTimeRank = await getPlayerRank(db, playerId);
      updatedPlayer.rank = realTimeRank;

      console.log(`Sending updated player data: ${JSON.stringify(updatedPlayer)}`);
      res.json(updatedPlayer);
    } else {
      console.log(`Player not found for ID: ${playerId}`);
      res.status(404).json({ message: "Player not found" });
    }
  } catch (error) {
    if (conn) await conn.rollback();
    console.error("Error resetting abilities:", error);
    res.status(500).json({ message: "Internal server error", error: error.message });
  } finally {
    if (conn) conn.release();
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

