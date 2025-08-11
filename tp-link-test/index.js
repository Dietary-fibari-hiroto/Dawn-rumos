const { cloudLogin, loginDeviceByIp } = require("tp-link-tapo-connect");

(async () => {
  const email = "hirorig12@gmail.com";
  const password = "ulorufu1357";
  const deviceIp = "192.168.10.13";

  const device = await loginDeviceByIp(email, password, deviceIp);

  // 電源ON
  await device.turnOn();

  // デバイス情報取得
  const info = await device.getDeviceInfo();
  console.log("Device Info:", info);

  // 電源OFF
  await device.turnOff();
})().catch(console.error);
