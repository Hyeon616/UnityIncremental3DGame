const express = require('express');
const missionController = require('../controllers/missionController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/progress', authenticateToken, missionController.getMissionProgress);
router.post('/progress', authenticateToken, missionController.updateMissionProgress);

module.exports = router;