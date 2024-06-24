// 무기 뽑기, 합성 엔드포인트
const express = require('express');
const weaponController = require('../controllers/weaponController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.post('/drawWeapon', authenticateToken, weaponController.drawWeapon);
router.post('/synthesizeWeapon', authenticateToken, weaponController.synthesizeWeapon);
router.post('/synthesizeAllWeapons', authenticateToken, weaponController.synthesizeAllWeapons);

module.exports = router;
