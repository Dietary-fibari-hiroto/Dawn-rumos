const path = require("path");
const grpc = require("@grpc/grpc-js");
const protoLoader = require("@grpc/proto-loader");
const grpcController = require("./grpcController");

//proto読み込み
const PROTO_PATH = path.join(
  __dirname,
  "..",
  "..",
  "rumos_proto",
  "device_control.proto"
);

const packageDef = protoLoader.loadSync(PROTO_PATH, {
  keedCase: true,
  longs: String,
  enums: String,
  defaults: true,
  oneofs: true,
});
const proto = grpc.loadPackageDefinition(packageDef).devicecontrol;

function main() {
  const server = new grpc.Server();
  server.addService(proto.DawnDeviceControl.service, grpcController);
  const addr = "0.0.0.0:50052";
  server.bindAsync(
    addr,
    grpc.ServerCredentials.createInsecure(),
    (err, port) => {
      if (err) throw err;
      console.log(`gRPC TLS server listening on ${addr}`);
      server.start();
    }
  );
}

main();
