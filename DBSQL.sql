USE wjhdb;

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS Players, Monsters, Rewards, Skills, Weapon, Guilds, PlayerGuilds, 
                     PlayerAttributes, mails, Friends, FriendRequests, 
                     PlayerWeaponInventory, PlayerSkills, MissionProgress, AttendanceCheck;

CREATE TABLE Players (
    player_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    player_username VARCHAR(255) UNIQUE NOT NULL,
    player_password VARCHAR(255) NOT NULL,
    player_nickname VARCHAR(255) UNIQUE NOT NULL
);

CREATE INDEX idx_player_username ON Players(player_username);
CREATE INDEX idx_player_nickname ON Players(player_nickname);

CREATE TABLE Monsters (
    id INT UNSIGNED PRIMARY KEY,
    Stage VARCHAR(10),
    Type VARCHAR(20),
    Name VARCHAR(50),
    Health INT UNSIGNED,
    Attack INT UNSIGNED,
    DropMoney INT UNSIGNED,
    DropElementStone INT UNSIGNED,
    DropElementStoneChance DECIMAL(5, 2)
);

CREATE INDEX idx_monster_stage ON Monsters(Stage);
CREATE INDEX idx_monster_type ON Monsters(Type);

CREATE TABLE Rewards (
    ID INT UNSIGNED PRIMARY KEY,
    Type VARCHAR(50),
    Description VARCHAR(255),
    Requirement INT UNSIGNED,
    Reward INT UNSIGNED
);

CREATE TABLE Skills (
    id INT UNSIGNED PRIMARY KEY,
    name VARCHAR(255),
    description TEXT,
    damage_percentage INT UNSIGNED,
    image VARCHAR(255),
    cooldown INT UNSIGNED
);

CREATE INDEX idx_skill_name ON Skills(name);

CREATE TABLE Weapon (
    weapon_id INT UNSIGNED PRIMARY KEY,
    weapon_grade INT UNSIGNED,
    attack_power INT UNSIGNED,
    crit_rate DECIMAL(5, 2),
    crit_damage DECIMAL(5, 2),
    weapon_exp DECIMAL(65, 0),
    prefab_name VARCHAR(255)
);

CREATE INDEX idx_weapon_grade ON Weapon(weapon_grade);

CREATE TABLE Guilds (
    guild_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    guild_name VARCHAR(255) UNIQUE NOT NULL,
    guild_leader INT UNSIGNED NOT NULL,
    FOREIGN KEY (guild_leader) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE INDEX idx_guild_name ON Guilds(guild_name);

CREATE TABLE PlayerGuilds (
    player_id INT UNSIGNED NOT NULL,
    guild_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (player_id, guild_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE CASCADE
);

CREATE TABLE PlayerAttributes (
    player_id INT UNSIGNED PRIMARY KEY,
    element_stone INT UNSIGNED DEFAULT 0,
    skill_summon_tickets INT UNSIGNED DEFAULT 0,
    money INT UNSIGNED DEFAULT 0,
    attack_power INT UNSIGNED DEFAULT 10,
    max_health INT UNSIGNED DEFAULT 50,
    critical_chance FLOAT DEFAULT 0,
    critical_damage FLOAT DEFAULT 0,
    current_stage VARCHAR(10) DEFAULT '1-1',
    level INT UNSIGNED DEFAULT 1,
    awakening INT UNSIGNED DEFAULT 0,
    guild_id INT UNSIGNED DEFAULT NULL,
    equipped_skill1_id INT UNSIGNED DEFAULT NULL,
    equipped_skill2_id INT UNSIGNED DEFAULT NULL,
    equipped_skill3_id INT UNSIGNED DEFAULT NULL,
    combat_power INT UNSIGNED GENERATED ALWAYS AS (
        CAST((attack_power + critical_chance + max_health + critical_damage) * 
        (IF(awakening = 0, 1, awakening * 10)) * 
        (level * 0.1) AS UNSIGNED)
    ) STORED,
    rank INT UNSIGNED DEFAULT NULL,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (guild_id) REFERENCES Guilds(guild_id) ON DELETE SET NULL,
    FOREIGN KEY (equipped_skill1_id) REFERENCES Skills(id),
    FOREIGN KEY (equipped_skill2_id) REFERENCES Skills(id),
    FOREIGN KEY (equipped_skill3_id) REFERENCES Skills(id)
);

CREATE INDEX idx_player_current_stage ON PlayerAttributes(current_stage);
CREATE INDEX idx_player_combat_power ON PlayerAttributes(combat_power);
CREATE INDEX idx_player_rank ON PlayerAttributes(rank);

CREATE TABLE mails (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED,
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
    player_id INT UNSIGNED NOT NULL,
    friend_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (player_id, friend_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (friend_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE FriendRequests (
    request_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    sender_id INT UNSIGNED NOT NULL,
    receiver_id INT UNSIGNED NOT NULL,
    status ENUM('pending', 'accepted', 'rejected', 'cancelled') DEFAULT 'pending',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (sender_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (receiver_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE PlayerWeaponInventory (
    player_weapon_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    player_id INT UNSIGNED NOT NULL,
    weapon_id INT UNSIGNED NOT NULL,
    level INT UNSIGNED DEFAULT 0,
    count INT UNSIGNED DEFAULT 0,
    attack_power INT UNSIGNED,
    critical_chance FLOAT,
    critical_damage FLOAT,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (weapon_id) REFERENCES Weapon(weapon_id)
);

CREATE TABLE PlayerSkills (
    player_skill_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    player_id INT UNSIGNED NOT NULL,
    skill_id INT UNSIGNED NOT NULL,
    level INT UNSIGNED DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    FOREIGN KEY (skill_id) REFERENCES Skills(id)
);

CREATE TABLE MissionProgress (
    player_id INT UNSIGNED PRIMARY KEY,
    level_progress INT UNSIGNED DEFAULT 0,
    combat_power_progress INT UNSIGNED DEFAULT 0,
    awakening_progress INT UNSIGNED DEFAULT 0,
    online_time_progress INT UNSIGNED DEFAULT 0,
    weapon_level_sum_progress INT UNSIGNED DEFAULT 0,
    last_online_time_check DATETIME DEFAULT CURRENT_TIMESTAMP,
    total_online_time INT UNSIGNED DEFAULT 0,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE
);

CREATE TABLE AttendanceCheck (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    player_id INT UNSIGNED NOT NULL,
    check_date DATE NOT NULL,
    day_count INT UNSIGNED NOT NULL,
    FOREIGN KEY (player_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    UNIQUE KEY (player_id, check_date)
);

SET FOREIGN_KEY_CHECKS = 1;

DELIMITER //

CREATE PROCEDURE update_player_ranks()
BEGIN
    SET @rank = 0;
    UPDATE PlayerAttributes
    SET rank = (@rank := @rank + 1)
    ORDER BY combat_power DESC, player_id ASC;
END //

CREATE PROCEDURE add_new_player_rank(IN new_player_id INT UNSIGNED)
BEGIN
    SET @new_player_combat_power = (SELECT combat_power FROM PlayerAttributes WHERE player_id = new_player_id);
    
    SET @new_rank = (SELECT COUNT(*) + 1 FROM PlayerAttributes WHERE combat_power > @new_player_combat_power OR (combat_power = @new_player_combat_power AND player_id < new_player_id));
    
    UPDATE PlayerAttributes SET rank = @new_rank WHERE player_id = new_player_id;
    
    UPDATE PlayerAttributes 
    SET rank = rank + 1 
    WHERE (combat_power < @new_player_combat_power OR (combat_power = @new_player_combat_power AND player_id > new_player_id))
      AND player_id != new_player_id;
END //

DELIMITER ;

SET GLOBAL event_scheduler = ON;

CREATE EVENT periodic_rank_update
ON SCHEDULE EVERY 5 MINUTE
DO
    CALL update_player_ranks();