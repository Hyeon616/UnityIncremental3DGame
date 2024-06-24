// 가호 엔드포인트
const express = require('express');
const blessingController = require('../controllers/blessingController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, blessingController.getBlessings);

module.exports = router;
