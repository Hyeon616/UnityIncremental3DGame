-- 새로운 데이터베이스 생성 (이미 존재한다면 사용)
CREATE DATABASE IF NOT EXISTS wjhdb;
USE wjhdb;

-- 외래 키 체크 비활성화
SET FOREIGN_KEY_CHECKS = 0;

-- 테이블 생성
CREATE TABLE Players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL
);

CREATE INDEX idx_player_username ON Players(player_username);
CREATE INDEX idx_player_nickname ON Players(player_nickname);

CREATE TABLE Monsters (
    id INT PRIMARY KEY,
    Stage VARCHAR(50),
    Type VARCHAR(50),
    Name VARCHAR(50),
    Health INT,
    Attack INT,
    DropMoney INT,
    DropElementStone INT,
    DropElementStoneChance DECIMAL(5, 2),
    PrefabName VARCHAR(50)
);

CREATE INDEX idx_monster_stage ON Monsters(Stage);
CREATE INDEX idx_monster_type ON Monsters(Type);

CREATE TABLE Rewards (
    ID INT PRIMARY KEY,
    Type VARCHAR(50),
    Description VARCHAR(255),
    Requirement INT,
    Reward INT
);

CREATE TABLE Skills (
    id INT PRIMARY KEY,
    name VARCHAR(255),
    description TEXT,
    damage_percentage INT,
    image VARCHAR(255),
    cooldown INT
);

CREATE INDEX idx_skill_name ON Skills(name);

CREATE TABLE Weapon (
    weapon_id INT PRIMARY KEY,
    weapon_grade INT,
    attack_power INT,
    crit_rate DECIMAL(5, 2),
    crit_damage DECIMAL(5, 2),
    weapon_exp BIGINT,
    prefab_name VARCHAR(255)
);

CREATE INDEX idx_weapon_grade ON Weapon(weapon_grade);

CREATE TABLE Guilds (
    guild_id INT AUTO_INCREMENT PRIMARY KEY,
    guild_name VARCHAR(255) UNIQUE NOT NULL,
    guild_leader INT NOT NULL,
    FOREIGN KEY (guild_leader) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE INDEX idx_guild_name ON Guilds(guild_name);

CREATE TABLE PlayerGuilds (
    player_id INT NOT NULL,
    guild_id INT NOT NULL,
    PRIMARY KEY (player_id, guild_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE CASCADE
);

CREATE TABLE PlayerAttributes (
    player_id INT PRIMARY KEY,
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
    guild_id INT DEFAULT NULL,
    equipped_skill1_id INT DEFAULT NULL,
    equipped_skill2_id INT DEFAULT NULL,
    equipped_skill3_id INT DEFAULT NULL,
    Ability1 VARCHAR(20) DEFAULT NULL,
    Ability2 VARCHAR(20) DEFAULT NULL,
    Ability3 VARCHAR(20) DEFAULT NULL,
     combat_power VARCHAR(255) GENERATED ALWAYS AS (
        CAST((
            attack_power * (1 + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability1, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability2, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability3, ':', -1) AS DECIMAL(5,2)) / 100, 0)
            ) +
            critical_chance * (1 + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability1, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability2, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability3, ':', -1) AS DECIMAL(5,2)) / 100, 0)
            ) +
            max_health * (1 + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability1, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability2, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability3, ':', -1) AS DECIMAL(5,2)) / 100, 0)
            ) +
            critical_damage * (1 + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability1, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability2, ':', -1) AS DECIMAL(5,2)) / 100, 0) + 
                IFNULL(CAST(SUBSTRING_INDEX(Ability3, ':', -1) AS DECIMAL(5,2)) / 100, 0)
            )
        ) * 
        (IF(awakening = 0, 1, awakening * 10)) * 
        (level * 0.1) AS CHAR(255))
    ) STORED,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE SET NULL,
    FOREIGN KEY (equipped_skill1_id) REFERENCES Skills(id),
    FOREIGN KEY (equipped_skill2_id) REFERENCES Skills(id),
    FOREIGN KEY (equipped_skill3_id) REFERENCES Skills(id)
);

CREATE INDEX idx_player_current_stage ON PlayerAttributes(current_stage);
CREATE INDEX idx_player_combat_power ON PlayerAttributes(combat_power);

CREATE TABLE mails (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    type ENUM('attendance', 'event', 'offline_reward', 'mission'),
    reward JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    expires_at DATETIME DEFAULT (CURRENT_TIMESTAMP + INTERVAL 3 DAY),
    is_read BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE INDEX idx_mail_user_id ON mails(user_id);
CREATE INDEX idx_mail_type ON mails(type);

CREATE TABLE Friends (
    player_id INT NOT NULL,
    friend_id INT NOT NULL,
    PRIMARY KEY (player_id, friend_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (friend_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE FriendRequests (
    request_id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
    status ENUM('pending', 'accepted', 'rejected', 'cancelled') DEFAULT 'pending',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (sender_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (receiver_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE PlayerWeaponInventory (
    player_weapon_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    weapon_id INT NOT NULL,
    level INT DEFAULT 0,
    count INT DEFAULT 0,
    attack_power INT,
    critical_chance FLOAT,
    critical_damage FLOAT,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (weapon_id) REFERENCES Weapon(weapon_id)
);

CREATE TABLE PlayerSkills (
    player_skill_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    skill_id INT NOT NULL,
    level INT DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (skill_id) REFERENCES Skills(id)
);

CREATE TABLE MissionProgress (
    player_id INT PRIMARY KEY,
    level_progress INT DEFAULT 0,
    combat_power_progress INT DEFAULT 0,
    awakening_progress INT DEFAULT 0,
    online_time_progress INT DEFAULT 0,
    weapon_level_sum_progress INT DEFAULT 0,
    last_online_time_check DATETIME DEFAULT CURRENT_TIMESTAMP,
    total_online_time INT DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE AttendanceCheck (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    check_date DATE NOT NULL,
    day_count INT NOT NULL,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    UNIQUE KEY (player_id, check_date)
);

-- 외래 키 체크 활성화
SET FOREIGN_KEY_CHECKS = 1;

-- combat_power 업데이트를 위한 프로시저
DELIMITER //
CREATE PROCEDURE update_player_combat_power(IN player_id INT)
BEGIN
    SELECT combat_power FROM PlayerAttributes WHERE player_id = player_id;
END //
DELIMITER ;