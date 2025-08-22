/*プラットフォームの種類を管理*/
CREATE TABLE platforms(
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT NOT NULL
)ENGINE=InnoDB;

/*部屋割り*/
CREATE TABLE rooms(
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT NOT NULL
)ENGINE=InnoDB;

/*デバイスの情報を管理*/
CREATE TABLE devices(
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    ip_v4 VARCHAR(20) NULL,
    platform_id INT NOT NULL,
    room_id INT NOT NULL,
    FOREIGN KEY (platform_id) REFERENCES platforms(id),
    FOREIGN KEY (room_id) REFERENCES rooms(id)
)ENGINE=InnoDB;