// 스테이지 정보 엔드포인트
const express = require('express');
const stageController = require('../controllers/stageController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, stageController.getStages);
router.get('/currentStage', authenticateToken, stageController.getCurrentStage);
router.post('/updateStage', authenticateToken, stageController.updateStage);

module.exports = router;
