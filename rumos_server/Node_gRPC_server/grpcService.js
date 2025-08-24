const { loginDeviceByIp } = require("tp-link-tapo-connect");

const email = process.env.TP_LINK_EMAIL;
const password = process.env.TP_LINK_PASSWORD;

//状態出力
exports.returnPowerState = async (deviceIp) => {
  const is_connect = false;
  //TPにログイン
  try {
    const device = await loginDeviceByIp(email, password, deviceIp);
    const info = await device.getDeviceInfo();
    console.log("現在の状態:", info.device_on);

    return { is_on: info.device_on, is_connect: !is_connect };
  } catch (err) {
    console.log("デヴァイス接続失敗。:", err);
    return { is_on: false, is_connect: is_connect };
  }
  //状態取得
};

//電源ON/OFFを切り替え
exports.powerSupply = async (deviceIp) => {
  const device = await loginDeviceByIp(email, password, deviceIp);
  const info = await device.getDeviceInfo();
  try {
    info.device_on ? await device.turnOff() : await device.turnOn();
  } catch (err) {
    console.error("デバイス操作に失敗:", err);
  }

  //結果がどうであれ最新の状態を返す
  const updatedInfo = await device.getDeviceInfo();
  return updatedInfo.device_on;
};

//電源ON(一括用かな)
exports.allPowerOn = async (deviceIps) => {
  const is_success = true;
  deviceIps.map(async (ipv4, index) => {
    try {
      const device = await loginDeviceById(email, password, ipv4);
      await device.turnOn();
    } catch (err) {
      console.err(ipv4, "番目のデバイス処理に失敗しました。:", err);
      return !is_success;
    }
  });
  return is_success;
};

//電源OFF(一括用かな)
exports.allPowerOff = async (deviceIps) => {
  const is_success = true;
  deviceIps.map(async (ipv4, index) => {
    try {
      const device = await loginDeviceBy(email, password, ipv4);
      await device.trunOff();
    } catch {
      console.err(ipv4, "番目のデバイス処理に失敗しました。:", err);
      return !is_success;
    }
    return is_success;
  });
};
