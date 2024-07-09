const express = require('express');
const router = express.Router();
const mailController = require('../controllers/mailController');
const authenticateToken = require('../middleware/authenticateToken');

router.get('/:userId', authenticateToken, mailController.getUserMails);
router.post('/send', authenticateToken, mailController.sendMail);
router.put('/:mailId/read', authenticateToken, mailController.markMailAsRead);
router.post('/attendance-reward', authenticateToken, mailController.sendAttendanceReward);

module.exports = router;