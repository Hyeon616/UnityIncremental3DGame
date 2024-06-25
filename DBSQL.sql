-- 데이터베이스 사용
USE wjhdb;

-- Players 테이블 생성
CREATE TABLE Players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL,
    INDEX(player_username),
    INDEX(player_nickname)
);

-- Guilds 테이블 생성
CREATE TABLE Guilds (
    guild_id INT AUTO_INCREMENT PRIMARY KEY,
    guild_name VARCHAR(255) UNIQUE NOT NULL,
    guild_leader INT NOT NULL,
    FOREIGN KEY (guild_leader) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(guild_leader)
);

-- PlayerGuilds 연결 테이블 생성
CREATE TABLE PlayerGuilds (
    player_id INT NOT NULL,
    guild_id INT NOT NULL,
    PRIMARY KEY (player_id, guild_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE CASCADE,
    INDEX(player_id),
    INDEX(guild_id)
);

-- PlayerAttributes 테이블 생성
CREATE TABLE PlayerAttributes (
    player_id INT PRIMARY KEY,
    star_dust INT DEFAULT 0,
    element_stone INT DEFAULT 0,
    skill_summon_tickets INT DEFAULT 0,
    money INT DEFAULT 0,
    attack_power INT DEFAULT 10,
    max_health INT DEFAULT 50,
    critical_chance FLOAT DEFAULT 0,
    critical_damage FLOAT DEFAULT 0,
    current_stage VARCHAR(10) DEFAULT '1-1',
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
    guild_id INT DEFAULT NULL,
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
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE SET NULL,
    INDEX(guild_id)
);

-- mails 테이블 생성
CREATE TABLE mails (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    type ENUM('attendance', 'event', 'offline_reward', 'mission'),
    reward JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    expires_at DATETIME DEFAULT (CURRENT_TIMESTAMP + INTERVAL 3 DAY),
    is_read BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(user_id)
);

-- Friends 테이블 생성
CREATE TABLE Friends (
    player_id INT NOT NULL,
    friend_id INT NOT NULL,
    PRIMARY KEY (player_id, friend_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (friend_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(player_id),
    INDEX(friend_id)
);

-- FriendRequests 테이블 생성
CREATE TABLE FriendRequests (
    request_id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
    status ENUM('pending', 'accepted', 'rejected', 'cancelled') DEFAULT 'pending',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (sender_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (receiver_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(sender_id),
    INDEX(receiver_id)
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
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(player_id)
);

-- PlayerSkills 테이블 생성
CREATE TABLE PlayerSkills (
    player_skill_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    skill_id INT NOT NULL,
    level INT DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(player_id)
);

-- PlayerBlessings 테이블 생성
CREATE TABLE PlayerBlessings (
    player_blessing_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    blessing_id INT NOT NULL,
    level INT DEFAULT 0,
    attack_multiplier FLOAT DEFAULT 2,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(player_id)
);

-- 미션 진행 상태를 저장할 테이블 생성
CREATE TABLE MissionProgress (
    player_id INT PRIMARY KEY,
    last_level_check INT DEFAULT 0,
    last_combat_power_check INT DEFAULT 0,
    last_awakening_check INT DEFAULT 0,
    last_online_time_check DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    INDEX(player_id)
);
