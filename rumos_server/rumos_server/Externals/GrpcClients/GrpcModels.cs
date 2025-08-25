namespace rumos_server.Externals.GrpcClients
{
    public record RSetDeviceRequest(string ip);
    public record RSetPowerReply(bool Success,string Message,bool IsOn);
    public record RGetStatusReply(bool IsConnect,bool IsOn);
    public record RSetAllDeviceRequest(string json);
    public record RSetAllDeviceReply(bool Success);
}
