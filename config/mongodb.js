const { MongoClient } = require('mongodb');

const uri = process.env.MONGODB_URI;
const client = new MongoClient(uri);

async function connectToMongo() {
  try {
    await client.connect();
    console.log("Connected to MongoDB Atlas");
    const db = client.db("playerRank");
    const playerRankings = db.collection("playerRankings");
    return { db, playerRankings };
  } catch (e) {
    console.error("Could not connect to MongoDB", e);
    process.exit(1);
  }
}

module.exports = { connectToMongo, client };