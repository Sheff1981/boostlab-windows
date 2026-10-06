namespace BoostLab.Client.Networking;

public static class RouteMetricsCalculator
{
    public static RouteMetrics Calculate(IReadOnlyList<double?> samplesMs)
    {
        ArgumentNullException.ThrowIfNull(samplesMs);

        if (samplesMs.Count == 0)
        {
            return new RouteMetrics(
                MedianRttMs: null,
                JitterMs: null,
                PacketLossPercent: 100,
                Sent: 0,
                Received: 0);
        }

        var received = samplesMs
            .Where(static sample => sample.HasValue)
            .Select(static sample => sample!.Value)
            .Order()
            .ToArray();

        double? median = received.Length switch
        {
            0 => null,
            var count when count % 2 == 1 => received[count / 2],
            var count => (received[(count / 2) - 1] + received[count / 2]) / 2.0,
        };

        double? jitter = received.Length < 2
            ? null
            : received
                .Zip(received.Skip(1), static (left, right) => Math.Abs(right - left))
                .Average();

        var lost = samplesMs.Count - received.Length;
        var loss = (double)lost / samplesMs.Count * 100.0;

        return new RouteMetrics(
            MedianRttMs: median,
            JitterMs: jitter,
            PacketLossPercent: loss,
            Sent: samplesMs.Count,
            Received: received.Length);
    }
}
