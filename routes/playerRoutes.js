const express = require('express');
const router = express.Router();
const playerController = require('../controllers/playerController');
const authenticateToken = require('../middleware/authenticateToken');

router.get('/:id', authenticateToken, playerController.getPlayerData);
router.put('/:id', authenticateToken, playerController.updatePlayerData);
router.post('/:id/equip-skill', authenticateToken, playerController.equipSkill);
router.post('/:id/reset-abilities', authenticateToken, playerController.resetAbilities);

module.exports = router;