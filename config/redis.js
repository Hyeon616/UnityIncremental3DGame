const redis = require('redis');
let client;

async function connectRedis() {
    if (!client) {
        client = redis.createClient({
            url: process.env.REDIS_URL,
            socket: {
                reconnectStrategy: function (times) {
                    const delay = Math.min(times * 50, 2000);
                    return delay;
                }
            }
        });

        client.on('error', (err) => console.error('Redis Client Error', err));
        client.on('connect', () => console.log('Connected to Redis'));
        client.on('reconnecting', () => console.log('Reconnecting to Redis'));

        try {
            await client.connect();
        } catch (err) {
            console.error('Failed to connect to Redis:', err);
            throw err;
        }
    }
    return client;
}

function getClient() {
    if (!client) {
        throw new Error('Redis client not initialized. Call connectRedis() first.');
    }
    return client;
}

async function setAsync(key, value) {
    const client = getClient();
    try {
        if (typeof key !== 'string' || key.trim() === '') {
            throw new Error('Invalid key');
        }
        if (value === undefined || value === null) {
            throw new Error('Invalid value');
        }
        if (typeof value !== 'string') {
            value = JSON.stringify(value);
        }
        await client.set(key, value);
        console.log(`Successfully set key: ${key}`);
    } catch (err) {
        console.error(`Failed to set key: ${key}`, err);
        if (err.message.includes('ERR syntax error')) {
            console.error('Syntax error in Redis command. Key:', key, 'Value:', value);
        }
        // 연결 재시도
        await reconnectIfNeeded();
        throw err;
    }
}

async function getAsync(key) {
    const client = getClient();
    try {
        if (typeof key !== 'string' || key.trim() === '') {
            throw new Error('Invalid key');
        }
        const value = await client.get(key);
        console.log(`Successfully got value for key: ${key}`);
        return value;
    } catch (err) {
        console.error(`Failed to get value for key: ${key}`, err);
        if (err.message.includes('ERR syntax error')) {
            console.error('Syntax error in Redis command. Key:', key);
        }
        // 연결 재시도
        await reconnectIfNeeded();
        throw err;
    }
}

async function reconnectIfNeeded() {
    if (!client.isOpen) {
        console.log('Redis connection lost. Attempting to reconnect...');
        await connectRedis();
    }
}

module.exports = {
    connectRedis,
    getClient,
    setAsync,
    getAsync,
    reconnectIfNeeded
};