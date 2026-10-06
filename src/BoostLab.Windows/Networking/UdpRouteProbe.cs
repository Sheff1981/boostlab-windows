using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoostLab.Client.Networking;

public static class UdpRouteProbe
{
    public const int DefaultPort = 51821;
    public const int DefaultSamples = 8;
    public const int DefaultTimeoutMilliseconds = 700;
    private const string Magic = "BOOSTLAB/PROBE/1";

    public static async Task<RouteMetrics> MeasureAsync(
        string host,
        int port = DefaultPort,
        int samples = DefaultSamples,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentOutOfRangeException.ThrowIfLessThan(port, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);
        ArgumentOutOfRangeException.ThrowIfLessThan(samples, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(samples, 50);

        var addresses = await Dns.GetHostAddressesAsync(host.Trim(), cancellationToken);
        var address = addresses.FirstOrDefault(static candidate =>
            candidate.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6)
            ?? throw new InvalidOperationException("Gateway host did not resolve to an IP address.");

        using var socket = new UdpClient(address.AddressFamily);
        socket.Connect(new IPEndPoint(address, port));

        var payload = Encoding.UTF8.GetBytes(Magic);
        var results = new double?[samples];
        var perSampleTimeout = timeout ?? TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds);

        for (var index = 0; index < samples; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var started = Stopwatch.GetTimestamp();
            await socket.SendAsync(payload, cancellationToken);

            using var sampleCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            sampleCts.CancelAfter(perSampleTimeout);

            try
            {
                var response = await socket.ReceiveAsync(sampleCts.Token);
                var body = Encoding.UTF8.GetString(response.Buffer);

                if (body == Magic)
                {
                    results[index] = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                }
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                results[index] = null;
            }

            if (index != samples - 1)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(120), cancellationToken);
            }
        }

        return RouteMetricsCalculator.Calculate(results);
    }
}
