const NodeCache = require('node-cache');
const cache = new NodeCache({ stdTTL: 300 }); // 5분 TTL

function setRankings(rankings) {
  cache.set('topRankings', rankings);
}

function getRankings() {
  return cache.get('topRankings') || [];
}

function setPlayerRank(playerId, rank) {
  cache.set(`playerRank:${playerId}`, rank);
}

function getPlayerRank(playerId) {
  return cache.get(`playerRank:${playerId}`);
}

module.exports = { setRankings, getRankings, setPlayerRank, getPlayerRank };