// 스테이지 정보
const path = require('path');
const loadXmlData = require('./loadXmlData');

const monsterXmlFilePath = path.join(__dirname, '../xml/Monsters.xml');

let stageData = {};

loadXmlData(monsterXmlFilePath, (result) => {
    stageData = {};
    result.root.row.forEach(monster => {
        let stage = monster.Stage;
        if (!stageData[stage]) {
            stageData[stage] = [];
        }
        const monsterObj = {
            name: monster.Name,
            type: monster.Type,
            health: parseInt(monster.Health, 10),
            attack: parseInt(monster.Attack, 10),
            drops: {
                money: parseInt(monster.DropMoney, 10),
                star_dust: parseInt(monster.DropStarDust, 10),
                element_stone: parseInt(monster.DropElementStone, 10),
                star_dust_chance: parseFloat(monster.DropStarDustChance),
                element_stone_chance: parseFloat(monster.DropElementStoneChance),
            }
        };
        stageData[stage].push(monsterObj);
    });
});

module.exports = { stageData };
