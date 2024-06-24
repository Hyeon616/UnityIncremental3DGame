// 스킬 데이터
const path = require('path');
const loadXmlData = require('./loadXmlData');

const skillsXmlFilePath = path.join(__dirname, '../xml/Skills.xml');

let skillData = [];

loadXmlData(skillsXmlFilePath, (result) => {
    skillData = result.root.row.map(skill => ({
        id: parseInt(skill.id, 10),
        name: skill.name,
        element: skill.element,
        rarity: skill.rarity,
        damage_multiplier: parseFloat(skill.damage_multiplier)
    }));
});

module.exports = { skillData };
