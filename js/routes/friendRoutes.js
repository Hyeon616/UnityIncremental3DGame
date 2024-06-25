const express = require('express');
const friendController = require('../controllers/friendController');
const authenticateToken = require('../middleware/authenticateToken');
const router = express.Router();

router.get('/', authenticateToken, friendController.getFriends);
router.post('/add', authenticateToken, friendController.addFriend);
router.post('/remove', authenticateToken, friendController.removeFriend);
router.get('/requests', authenticateToken, friendController.getFriendRequests);
router.post('/sendRequest', authenticateToken, friendController.sendFriendRequest);
router.post('/respondRequest', authenticateToken, friendController.respondToFriendRequest);
router.post('/cancelRequest', authenticateToken, friendController.cancelFriendRequest);

module.exports = router;
