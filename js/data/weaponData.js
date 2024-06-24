// 무기 데이터
const path = require('path');
const loadXmlData = require('./loadXmlData');

const weaponXmlFilePath = path.join(__dirname, '../xml/Weapon.xml');

let weaponData = [];

loadXmlData(weaponXmlFilePath, (result) => {
    weaponData = result.root.row.map(weapon => ({
        id: parseInt(weapon.id, 10),
        rarity: weapon.rarity,
        grade: weapon.grade,
        base_attack_power: parseInt(weapon.base_attack_power, 10),
        base_critical_chance: parseFloat(weapon.base_critical_chance),
        base_critical_damage: parseFloat(weapon.base_critical_damage),
        base_max_health: parseInt(weapon.base_max_health, 10),
    }));
});

module.exports = { weaponData };
