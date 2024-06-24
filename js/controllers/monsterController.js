// 몬스터 정보

const { monsterData } = require('../data/monsterData');

exports.getMonsters = (req, res) => {
    try {
        res.json(monsterData);
    } catch (err) {
        console.error('몬스터 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};
