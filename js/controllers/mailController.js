const pool = require('../config/db');

exports.getUserMails = async (req, res) => {
    const userId = parseInt(req.params.userId);
    let conn;
    try {
        conn = await pool.getConnection();
        const [mails] = await conn.query('SELECT * FROM mails WHERE user_id = ? ORDER BY created_at DESC', [userId]);
        res.json(mails || []); 
    } catch (error) {
        console.error('Error fetching user mails:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.sendMail = async (req, res) => {
    const { userId, type, reward } = req.body;
    let conn;
    try {
        conn = await pool.getConnection();
        const [result] = await conn.query(
            'INSERT INTO mails (user_id, type, reward) VALUES (?, ?, ?)',
            [userId, type, JSON.stringify(reward)]
        );
        res.status(201).json({ message: 'Mail sent successfully', mailId: result.insertId });
    } catch (error) {
        console.error('Error sending mail:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.markMailAsRead = async (req, res) => {
    const mailId = parseInt(req.params.mailId);
    let conn;
    try {
        conn = await pool.getConnection();
        await conn.query('UPDATE mails SET is_read = TRUE WHERE id = ?', [mailId]);
        res.json({ message: 'Mail marked as read' });
    } catch (error) {
        console.error('Error marking mail as read:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};

exports.sendAttendanceReward = async (req, res) => {
    const userId = req.user.userId; // Assuming the user ID is stored in the token
    let conn;
    try {
        conn = await pool.getConnection();
        await conn.beginTransaction();
        
        const today = new Date().toISOString().split('T')[0];
        
        // Check if user has already received attendance reward today
        const [lastCheck] = await conn.query(
            'SELECT * FROM AttendanceCheck WHERE player_id = ? AND check_date = ?',
            [userId, today]
        );

        if (lastCheck.length > 0) {
            await conn.rollback();
            return res.status(400).json({ message: 'Attendance reward already claimed today' });
        }

        // Get the last attendance check
        const [lastAttendance] = await conn.query(
            'SELECT * FROM AttendanceCheck WHERE player_id = ? ORDER BY check_date DESC LIMIT 1',
            [userId]
        );

        let dayCount = 1;
        if (lastAttendance.length > 0) {
            const lastDate = new Date(lastAttendance[0].check_date);
            const diffTime = Math.abs(new Date(today) - lastDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            
            if (diffDays === 1) {
                dayCount = (lastAttendance[0].day_count % 28) + 1;
            }
        }

        // Insert new attendance check
        await conn.query(
            'INSERT INTO AttendanceCheck (player_id, check_date, day_count) VALUES (?, ?, ?)',
            [userId, today, dayCount]
        );

        // Update player's element_stone
        await conn.query(
            'UPDATE PlayerAttributes SET element_stone = element_stone + 1000 WHERE player_id = ?',
            [userId]
        );

        // Send mail
        const reward = { element_stone: 1000 };
        await conn.query(
            'INSERT INTO mails (user_id, type, reward) VALUES (?, "attendance", ?)',
            [userId, JSON.stringify(reward)]
        );

        await conn.commit();
        res.status(200).json({ message: 'Attendance reward sent successfully', dayCount: dayCount });
    } catch (error) {
        if (conn) await conn.rollback();
        console.error('Error sending attendance reward:', error);
        res.status(500).json({ message: 'Internal server error' });
    } finally {
        if (conn) conn.release();
    }
};