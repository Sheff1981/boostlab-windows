using System.Net.Http.Json;

namespace BoostLab.Client.Networking;

public sealed class ControlPlaneClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IReadOnlyList<GatewayNode>> GetHealthyNodesAsync(
        Uri baseUri,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(baseUri);

        if (!string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Control-plane URL must use HTTPS.", nameof(baseUri));
        }

        var endpoint = new Uri(baseUri, "/v1/nodes");
        var nodes = await _httpClient.GetFromJsonAsync<GatewayNode[]>(
            endpoint,
            cancellationToken) ?? [];

        return nodes
            .Where(static node =>
                node.Healthy &&
                !string.IsNullOrWhiteSpace(node.Id) &&
                !string.IsNullOrWhiteSpace(node.Region) &&
                !string.IsNullOrWhiteSpace(node.Host) &&
                node.UdpPort is >= 1 and <= 65535)
            .ToArray();
    }
}
