namespace BoostLab.Client.Networking;

public sealed record RouteMetrics(
    double? MedianRttMs,
    double? JitterMs,
    double PacketLossPercent,
    int Sent,
    int Received);
