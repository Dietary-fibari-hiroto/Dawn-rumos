const { powerSupply, spPowerSupply } = require("./onnoff");

exports.svcImpl = {
  setPower: async (call, callback) => {
    const { device_id, ip, account_email, account_password } = call.request;
    console.log("callState:", call.request);
    try {
      //TODO:ここでtpを制御
      const is_on = await spPowerSupply(account_email, account_password, ip);
      console.log(
        `[SetPower] id=${device_id} ip=${ip} user=${account_email} is_on=${is_on}`
      );
      //成功レスポンス
      callback(null, {
        success: true,
        message: "PowerSupply処理が正常に終わったよ",
        is_on: is_on,
      });
    } catch (e) {
      callback(null, { success: false, message: e.message ?? "error" });
    }
  },

  GetStatus: async (call, callback) => {
    const { device_id, ip, account_email, account_password } = call.request;
    try {
      const is_on = false;
      callback(null, { success: true, message: "OK", is_on });
    } catch (e) {
      console.error(e);
      callback(null, {
        success: false,
        message: e.message ?? "error",
        is_on: false,
      });
    }
  },
};
