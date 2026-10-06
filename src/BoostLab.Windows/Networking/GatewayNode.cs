using System.Text.Json.Serialization;

namespace BoostLab.Client.Networking;

public sealed record GatewayNode(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("host")] string Host,
    [property: JsonPropertyName("udp_port")] int UdpPort,
    [property: JsonPropertyName("wireguard_public_key")] string? WireGuardPublicKey,
    [property: JsonPropertyName("wireguard_port")] int? WireGuardPort,
    [property: JsonPropertyName("healthy")] bool Healthy);
