const express = require('express');
const mariadb = require('mariadb');
const dotenv = require('dotenv');
const cors = require('cors');

dotenv.config();

const app = express();
const port = process.env.PORT || 3000;

const pool = mariadb.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_DATABASE,
    port: process.env.DB_PORT,
    //connectionLimit: 5
});

app.use(cors());

async function testDBConnection() {
    let conn;
    try {
        conn = await pool.getConnection();
        await conn.query('SELECT 1');
        console.log('Database connection successful');
    } catch (err) {
        console.error('Database connection failed:', err);
        process.exit(1); // DB 연결 실패 시 프로세스 종료
    } finally {
        if (conn) conn.release();
    }
}

app.get('/weapons', async (req, res) => {
    let conn;
    try {
        conn = await pool.getConnection();
        const rows = await conn.query('SELECT * FROM WeaponDB');
        res.json(rows);
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Database error' });
    } finally {
        if (conn) conn.release();
    }
});

app.listen(port, async () => {
    await testDBConnection(); // 서버 시작 전 DB 연결 테스트
    console.log(`Server running on port ${port}`);
});
