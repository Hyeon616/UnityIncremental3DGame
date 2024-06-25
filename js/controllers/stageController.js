// 스테이지 정보
const pool = require('../config/db');
const { getAsync, setAsync } = require('../config/redis');

exports.getStages = async (req, res) => {
    try {
        let stages = await getAsync('stages');
        if (!stages) {
            let conn;
            try {
                conn = await pool.getConnection();
                const [rows] = await conn.query('SELECT * FROM Stages');
                stages = JSON.stringify(rows);
                await setAsync('stages', stages, 'EX', 3600); // 1시간 동안 캐싱
                console.log('Stages 데이터가 성공적으로 데이터베이스에서 로드되었습니다.');
            } finally {
                if (conn) conn.release();
            }
        } else {
            console.log('Stages 데이터가 성공적으로 Redis에서 로드되었습니다.');
        }
        res.json(JSON.parse(stages));
    } catch (err) {
        console.error('스테이지 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};

exports.updateStage = async (req, res) => {
    const { userId, stage } = req.body;

    if (!userId || !stage) {
        return res.status(400).json({ error: 'User ID and stage are required' });
    }

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.query('UPDATE Players SET current_stage = ? WHERE player_id = ?', [stage, userId]);
        console.log(`User ID ${userId}의 스테이지가 ${stage}로 성공적으로 업데이트되었습니다.`);
        res.status(200).json({ message: 'Stage updated successfully' });
    } catch (err) {
        console.error('스테이지 업데이트 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.getCurrentStage = async (req, res) => {
    const userId = req.user.player_id;

    let conn;
    try {
        conn = await pool.getConnection();
        const [result] = await conn.query('SELECT current_stage FROM Players WHERE player_id = ?', [userId]);
        if (result.length > 0) {
            console.log(`User ID ${userId}의 현재 스테이지가 성공적으로 로드되었습니다.`);
            res.json({ current_stage: result[0].current_stage });
        } else {
            res.status(404).json({ error: 'Stage not found' });
        }
    } catch (err) {
        console.error('현재 스테이지 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};