-- プラットフォーム
INSERT INTO platforms (name, description) VALUES
('ESP32', 'Wi-Fi/Bluetooth対応のマイクロコントローラ'),
('Raspberry Pi', '小型のシングルボードコンピュータ'),
('Arduino Uno', '基本的なマイクロコントローラボード');

-- 部屋
INSERT INTO rooms (name, description) VALUES
('リビング', '家族が集まるリビングルーム'),
('キッチン', '調理用のキッチン'),
('ベッドルーム', '寝室'),
('ガレージ', '作業用ガレージ');

-- デバイス
INSERT INTO devices (name, ip_v4, platform_id, room_id) VALUES
('TP-1', '192.168.10.13', 1, 1),
('TP-2', '192.168.10.8', 1, 2),
('TP-3', '192.168.10.10', 2, 1),
('Arduino-DoorSensor', NULL, 3, 4),
('ESP32-HumiditySensor', '192.168.1.12', 1, 3);
