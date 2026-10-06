using BoostLab.Client.Networking;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;

namespace BoostLab.Client;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppWindow.Resize(new SizeInt32(960, 720));
    }

    private async void ProbeButton_Click(object sender, RoutedEventArgs e)
    {
        var host = GatewayHostBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(host))
        {
            ShowProbeError("Enter a gateway host first.");
            return;
        }

        ProbeButton.IsEnabled = false;
        ProbeStatus.IsOpen = true;
        ProbeStatus.Severity = InfoBarSeverity.Informational;
        ProbeStatus.Title = "Measuring";
        ProbeStatus.Message = "Sending route-quality probes…";

        try
        {
            var metrics = await UdpRouteProbe.MeasureAsync(host);

            PingValue.Text = metrics.MedianRttMs is double ping
                ? $"{ping:F0} ms"
                : "—";
            JitterValue.Text = metrics.JitterMs is double jitter
                ? $"{jitter:F0} ms"
                : "—";
            LossValue.Text = $"{metrics.PacketLossPercent:F1}%";

            ProbeStatus.Severity = metrics.Received > 0
                ? InfoBarSeverity.Success
                : InfoBarSeverity.Warning;
            ProbeStatus.Title = metrics.Received > 0
                ? "Gateway reachable"
                : "No reply";
            ProbeStatus.Message =
                $"{metrics.Received}/{metrics.Sent} probe packets returned.";
        }
        catch (Exception error)
        {
            ShowProbeError(error.GetType().Name);
        }
        finally
        {
            ProbeButton.IsEnabled = true;
        }
    }

    private void ShowProbeError(string message)
    {
        PingValue.Text = "—";
        JitterValue.Text = "—";
        LossValue.Text = "—";

        ProbeStatus.IsOpen = true;
        ProbeStatus.Severity = InfoBarSeverity.Error;
        ProbeStatus.Title = "Probe failed";
        ProbeStatus.Message = message;
    }
}
