namespace BoostLab.Client.Networking;

public static class RouteScorer
{
    public static double Score(RouteMetrics metrics)
    {
        ArgumentNullException.ThrowIfNull(metrics);

        if (metrics.MedianRttMs is null)
        {
            return double.PositiveInfinity;
        }

        var jitter = metrics.JitterMs ?? 50.0;

        return metrics.MedianRttMs.Value +
               (jitter * 2.0) +
               (metrics.PacketLossPercent * 12.0);
    }
}
