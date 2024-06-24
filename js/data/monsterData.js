// 몬스터 데이터
const path = require('path');
const loadXmlData = require('./loadXmlData');

const monsterXmlFilePath = path.join(__dirname, '../xml/Monsters.xml');

let monsterData = [];

loadXmlData(monsterXmlFilePath, (result) => {
    monsterData = result.root.row.map(monster => ({
        stage: monster.Stage,
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
    }));
});

module.exports = { monsterData };
