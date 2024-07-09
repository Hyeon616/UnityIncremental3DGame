const redis = require('redis');

let client;

async function connectRedis() {
    if (!client) {
        client = redis.createClient({
            url: process.env.REDIS_URL
        });

        client.on('error', (err) => console.error('Redis Client Error', err));

        await client.connect();
        console.log('Connected to Redis');
    }
    return client;
}

function getClient() {
    if (!client) {
        throw new Error('Redis client not initialized. Call connectRedis() first.');
    }
    return client;
}

module.exports = {
    connectRedis,
    getClient
};