const pool = require('../config/db');

async function getFriendsFromDB(playerId) {
    let conn;
    try {
        conn = await pool.getConnection();
        const friends = await conn.query('SELECT * FROM Friends WHERE player_id = ?', [playerId]);
        return friends || [];
    } catch (err) {
        console.error('친구 목록 가져오기 중 오류 발생:', err);
        throw err;
    } finally {
        if (conn) conn.release();
    }
}

exports.getFriends = async (req, res) => {
    const playerId = req.user.userId;

    try {
        const friends = await getFriendsFromDB(playerId);
        res.json(friends);
    } catch (err) {
        console.error('친구 목록 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.addFriend = async (req, res) => {
    const { friendId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        await conn.query('INSERT INTO Friends (player_id, friend_id) VALUES (?, ?)', [playerId, friendId]);
        await conn.query('INSERT INTO Friends (player_id, friend_id) VALUES (?, ?)', [friendId, playerId]);

        res.status(200).json({ message: 'Friend added successfully' });
    } catch (err) {
        console.error('친구 추가 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.removeFriend = async (req, res) => {
    const { friendId } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        await conn.query('DELETE FROM Friends WHERE player_id = ? AND friend_id = ?', [playerId, friendId]);
        await conn.query('DELETE FROM Friends WHERE player_id = ? AND friend_id = ?', [friendId, playerId]);

        res.status(200).json({ message: 'Friend removed successfully' });
    } catch (err) {
        console.error('친구 삭제 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.getFriendRequests = async (req, res) => {
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        const [requests] = await conn.query('SELECT * FROM FriendRequests WHERE receiver_id = ? AND status = ?', [playerId, 'pending']);
        res.json(requests);
    } catch (err) {
        console.error('친구 요청 목록 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.sendFriendRequest = async (req, res) => {
    const { receiverId } = req.body;
    const senderId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        await conn.query('INSERT INTO FriendRequests (sender_id, receiver_id) VALUES (?, ?)', [senderId, receiverId]);

        res.status(200).json({ message: 'Friend request sent successfully' });
    } catch (err) {
        console.error('친구 요청 전송 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.respondToFriendRequest = async (req, res) => {
    const { requestId, status } = req.body;
    const playerId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        const [requests] = await conn.query('SELECT * FROM FriendRequests WHERE request_id = ?', [requestId]);
        const request = requests[0];

        if (request.receiver_id !== playerId) {
            return res.status(403).json({ error: 'Unauthorized request' });
        }

        await conn.query('UPDATE FriendRequests SET status = ? WHERE request_id = ?', [status, requestId]);

        if (status === 'accepted') {
            await conn.query('INSERT INTO Friends (player_id, friend_id) VALUES (?, ?)', [playerId, request.sender_id]);
            await conn.query('INSERT INTO Friends (player_id, friend_id) VALUES (?, ?)', [request.sender_id, playerId]);
        }

        res.status(200).json({ message: 'Friend request responded successfully' });
    } catch (err) {
        console.error('친구 요청 응답 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.cancelFriendRequest = async (req, res) => {
    const { requestId } = req.body;
    const senderId = req.user.userId;

    let conn;
    try {
        conn = await pool.getConnection();

        const [requests] = await conn.query('SELECT * FROM FriendRequests WHERE request_id = ?', [requestId]);
        const request = requests[0];

        if (request.sender_id !== senderId) {
            return res.status(403).json({ error: 'Unauthorized request' });
        }

        await conn.query('DELETE FROM FriendRequests WHERE request_id = ?', [requestId]);

        res.status(200).json({ message: 'Friend request cancelled successfully' });
    } catch (err) {
        console.error('친구 요청 취소 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

module.exports = {
    getFriends: exports.getFriends,
    addFriend: exports.addFriend,
    removeFriend: exports.removeFriend,
    getFriendRequests: exports.getFriendRequests,
    sendFriendRequest: exports.sendFriendRequest,
    respondToFriendRequest: exports.respondToFriendRequest,
    cancelFriendRequest: exports.cancelFriendRequest
};