namespace MuonSync.Features;

internal static class Paths
{
    private const string DataDir = @"C:\Muonbox\code_muonsync\input_data";

    public static readonly string InputCsv       = Path.Combine(DataDir, "input.csv");
    public static readonly string CleanedCsv     = Path.Combine(DataDir, "input_cleaned.csv");
    public static readonly string IntegratedCsv  = Path.Combine(DataDir, "input_integrated.csv");
    public static readonly string CoincidenceCsv = Path.Combine(DataDir, "coincidence.csv");
    public static readonly string EventCountsCsv = Path.Combine(DataDir, "event_counts_per_minute.csv");
    public static readonly string InputGpx       = Path.Combine(DataDir, "input.gpx");
    public static readonly string TunnelGpx      = Path.Combine(DataDir, "tunnel.gpx");
}
