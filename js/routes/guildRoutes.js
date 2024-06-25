const express = require('express');
const guildController = require('../controllers/guildController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, guildController.getGuilds);
router.post('/create', authenticateToken, guildController.createGuild);
router.post('/join', authenticateToken, guildController.joinGuild);
router.post('/leave', authenticateToken, guildController.leaveGuild);
router.post('/kick', authenticateToken, guildController.kickMember);
router.post('/disband', authenticateToken, guildController.disbandGuild);
router.post('/updateName', authenticateToken, guildController.updateGuildName);

module.exports = router;
