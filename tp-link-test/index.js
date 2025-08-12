require("dotenv").config(); // これで.envファイルを読み込む

const { cloudLogin, loginDeviceByIp } = require("tp-link-tapo-connect");

(async () => {
  const email = process.env.TP_LINK_EMAIL;
  const password = process.env.TP_LINK_PASSWORD;
  const deviceIp = process.env.TP_LINK_IP;

  const device = await loginDeviceByIp(email, password, deviceIp);

  // 電源ON
  await device.turnOn();

  // デバイス情報取得
  const info = await device.getDeviceInfo();
  console.log("Device Info:", info);

  // 電源OFF
  await device.turnOff();
})().catch(console.error);
