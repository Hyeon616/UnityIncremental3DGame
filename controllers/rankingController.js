const { ObjectId } = require('mongodb');

async function updatePlayerRanking(db, playerId, combatPower) {
  const rankings = db.collection('playerRankings');
  
  await rankings.updateOne(
    { playerId: playerId },
    { 
      $set: { 
        combatPower: combatPower,
        lastUpdated: new Date()
      }
    },
    { upsert: true }
  );
}

async function getTopRankings(db, limit = 100) {
  const rankings = db.collection('playerRankings');
  
  return await rankings.find()
    .sort({ combatPower: -1 })
    .limit(limit)
    .toArray();
}

async function getPlayerRank(db, playerId) {
  const rankings = db.collection('playerRankings');
  
  const playerRanking = await rankings.findOne({ playerId: playerId });
  if (!playerRanking) return null;

  const rank = await rankings.countDocuments({ combatPower: { $gt: playerRanking.combatPower } });
  return rank + 1;
}

module.exports = {
  updatePlayerRanking,
  getTopRankings,
  getPlayerRank
};