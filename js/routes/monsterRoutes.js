// 몬스터 정보 엔드포인트
const express = require('express');
const monsterController = require('../controllers/monsterController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, monsterController.getMonsters);

module.exports = router;
