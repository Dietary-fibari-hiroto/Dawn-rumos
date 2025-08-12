require("dotenv").config(); // これで.envファイルを読み込む
const grpc = require("@grpc/grpc-js");
const protoLoader = require("@grpc/proto-loader");
const { powerSupply } = require("./onnoff");

const packageDef = protoLoader.loadSync("../proto/myservice.proto", {});
const grpcObject = grpc.loadPackageDefinition(packageDef);
const myservice = grpcObject.myservice;

function getData(call, callback) {
  console.log("Node:リクエスト受信");
  const query = call.request.query;
  console.log("リクエストを受信:", query); // ここでログ出力

  if (query === "Yuzuki") {
    // ここで非同期実行するがawaitしないので即時に次に進む
    powerSupply().catch((err) => {
      console.error("powerSupply error:", err);
    });

    console.log("PowerSupply 実行指示 発行済み");
  }

  callback(null, { value1: `Hello, ${query}`, value2: 42 });
}

const server = new grpc.Server();
server.addService(myservice.MyService.service, { getData });
server.bindAsync(
  "0.0.0.0:50051",
  grpc.ServerCredentials.createInsecure(),
  () => {
    server.start();
    console.log("gRPC 起動...");
  }
);
