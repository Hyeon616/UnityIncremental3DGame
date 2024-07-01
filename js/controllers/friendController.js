const pool = require('../config/db');
const { getClient, connectRedis } = require('../config/redis');


// 친구 데이터를 Redis에 캐싱하는 함수
async function cacheFriends(playerId) {
    let client = getClient();
    if (!client.isOpen) {
        client = await connectRedis();
    }

    const conn = await pool.getConnection();
    try {
        const [friends] = await conn.query('SELECT * FROM Friends WHERE player_id = ?', [playerId]);
        const friendsData = friends ? friends : [];
        await client.set(`friends:${playerId}`, JSON.stringify(friendsData), 'EX', 3600);
        console.log(`플레이어 ${playerId}의 친구 목록이 성공적으로 캐싱되었습니다.`);
        return friendsData;
    } finally {
        if (conn) conn.release();
    }
}


// Redis에서 친구 목록을 가져오는 함수
async function getCachedFriends(playerId) {
    let client = getClient();
    if (!client.isOpen) {
        client = await connectRedis();
    }

    let friends = await client.get(`friends:${playerId}`);
    if (friends) {
        console.log(`플레이어 ${playerId}의 친구 목록이 성공적으로 Redis에서 로드되었습니다.`);
        return JSON.parse(friends);
    } else {
        return await cacheFriends(playerId);
    }
}

exports.getFriends = async (req, res) => {
    const playerId = req.user.userId;

    try {
        const friends = await getCachedFriends(playerId);
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

        await cacheFriends(playerId);
        await cacheFriends(friendId);

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

        await cacheFriends(playerId);
        await cacheFriends(friendId);

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

            await cacheFriends(playerId);
            await cacheFriends(request.sender_id);
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
