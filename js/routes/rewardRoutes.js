// 미션 보상 엔드포인트
const express = require('express');
const rewardController = require('../controllers/rewardController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, rewardController.getRewards);
router.post('/claimReward', authenticateToken, rewardController.claimReward);

module.exports = router;
