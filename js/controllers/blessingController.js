// 가호
const { blessingData } = require('../data/blessingData');

exports.getBlessings = (req, res) => {
    try {
        res.json(blessingData);
    } catch (err) {
        console.error('가호 데이터 가져오기 중 오류 발생:', err);
        res.status(500).json({ error: 'Server error' });
    }
};