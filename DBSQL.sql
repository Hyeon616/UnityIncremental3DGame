-- 데이터베이스 사용
USE wjhdb;

-- Players 테이블 생성
CREATE TABLE Players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL,
    star_dust INT DEFAULT 0,
    pet_summon_tickets INT DEFAULT 0,
    element_stone INT DEFAULT 0,
    skill_summon_tickets INT DEFAULT 0
);

-- PlayerAttributes 테이블 생성
CREATE TABLE PlayerAttributes (
    player_id INT PRIMARY KEY,
    money INT DEFAULT 0,
    attack_power INT DEFAULT 10,
    max_health INT DEFAULT 50,
    critical_chance FLOAT DEFAULT 0,
    critical_damage FLOAT DEFAULT 0,
    level INT DEFAULT 1,
    awakening INT DEFAULT 0,
    fire_damage FLOAT DEFAULT 0,
    water_damage FLOAT DEFAULT 0,
    electric_damage FLOAT DEFAULT 0,
    wind_damage FLOAT DEFAULT 0,
    light_damage FLOAT DEFAULT 0,
    dark_damage FLOAT DEFAULT 0,
    fire_enhance INT DEFAULT 0,
    water_enhance INT DEFAULT 0,
    electric_enhance INT DEFAULT 0,
    wind_enhance INT DEFAULT 0,
    light_enhance INT DEFAULT 0,
    dark_enhance INT DEFAULT 0,
    combat_power INT AS (
        (attack_power + max_health) *
        IF(critical_chance = 0, 1, critical_chance) *
        IF(critical_damage = 0, 1, critical_damage) *
        IF(fire_damage = 0, 1, fire_damage) *
        IF(water_damage = 0, 1, water_damage) *
        IF(electric_damage = 0, 1, electric_damage) *
        IF(wind_damage = 0, 1, wind_damage) *
        IF(light_damage = 0, 1, light_damage) *
        IF(dark_damage = 0, 1, dark_damage) *
        IF(awakening = 0, 1, awakening * 10)
    ) PERSISTENT,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
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

-- PlayerWeaponInventory 테이블 생성
CREATE TABLE PlayerWeaponInventory (
    player_weapon_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    weapon_id INT NOT NULL,
    level INT DEFAULT 0,
    count INT DEFAULT 0,
    attack_power INT,
    critical_chance FLOAT,
    critical_damage FLOAT,
    max_health INT,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

-- PlayerSkills 테이블 생성
CREATE TABLE PlayerSkills (
    player_skill_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    skill_id INT NOT NULL,
    level INT DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

-- PlayerBlessings 테이블 생성
CREATE TABLE PlayerBlessings (
    player_blessing_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    blessing_id INT NOT NULL,
    level INT DEFAULT 0,
    attack_multiplier FLOAT DEFAULT 2,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
);