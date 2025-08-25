const {
  returnPowerState,
  powerSupply,
  allPowerOn,
  allPowerOff,
} = require("./grpcService");

exports.grpcController = {
  //電源を切り替える処理
  SetPowerChange: async (call, callback) => {
    console.log("手st");
    const { ip } = call.request;
    console.log("callState:", call.request);

    try {
      const is_on = await powerSupply(ip);
      callback(null, {
        success: true,
        message: "PowerSupply処理成功。",
        is_on: is_on,
      });
    } catch (error) {
      callback(null, { success: false, message: error.message ?? "error" });
    }
  },
  //電源を一括ONに
  SetAllPowerOn: async (call, callback) => {
    //Jsonを受け取って回せるようにパース
    const { json } = call.request;
    const deviceIps = JSON.parse(json);
    console.log(deviceIps); //テスト用のコンソール表示

    //結果を取得してcallback
    const is_success = allPowerOn(deviceIps);
    callback(null, { success: is_success });
  },
  //電源を一括OFFに
  SetAllPowerOff: async (call, callback) => {
    const { json } = call.request;
    const deviceIps = JSON.parse(json);
    //結果を取得してcallback
    const is_success = allPowerOff(deviceIps);
    callback(null, { success: is_success });
  },
  //デバイスの状態取得
  GetDevicePower: async (call, callback) => {
    const { ip } = call.request;
    const { is_on, is_connect } = await returnPowerState(ip);
    callback(null, {
      is_connect: is_connect,
      is_on: is_on,
    });
  },
};
