// 웹서버 인증 토큰 미들웨어
const jwt = require('jsonwebtoken');

function authenticateToken(req, res, next) {
    const authHeader = req.headers['authorization']
    const token = authHeader && authHeader.split(' ')[1]
    console.log('Received token:', token);

    if (token == null) {
        console.log('No token provided');
        return res.sendStatus(401);
    }

    jwt.verify(token, process.env.JWT_SECRET, (err, user) => {
        if (err) {
            console.log('Token verification failed:', err);
            return res.sendStatus(403);
        }
        console.log('Token verified successfully for user:', user);
        req.user = user;
        next();
    });
}

module.exports = authenticateToken;
