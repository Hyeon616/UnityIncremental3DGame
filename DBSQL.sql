-- 데이터베이스 사용
USE wjhdb;

-- 외래 키 체크 비활성화
SET FOREIGN_KEY_CHECKS = 0;

-- 기존 테이블 삭제 (있다면)
DROP TABLE IF EXISTS Players, Monsters, Rewards, Skills, Weapon, Guilds, PlayerGuilds, 
                     PlayerAttributes, mails, Friends, FriendRequests, 
                     PlayerWeaponInventory, PlayerSkills, MissionProgress;

-- Players 테이블 생성
CREATE TABLE Players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL
);


-- Monsters 테이블 생성
CREATE TABLE Monsters (
    id INT PRIMARY KEY,
    Stage VARCHAR(10),
    Type VARCHAR(20),
    Name VARCHAR(50),
    Health INT,
    Attack INT,
    DropMoney INT,
    DropElementStone INT,
    DropElementStoneChance DECIMAL(5, 2)
);

-- Rewards 테이블 생성
CREATE TABLE Rewards (
    ID INT PRIMARY KEY,
    Type VARCHAR(50),
    Description VARCHAR(255),
    Requirement INT,
    Reward INT
);

-- Skills 테이블 생성
CREATE TABLE Skills (
    id INT PRIMARY KEY,
    name VARCHAR(255),
    description TEXT,
    damage_percentage INT,
    image VARCHAR(255),
    cooldown INT
);

-- Weapon 테이블 생성
CREATE TABLE Weapon (
    weapon_id INT PRIMARY KEY,
    weapon_grade INT,
    attack_power INT,
    crit_rate DECIMAL(5, 2),
    crit_damage DECIMAL(5, 2),
    weapon_exp BIGINT,
    prefab_name VARCHAR(255)
);

-- Guilds 테이블 생성
CREATE TABLE Guilds (
    guild_id INT AUTO_INCREMENT PRIMARY KEY,
    guild_name VARCHAR(255) UNIQUE NOT NULL,
    guild_leader INT NOT NULL
);

-- PlayerGuilds 연결 테이블 생성
CREATE TABLE PlayerGuilds (
    player_id INT NOT NULL,
    guild_id INT NOT NULL,
    PRIMARY KEY (player_id, guild_id)
);

-- PlayerAttributes 테이블 생성
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
    combat_power INT GENERATED ALWAYS AS (
        CAST((attack_power + critical_chance + max_health + critical_damage) * 
        (IF(awakening = 0, 1, awakening * 10)) * 
        (level * 0.1) AS SIGNED)
    ) STORED,
    rank INT
);

-- mails 테이블 생성
CREATE TABLE mails (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    type ENUM('attendance', 'event', 'offline_reward', 'mission'),
    reward JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    expires_at DATETIME DEFAULT (CURRENT_TIMESTAMP + INTERVAL 3 DAY),
    is_read BOOLEAN DEFAULT FALSE
);

-- Friends 테이블 생성
CREATE TABLE Friends (
    player_id INT NOT NULL,
    friend_id INT NOT NULL,
    PRIMARY KEY (player_id, friend_id)
);

-- FriendRequests 테이블 생성
CREATE TABLE FriendRequests (
    request_id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
    status ENUM('pending', 'accepted', 'rejected', 'cancelled') DEFAULT 'pending',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
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
    critical_damage FLOAT
);

-- PlayerSkills 테이블 생성
CREATE TABLE PlayerSkills (
    player_skill_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    skill_id INT NOT NULL,
    level INT DEFAULT 0
);

-- 미션 진행 상태를 저장할 테이블 생성
CREATE TABLE MissionProgress (
    player_id INT PRIMARY KEY,
    level_progress INT DEFAULT 0,
    combat_power_progress INT DEFAULT 0,
    awakening_progress INT DEFAULT 0,
    online_time_progress INT DEFAULT 0,
    weapon_level_sum_progress INT DEFAULT 0,
    last_online_time_check DATETIME DEFAULT CURRENT_TIMESTAMP,
    total_online_time INT DEFAULT 0
);

CREATE TABLE AttendanceCheck (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    check_date DATE NOT NULL,
    day_count INT NOT NULL,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    UNIQUE KEY (player_id, check_date)
);


-- 외래 키 제약 조건 추가
ALTER TABLE Guilds
ADD FOREIGN KEY (guild_leader) REFERENCES Players(player_id) ON DELETE CASCADE;

ALTER TABLE PlayerGuilds
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
ADD FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE CASCADE;

ALTER TABLE PlayerAttributes
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
ADD FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE SET NULL,
ADD FOREIGN KEY (equipped_skill1_id) REFERENCES Skills(id),
ADD FOREIGN KEY (equipped_skill2_id) REFERENCES Skills(id),
ADD FOREIGN KEY (equipped_skill3_id) REFERENCES Skills(id);

ALTER TABLE mails
ADD FOREIGN KEY (user_id) REFERENCES Players(player_id) ON DELETE CASCADE;

ALTER TABLE Friends
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
ADD FOREIGN KEY (friend_id) REFERENCES Players(player_id) ON DELETE CASCADE;

ALTER TABLE FriendRequests
ADD FOREIGN KEY (sender_id) REFERENCES Players(player_id) ON DELETE CASCADE,
ADD FOREIGN KEY (receiver_id) REFERENCES Players(player_id) ON DELETE CASCADE;

ALTER TABLE PlayerWeaponInventory
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE;

ALTER TABLE PlayerSkills
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
ADD FOREIGN KEY (skill_id) REFERENCES Skills(id);

ALTER TABLE MissionProgress
ADD FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE;

-- 외래 키 체크 활성화
SET FOREIGN_KEY_CHECKS = 1;

-- rank 업데이트를 위한 저장 프로시저
DELIMITER //
CREATE PROCEDURE update_player_ranks()
BEGIN
    SET @rank = 0;
    UPDATE PlayerAttributes
    SET rank = (@rank := @rank + 1)
    ORDER BY combat_power DESC;
END //
DELIMITER ;

-- 트리거 생성 (combat_power가 변경될 때마다 순위 업데이트)
CREATE TRIGGER update_ranks_after_update
AFTER UPDATE ON PlayerAttributes
FOR EACH ROW
BEGIN
    IF NEW.combat_power <> OLD.combat_power THEN
        CALL update_player_ranks();
    END IF;
END;

-- 트리거 생성 (새로운 플레이어가 추가될 때마다 순위 업데이트)
CREATE TRIGGER update_ranks_after_insert
AFTER INSERT ON PlayerAttributes
FOR EACH ROW
BEGIN
    CALL update_player_ranks();
END;




