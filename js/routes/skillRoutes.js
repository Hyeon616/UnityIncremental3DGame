// 스킬 엔드포인트
const express = require('express');
const skillController = require('../controllers/skillController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, skillController.getSkills);

module.exports = router;
