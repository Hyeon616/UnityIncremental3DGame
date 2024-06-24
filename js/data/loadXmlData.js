// xml 데이터 파싱
const fs = require('fs');
const xml2js = require('xml2js');

function loadXmlData(filePath, callback) {
    const xml = fs.readFileSync(filePath, 'utf-8');
    xml2js.parseString(xml, { explicitArray: false }, (err, result) => {
        if (err) {
            console.error('XML 파싱 중 오류 발생:', err);
        } else {
            callback(result);
            console.log(`${filePath} 데이터가 성공적으로 로드되었습니다.`);
        }
    });
}

module.exports = loadXmlData;