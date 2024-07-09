const express = require('express');
const friendController = require('../controllers/friendController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, friendController.getFriends);
router.post('/add', authenticateToken, friendController.addFriend);
router.post('/remove', authenticateToken, friendController.removeFriend);
router.get('/requests', authenticateToken, friendController.getFriendRequests);
router.post('/requests/send', authenticateToken, friendController.sendFriendRequest);
router.post('/requests/respond', authenticateToken, friendController.respondToFriendRequest);
router.post('/requests/cancel', authenticateToken, friendController.cancelFriendRequest);

module.exports = router;
