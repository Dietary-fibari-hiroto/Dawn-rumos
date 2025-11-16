-- プラットフォーム
INSERT INTO platforms (name, description) VALUES
('TP-Link', '操作可能なコンセントplug'),
('Astrolume_Sangria','空いたワインボトルに装着するタイプのライト'),
('Astrolume_Startrail','線系の照明,間接照明');

-- 部屋
INSERT INTO rooms (name, description) VALUES
('リビング', '家族が集まるリビングルーム'),
('キッチン', '調理用のキッチン'),
('ベッドルーム', '寝室'),
('ガレージ', '作業用ガレージ');

-- デバイス
INSERT INTO devices (name, ip_v4, platform_id, room_id) VALUES
('TP-1', '192.168.10.4', 1, 1),
('TP-2', '192.168.10.2', 1, 2),
('TP-3', '192.168.10.3', 1, 1),
('Astrolume_Sangria_1', NULL, 2, 4),
('Astrolume_Sangria_2', NULL, 2, 3),
('Astrolume_Sangria_3', NULL, 2, 3),
('Astrolume_Startrail_1', NULL, 3, 1);


INSERT INTO presets(name,img_url) VALUES
('Test_preset','https://dawn-waiting.com/static/media/IMG_7038.fb29bdc7d5c0f55d07eb.jpg');


INSERT INTO modes(name) VALUES
('normal'),
('fadein'),
('whitegradient');