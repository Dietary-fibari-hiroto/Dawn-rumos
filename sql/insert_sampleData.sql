-- プラットフォーム
INSERT INTO platforms (name, description) VALUES
('ESP32', 'Wi-Fi/Bluetooth対応のマイクロコントローラ'),
('TP-Link', '操作可能なコンセントplug');

-- 部屋
INSERT INTO rooms (name, description) VALUES
('リビング', '家族が集まるリビングルーム'),
('キッチン', '調理用のキッチン'),
('ベッドルーム', '寝室'),
('ガレージ', '作業用ガレージ');

-- デバイス
INSERT INTO devices (name, ip_v4, platform_id, room_id) VALUES
('TP-1', '192.168.10.13', 2, 1),
('TP-2', '192.168.10.8', 2, 2),
('TP-3', '192.168.10.10', 2, 1),
('Lumina_Sangria_1', NULL, 1, 4),
('Lumina_Sangria_2', NULL, 1, 3);
