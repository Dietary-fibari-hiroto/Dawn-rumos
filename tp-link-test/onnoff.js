const { loginDeviceByIp } = require("tp-link-tapo-connect");

exports.powerSupply = () => {
  (async () => {
    const email = "hirorig12@gmail.com";
    const password = "ulorufu1357";
    const deviceIp = "192.168.10.13";

    const device = await loginDeviceByIp(email, password, deviceIp);

    const info = await device.getDeviceInfo();

    console.log("現在の状態:", info.device_on ? "ON" : "OFF");

    info.device_on ? await device.turnOff() : await device.turnOn();

    console.log("On/Off切り替え");
  })();
};
