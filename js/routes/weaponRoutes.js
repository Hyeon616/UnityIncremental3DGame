const express = require('express');
const weaponController = require('../controllers/weaponController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/playerWeapons', authenticateToken, weaponController.getCachedWeapons);
router.post('/drawWeapon', authenticateToken, weaponController.drawWeapon);
router.post('/synthesizeWeapon', authenticateToken, weaponController.synthesizeWeapon);
router.post('/synthesizeAllWeapons', authenticateToken, weaponController.synthesizeAllWeapons);

module.exports = router