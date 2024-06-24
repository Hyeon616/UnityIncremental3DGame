// 가호 데이터
const path = require('path');
const loadXmlData = require('./loadXmlData');

const blessingsXmlFilePath = path.join(__dirname, '../xml/Blessings.xml');

let blessingData = [];

loadXmlData(blessingsXmlFilePath, (result) => {
    blessingData = result.root.row.map(blessing => ({
        id: parseInt(blessing.id, 10),
        name: blessing.name,
        element: blessing.element,
        level: parseInt(blessing.level, 10),
        attack_multiplier: parseFloat(blessing.attack_multiplier)
    }));
});

module.exports = { blessingData };
