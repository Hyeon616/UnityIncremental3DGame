// 스킬

const { skillData } = require('../data/skillData');

exports.getSkills = (req, res) => {
    try {
        res.json(skillData);
    } catch (err) {
        console.error('스킬 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};
