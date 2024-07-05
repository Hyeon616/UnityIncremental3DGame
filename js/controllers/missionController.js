const pool = require('../config/db');

exports.updateMissionProgress = async (req, res) => {
    const playerId = req.user.userId;
    const { type, value } = req.body;

    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();
        
        // 먼저 MissionProgress 데이터가 있는지 확인
        const [existingProgress] = await conn.query(
            'SELECT * FROM MissionProgress WHERE player_id = ?',
            [playerId]
        );

        if (!existingProgress || existingProgress.length === 0) {
            await conn.rollback();
            return res.status(404).json({ error: 'Mission progress not found for this user' });
        }

        if (type === 'online_time') {
            const currentProgress = existingProgress[0];
            const lastCheck = new Date(currentProgress.last_online_time_check);
            const now = new Date();
            const timeDiff = Math.floor((now - lastCheck) / (1000 * 60)); // 분 단위로 계산

            const maxAllowedTimeDiff = 10; // 최대 10분
            const validTimeDiff = Math.min(timeDiff, maxAllowedTimeDiff);

            const newTotalTime = currentProgress.total_online_time + validTimeDiff;
            const newHours = Math.floor(newTotalTime / 60);
            
            await conn.query(
                'UPDATE MissionProgress SET last_online_time_check = ?, total_online_time = ?, online_time_progress = ? WHERE player_id = ?',
                [now, newTotalTime, newHours, playerId]
            );
        } else {
            await conn.query(
                `UPDATE MissionProgress SET ${type}_progress = ? WHERE player_id = ?`,
                [value, playerId]
            );
        }

        // 미션 완료 체크 로직
        const [missionProgress] = await conn.query('SELECT * FROM MissionProgress WHERE player_id = ?', [playerId]);
        const [rewards] = await conn.query('SELECT * FROM Rewards');

        let rewardsSent = false;
        for (const reward of rewards) {
            if (missionProgress[0][`${reward.Type}_progress`] >= reward.Requirement) {
                // 보상 조건 달성
                await conn.query(
                    'INSERT INTO mails (user_id, type, reward) VALUES (?, ?, ?)',
                    [playerId, 'mission_reward', JSON.stringify({ [reward.Type]: reward.Reward })]
                );

                // 미션 진행도 리셋
                await conn.query(
                    `UPDATE MissionProgress SET ${reward.Type}_progress = ${reward.Type}_progress - ? WHERE player_id = ?`,
                    [reward.Requirement, playerId]
                );

                rewardsSent = true;
            }
        }

        await conn.commit();

        if (type === 'online_time') {
            res.json({ 
                message: 'Online time updated and missions checked',
                totalTime: newTotalTime, 
                hoursCompleted: newHours,
                rewardsSent: rewardsSent
            });
        } else {
            res.json({ 
                message: 'Mission progress updated and checked',
                rewardsSent: rewardsSent
            });
        }
    } catch (err) {
        if (conn) await conn.rollback();
        console.error('Error updating mission progress:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.getMissionProgress = async (req, res) => {
    const playerId = req.user.userId;
    let conn;
    try {
        conn = await pool.getConnection();
        console.log(`Attempting to fetch mission progress for user ID: ${playerId}`);
        const rows = await conn.query('SELECT * FROM MissionProgress WHERE player_id = ?', [playerId]);
        
        console.log('Query result:', JSON.stringify(rows));
        
        if (rows && rows.length > 0) {
            console.log('Mission progress found');
            res.json(rows[0]);
        } else {
            console.log('No mission progress found for this user');
            res.status(404).json({ error: 'Mission progress not found' });
        }
    } catch (err) {
        console.error('Error retrieving mission progress:', err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
};