INSERT INTO Rewards (ID, Type, Description, Requirement, Reward) VALUES
(1, 'level', '캐릭터 레벨업 보상', 1, 1000),
(2, 'combat_power', '전투력 보상', 500, 12),
(3, 'awakening', '각성 보상', 1, 10000),
(4, 'online_time', '접속시간 보상', 1, 500),
(5, 'weapon_level_sum', '오브 레벨업 보상', 5, 25);

INSERT INTO Skills (id, name, description, damage_percentage, image, cooldown) VALUES
(1, '아이스 애로우', '얼음 화살로 1명의 적에게 X%의 데미지로 공격합니다.', 100, 'Skills_1', 6),
(2, '아이시클 애로우', '강력한 얼음 화살로 직선 상의 3명의 적을 꿰뚫어 X%의 데미지로 공격합니다.', 150, 'Skills_2', 6),
(3, '아이스 니들', '얼음 탄환을 발사해 주변 3병의 적에게 X%의 데미지로 공격합니다.', 200, 'Skills_3', 10),
(4, '아이스 샤드', '더 많은 얼음 탄환을 발사해 주변 7명의 적에게 X%의 데미지로 공격합니다.', 200, 'Skills_4', 10),
(5, '아이스 오브', '오브의 기운을 응축하여 주변 적 5명에게 X%의 데미지로 공격합니다.', 400, 'Skills_5', 12),
(6, '아이스 월', '얼음 방패를 소환해 6초간 전투력의 X%만큼 데미지를 막습니다.', 100, 'Skills_6', 14),
(7, '아이스 해머', '거대한 빙하를 소환해 1명의 적에게 X%의 데미지를 주며 내려찍습니다.', 1000, 'Skills_7', 16),
(8, '아이스 토네이도', '얼음 폭풍을 소환해 주변 모든 적에게 X%의 데미지로 공격합니다.', 600, 'Skills_8', 20),
(9, '글라시아', '오브의 기운을 폭발시켜 주변 모든 적에게 X%의 데미지를 줍니다.', 2000, 'Skills_9', 24),
(10, '아이스 랜스', '전방으로 얼음 용을 발사해 모든 적에게 X%의 데미지를 줍니다.', 3000, 'Skills_10', 28),
(11, '아이스 레인', '주변 지역에 랜덤하게 얼음 운석을 떨어뜨려 모든 적에게 각각 X%의 피해를 입힌다.', 4000, 'Skills_11', 24),
(12, '아이스 캐논', '오브의 기운을 응축하여 전방으로 발사해 모든 적에게 X%의 피해를 입힌다.', 6000, 'Skills_12', 24),
(13, '블리자드', '거대한 얼음 폭풍을 발생시켜 모든 적에게 X%의 피해를 입힌다.', 8000, 'Skills_13', 30),
(14, '쥬데카', '1명의 적에게 저주를 내려 행동을 멈추고 X%의 피해를 입힌다.', 10000, 'Skills_14', 30),
(15, '아이스 에이지', '전방의 모든 적을 얼리는 폭풍을 소환 해 X%의 피해를 입힌다.', 12000, 'Skills_15', 36),
(16, '나스트론드', '얼음 영역을 생성 해 모든 적을 얼리고 X%의 피해를 입힌다.', 16000, 'Skills_16', 40);

INSERT INTO Weapon (weapon_id, weapon_grade, attack_power, crit_rate, crit_damage, weapon_exp, prefab_name) VALUES
(1, 1, 100, 0.10, 1.50, 0, 'Orb1'),
(2, 2, 200, 0.12, 1.76, 5, 'Orb1'),
(3, 3, 300, 0.14, 2.04, 25, 'Orb1'),
(4, 4, 400, 0.16, 2.34, 125, 'Orb1'),
(5, 5, 500, 0.18, 2.66, 625, 'Orb1'),
(6, 6, 600, 0.20, 3.00, 3125, 'Orb1'),
(7, 7, 700, 0.22, 3.36, 15625, 'Orb2'),
(8, 8, 800, 0.24, 3.74, 78125, 'Orb2'),
(9, 9, 900, 0.26, 4.14, 390625, 'Orb2'),
(10, 10, 1000, 0.28, 4.56, 1953125, 'Orb2'),
(11, 11, 2000, 0.30, 5.00, 9765625, 'Orb2'),
(12, 12, 2500, 0.32, 5.46, 48828125, 'Orb2'),
(13, 13, 3000, 0.34, 5.94, 244140625, 'Orb2'),
(14, 14, 3500, 0.36, 6.44, 1220703125, 'Orb3'),
(15, 15, 4000, 0.38, 6.96, 6103515625, 'Orb3'),
(16, 16, 5000, 0.40, 7.50, 30517578125, 'Orb3'),
(17, 17, 6000, 0.42, 8.06, 152587890625, 'Orb3'),
(18, 18, 7000, 0.44, 8.64, 762939453125, 'Orb3'),
(19, 19, 8000, 0.46, 9.24, 3814697265625, 'Orb4'),
(20, 20, 9000, 0.48, 9.86, 19073486328125, 'Orb4'),
(21, 21, 10000, 0.50, 10.50, 95367431640625, 'Orb4'),
(22, 22, 15000, 0.52, 11.16, 476837158203125, 'Orb4'),
(23, 23, 20000, 0.54, 11.84, 2384185791015620, 'Orb4'),
(24, 24, 25000, 0.56, 12.54, 11920928955078100, 'Orb5');

INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (1, '1-1', 'Normal', 'One Eyed Bat Red', 100, 10, 50, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (2, '1-2', 'Normal', 'One Eyed Bat Red', 110, 12, 55, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (3, '1-3', 'Normal', 'One Eyed Bat Red', 120, 14, 60, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (4, '1-4', 'Normal', 'One Eyed Bat Red', 130, 16, 65, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (5, '1-5', 'Normal', 'One Eyed Bat Red', 140, 18, 70, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (6, '1-6', 'Normal', 'One Eyed Bat Red', 150, 20, 75, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (7, '1-7', 'Normal', 'One Eyed Bat Red', 160, 22, 80, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (8, '1-8', 'Normal', 'One Eyed Bat Red', 170, 24, 85, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (9, '1-9', 'Normal', 'One Eyed Bat Red', 180, 26, 90, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (10, '1-10', 'Normal', 'One Eyed Bat Red', 190, 28, 95, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (11, '1-11', 'Normal', 'One Eyed Bat Red', 200, 30, 100, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (12, '1-12', 'Normal', 'One Eyed Bat Red', 210, 32, 105, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (13, '1-13', 'Normal', 'One Eyed Bat Red', 220, 34, 110, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (14, '1-14', 'Normal', 'One Eyed Bat Red', 230, 36, 115, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (15, '1-15', 'Boss', 'One Eyed Bat Black', 500, 50, 500, 1, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (16, '2-1', 'Normal', 'Giant Bee Blue', 240, 38, 120, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (17, '2-2', 'Normal', 'Giant Bee Blue', 250, 40, 125, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (18, '2-3', 'Normal', 'Giant Bee Blue', 260, 42, 130, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (19, '2-4', 'Normal', 'Giant Bee Blue', 270, 44, 135, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (20, '2-5', 'Normal', 'Giant Bee Blue', 280, 46, 140, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (21, '2-6', 'Normal', 'Giant Bee Blue', 290, 48, 145, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (22, '2-7', 'Normal', 'Giant Bee Blue', 300, 50, 150, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (23, '2-8', 'Normal', 'Giant Bee Blue', 310, 52, 155, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (24, '2-9', 'Normal', 'Giant Bee Blue', 320, 54, 160, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (25, '2-10', 'Normal', 'Giant Bee Blue', 330, 56, 165, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (26, '2-11', 'Normal', 'Giant Bee Blue', 340, 58, 170, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (27, '2-12', 'Normal', 'Giant Bee Blue', 350, 60, 175, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (28, '2-13', 'Normal', 'Giant Bee Blue', 360, 62, 180, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (29, '2-14', 'Normal', 'Giant Bee Blue', 370, 64, 185, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (30, '2-15', 'Boss', 'Giant Bee Yellow', 1000, 100, 1000, 2, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (31, '3-1', 'Normal', 'Spiderling Venom Green', 380, 66, 190, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (32, '3-2', 'Normal', 'Spiderling Venom Green', 390, 68, 195, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (33, '3-3', 'Normal', 'Spiderling Venom Green', 400, 70, 200, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (34, '3-4', 'Normal', 'Spiderling Venom Green', 410, 72, 205, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (35, '3-5', 'Normal', 'Spiderling Venom Green', 420, 74, 210, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (36, '3-6', 'Normal', 'Spiderling Venom Green', 430, 76, 215, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (37, '3-7', 'Normal', 'Spiderling Venom Green', 440, 78, 220, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (38, '3-8', 'Normal', 'Spiderling Venom Green', 450, 80, 225, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (39, '3-9', 'Normal', 'Spiderling Venom Green', 460, 82, 230, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (40, '3-10', 'Normal', 'Spiderling Venom Green', 470, 84, 235, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (41, '3-11', 'Normal', 'Spiderling Venom Green', 480, 86, 240, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (42, '3-12', 'Normal', 'Spiderling Venom Green', 490, 88, 245, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (43, '3-13', 'Normal', 'Spiderling Venom Green', 500, 90, 250, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (44, '3-14', 'Normal', 'Spiderling Venom Green', 510, 92, 255, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (45, '3-15', 'Boss', 'Spiderling Venom Red', 1500, 150, 1500, 3, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (46, '4-1', 'Normal', 'Wolf Brown', 520, 94, 260, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (47, '4-2', 'Normal', 'Wolf Brown', 530, 96, 265, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (48, '4-3', 'Normal', 'Wolf Brown', 540, 98, 270, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (49, '4-4', 'Normal', 'Wolf Brown', 550, 100, 275, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (50, '4-5', 'Normal', 'Wolf Brown', 560, 102, 280, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (51, '4-6', 'Normal', 'Wolf Brown', 570, 104, 285, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (52, '4-7', 'Normal', 'Wolf Brown', 580, 106, 290, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (53, '4-8', 'Normal', 'Wolf Brown', 590, 108, 295, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (54, '4-9', 'Normal', 'Wolf Brown', 600, 110, 300, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (55, '4-10', 'Normal', 'Wolf Brown', 610, 112, 305, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (56, '4-11', 'Normal', 'Wolf Brown', 620, 114, 310, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (57, '4-12', 'Normal', 'Wolf Brown', 630, 116, 315, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (58, '4-13', 'Normal', 'Wolf Brown', 640, 118, 320, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (59, '4-14', 'Normal', 'Wolf Brown', 650, 120, 325, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (60, '4-15', 'Boss', 'Wolf White', 2000, 200, 2000, 4, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (61, '5-1', 'Normal', 'Magma Orange', 660, 122, 330, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (62, '5-2', 'Normal', 'Magma Orange', 670, 124, 335, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (63, '5-3', 'Normal', 'Magma Orange', 680, 126, 340, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (64, '5-4', 'Normal', 'Magma Orange', 690, 128, 345, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (65, '5-5', 'Normal', 'Magma Orange', 700, 130, 350, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (66, '5-6', 'Normal', 'Magma Orange', 710, 132, 355, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (67, '5-7', 'Normal', 'Magma Orange', 720, 134, 360, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (68, '5-8', 'Normal', 'Magma Orange', 730, 136, 365, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (69, '5-9', 'Normal', 'Magma Orange', 740, 138, 370, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (70, '5-10', 'Normal', 'Magma Orange', 750, 140, 375, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (71, '5-11', 'Normal', 'Magma Orange', 760, 142, 380, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (72, '5-12', 'Normal', 'Magma Orange', 770, 144, 385, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (73, '5-13', 'Normal', 'Magma Orange', 780, 146, 390, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (74, '5-14', 'Normal', 'Magma Orange', 790, 148, 395, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (75, '5-15', 'Boss', 'Magma Purple', 2500, 250, 2500, 5, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (76, '6-1', 'Normal', 'Golem Green', 800, 150, 400, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (77, '6-2', 'Normal', 'Golem Green', 810, 152, 405, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (78, '6-3', 'Normal', 'Golem Green', 820, 154, 410, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (79, '6-4', 'Normal', 'Golem Green', 830, 156, 415, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (80, '6-5', 'Normal', 'Golem Green', 840, 158, 420, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (81, '6-6', 'Normal', 'Golem Green', 850, 160, 425, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (82, '6-7', 'Normal', 'Golem Green', 860, 162, 430, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (83, '6-8', 'Normal', 'Golem Green', 870, 164, 435, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (84, '6-9', 'Normal', 'Golem Green', 880, 166, 440, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (85, '6-10', 'Normal', 'Golem Green', 890, 168, 445, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (86, '6-11', 'Normal', 'Golem Green', 900, 170, 450, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (87, '6-12', 'Normal', 'Golem Green', 910, 172, 455, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (88, '6-13', 'Normal', 'Golem Green', 920, 174, 460, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (89, '6-14', 'Normal', 'Golem Green', 930, 176, 465, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (90, '6-15', 'Boss', 'Golem Blue', 3000, 300, 3000, 6, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (91, '7-1', 'Normal', 'King Cobra Red', 940, 178, 470, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (92, '7-2', 'Normal', 'King Cobra Red', 950, 180, 475, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (93, '7-3', 'Normal', 'King Cobra Red', 960, 182, 480, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (94, '7-4', 'Normal', 'King Cobra Red', 970, 184, 485, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (95, '7-5', 'Normal', 'King Cobra Red', 980, 186, 490, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (96, '7-6', 'Normal', 'King Cobra Red', 990, 188, 495, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (97, '7-7', 'Normal', 'King Cobra Red', 1000, 190, 500, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (98, '7-8', 'Normal', 'King Cobra Red', 1010, 192, 505, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (99, '7-9', 'Normal', 'King Cobra Red', 1020, 194, 510, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (100, '7-10', 'Normal', 'King Cobra Red', 1030, 196, 515, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (101, '7-11', 'Normal', 'King Cobra Red', 1040, 198, 520, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (102, '7-12', 'Normal', 'King Cobra Red', 1050, 200, 525, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (103, '7-13', 'Normal', 'King Cobra Red', 1060, 202, 530, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (104, '7-14', 'Normal', 'King Cobra Red', 1070, 204, 535, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (105, '7-15', 'Boss', 'King Cobra Black', 3500, 350, 3500, 7, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (106, '8-1', 'Normal', 'Wererat Soldier Green', 1080, 206, 540, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (107, '8-2', 'Normal', 'Wererat Soldier Green', 1090, 208, 545, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (108, '8-3', 'Normal', 'Wererat Soldier Green', 1100, 210, 550, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (109, '8-4', 'Normal', 'Wererat Soldier Green', 1110, 212, 555, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (110, '8-5', 'Normal', 'Wererat Soldier Green', 1120, 214, 560, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (111, '8-6', 'Normal', 'Wererat Soldier Green', 1130, 216, 565, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (112, '8-7', 'Normal', 'Wererat Soldier Green', 1140, 218, 570, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (113, '8-8', 'Normal', 'Wererat Soldier Green', 1150, 220, 575, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (114, '8-9', 'Normal', 'Wererat Soldier Green', 1160, 222, 580, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (115, '8-10', 'Normal', 'Wererat Soldier Green', 1170, 224, 585, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (116, '8-11', 'Normal', 'Wererat Soldier Green', 1180, 226, 590, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (117, '8-12', 'Normal', 'Wererat Soldier Green', 1190, 228, 595, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (118, '8-13', 'Normal', 'Wererat Soldier Green', 1200, 230, 600, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (119, '8-14', 'Normal', 'Wererat Soldier Green', 1210, 232, 605, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (120, '8-15', 'Boss', 'Wererat Soldier Red', 4000, 400, 4000, 8, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (121, '9-1', 'Normal', 'Orc Slinger Green', 1220, 234, 610, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (122, '9-2', 'Normal', 'Orc Slinger Green', 1230, 236, 615, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (123, '9-3', 'Normal', 'Orc Slinger Green', 1240, 238, 620, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (124, '9-4', 'Normal', 'Orc Slinger Green', 1250, 240, 625, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (125, '9-5', 'Normal', 'Orc Slinger Green', 1260, 242, 630, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (126, '9-6', 'Normal', 'Orc Slinger Green', 1270, 244, 635, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (127, '9-7', 'Normal', 'Orc Slinger Green', 1280, 246, 640, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (128, '9-8', 'Normal', 'Orc Slinger Green', 1290, 248, 645, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (129, '9-9', 'Normal', 'Orc Slinger Green', 1300, 250, 650, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (130, '9-10', 'Normal', 'Orc Slinger Green', 1310, 252, 655, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (131, '9-11', 'Normal', 'Orc Slinger Green', 1320, 254, 660, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (132, '9-12', 'Normal', 'Orc Slinger Green', 1330, 256, 665, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (133, '9-13', 'Normal', 'Orc Slinger Green', 1340, 258, 670, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (134, '9-14', 'Normal', 'Orc Slinger Green', 1350, 260, 675, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (135, '9-15', 'Boss', 'Orc Slinger Purple', 4500, 450, 4500, 9, 1.0);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (136, '10-1', 'Normal', 'Wraith Swordsman Purple', 1360, 262, 680, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (137, '10-2', 'Normal', 'Wraith Swordsman Purple', 1370, 264, 685, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (138, '10-3', 'Normal', 'Wraith Swordsman Purple', 1380, 266, 690, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (139, '10-4', 'Normal', 'Wraith Swordsman Purple', 1390, 268, 695, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (140, '10-5', 'Normal', 'Wraith Swordsman Purple', 1400, 270, 700, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (141, '10-6', 'Normal', 'Wraith Swordsman Purple', 1410, 272, 705, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (142, '10-7', 'Normal', 'Wraith Swordsman Purple', 1420, 274, 710, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (143, '10-8', 'Normal', 'Wraith Swordsman Purple', 1430, 276, 715, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (144, '10-9', 'Normal', 'Wraith Swordsman Purple', 1440, 278, 720, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (145, '10-10', 'Normal', 'Wraith Swordsman Purple', 1450, 280, 725, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (146, '10-11', 'Normal', 'Wraith Swordsman Purple', 1460, 282, 730, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (147, '10-12', 'Normal', 'Wraith Swordsman Purple', 1470, 284, 735, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (148, '10-13', 'Normal', 'Wraith Swordsman Purple', 1480, 286, 740, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (149, '10-14', 'Normal', 'Wraith Swordsman Purple', 1490, 288, 745, 0, 0.05);
INSERT INTO Monsters (id, Stage, Type, Name, Health, Attack, DropMoney, DropElementStone, DropElementStoneChance) VALUES (150, '10-15', 'Boss', 'Wraith Swordsman Red', 5000, 500, 5000, 10, 1.0);

