// 미션 보상 데이터
const path = require('path');
const loadXmlData = require('./loadXmlData');

const missionRewardsXmlFilePath = path.join(__dirname, '../xml/MissionRewards.xml');

let rewardData = [];

loadXmlData(missionRewardsXmlFilePath, (result) => {
    rewardData = result.root.row.map(reward => ({
        id: parseInt(reward.id, 10),
        type: reward.type,
        requirement: parseInt(reward.requirement, 10),
        reward: parseInt(reward.reward, 10)
    }));
});

module.exports = { rewardData };
