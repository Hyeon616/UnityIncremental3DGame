const express = require('express');
const router = express.Router();
const { getTopRankings, getPlayerRank } = require('../controllers/rankingController');

// 상위 랭킹 조회
router.get('/top', async (req, res) => {
  try {
    const limit = parseInt(req.query.limit) || 100;
    const topRankings = await getTopRankings(limit);
    res.json(topRankings);
  } catch (error) {
    console.error('Error fetching top rankings:', error);
    res.status(500).json({ message: 'Internal server error' });
  }
});

// 특정 플레이어의 랭킹 조회
router.get('/player/:playerId', async (req, res) => {
  try {
    const playerId = parseInt(req.params.playerId);
    const rank = await getPlayerRank(playerId);
    if (rank === null) {
      res.status(404).json({ message: 'Player not found' });
    } else {
      res.json({ playerId, rank });
    }
  } catch (error) {
    console.error('Error fetching player rank:', error);
    res.status(500).json({ message: 'Internal server error' });
  }
});

module.exports = router;