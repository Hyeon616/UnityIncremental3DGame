-- 기존 테이블 삭제 (이미 존재하는 경우)
DROP TABLE IF EXISTS Friends;
DROP TABLE IF EXISTS PlayerWeapon;
DROP TABLE IF EXISTS WeaponDB;
DROP TABLE IF EXISTS Guilds;
DROP TABLE IF EXISTS Players;
DROP TABLE IF EXISTS PlayerWeaponInventory;

-- 데이터베이스 사용
USE wjhdb;

-- Players 테이블 생성
CREATE TABLE Players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL
);

-- Guilds 테이블 생성
CREATE TABLE Guilds (
    guild_id INT AUTO_INCREMENT PRIMARY KEY,
    guild_name VARCHAR(255) UNIQUE NOT NULL,
    guild_leader INT NOT NULL,
    FOREIGN KEY (guild_leader) REFERENCES Players(player_id) ON DELETE CASCADE
);

-- Players 테이블에 guild_id 컬럼 추가 및 외래 키 설정
ALTER TABLE Players ADD COLUMN guild_id INT DEFAULT NULL;
ALTER TABLE Players ADD CONSTRAINT FK_Guild FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE SET NULL;

-- Friends 테이블 생성
CREATE TABLE Friends (
    player_id INT NOT NULL,
    friend_id INT NOT NULL,
    PRIMARY KEY (player_id, friend_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (friend_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

-- WeaponDB 테이블 생성
CREATE TABLE WeaponDB (
    id INT AUTO_INCREMENT PRIMARY KEY,
    rarity VARCHAR(20),
    grade VARCHAR(20),
    base_attack_power INT,
    base_critical_chance FLOAT,
    base_critical_damage FLOAT,
    base_max_health INT
);

-- PlayerWeaponInventory 테이블 생성
CREATE TABLE PlayerWeaponInventory (
    player_weapon_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    weapon_id INT NOT NULL,
    level INT DEFAULT 1,
    count INT DEFAULT 1,
    attack_power INT,
    critical_chance FLOAT,
    critical_damage FLOAT,
    max_health INT,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (weapon_id) REFERENCES WeaponDB(id) ON DELETE CASCADE
);

-- 무기 데이터 삽입 예시

-- 일반 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('일반', '하급', 25, 0, 0.1, 100),
('일반', '중급', 50, 0, 0.1, 250),
('일반', '상급', 100, 0, 0.1, 500),
('일반', '최상급', 150, 0, 0.1, 750);

-- 고급 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('고급', '하급', 200, 0, 0.2, 1000),
('고급', '중급', 250, 0, 0.2, 1250),
('고급', '상급', 300, 0, 0.2, 1500),
('고급', '최상급', 350, 0, 0.2, 1750);

-- 매직 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('매직', '하급', 400, 0, 0.4, 2000),
('매직', '중급', 450, 0, 0.4, 2250),
('매직', '상급', 500, 0, 0.4, 2500),
('매직', '최상급', 550, 0, 0.4, 2750);

-- 유물 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('유물', '하급', 600, 0.2, 0.8, 3000),
('유물', '중급', 650, 0.2, 0.8, 3250),
('유물', '상급', 700, 0.2, 0.8, 3500),
('유물', '최상급', 750, 0.2, 0.8, 3750);

-- 영웅 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('영웅', '하급', 800, 0.4, 1.6, 4000),
('영웅', '중급', 850, 0.4, 1.6, 4250),
('영웅', '상급', 900, 0.4, 1.6, 4500),
('영웅', '최상급', 950, 0.4, 1.6, 4750);

-- 에픽 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('에픽', '하급', 1000, 0.6, 3.2, 5000),
('에픽', '중급', 1050, 0.6, 3.2, 5250),
('에픽', '상급', 1100, 0.6, 3.2, 5500),
('에픽', '최상급', 1150, 0.6, 3.2, 5750);

-- 고대 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('고대', '하급', 1200, 0.8, 6.4, 6000),
('고대', '중급', 1250, 0.8, 6.4, 6250),
('고대', '상급', 1300, 0.8, 6.4, 6500),
('고대', '최상급', 1350, 0.8, 6.4, 6750);

-- 신화 등급 무기
INSERT INTO WeaponDB (rarity, grade, base_attack_power, base_critical_chance, base_critical_damage, base_max_health) VALUES
('신화', '하급', 1400, 1.0, 12.8, 7000),
('신화', '중급', 1450, 1.0, 12.8, 7250),
('신화', '상급', 1500, 1.0, 12.8, 7500),
('신화', '최상급', 1550, 1.0, 12.8, 7750);
