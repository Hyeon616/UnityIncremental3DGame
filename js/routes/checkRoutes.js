const express = require('express');
const checkController = require('../controllers/checkController');
const router = express.Router();

router.get('/check-username', checkController.checkUsername);
router.get('/check-nickname', checkController.checkNickname);

module.exports = router;