const { loginDeviceByIp } = require("tp-link-tapo-connect");

exports.powerSupply = async () => {
  const email = process.env.TP_LINK_EMAIL;
  const password = process.env.TP_LINK_PASSWORD;
  const deviceIp = process.env.TP_LINK_IP;

  const device = await loginDeviceByIp(email, password, deviceIp);

  const info = await device.getDeviceInfo();

  console.log("現在の状態:", info.device_on ? "ON" : "OFF");

  info.device_on ? await device.turnOff() : await device.turnOn();

  console.log("On/Off切り替え");
};
