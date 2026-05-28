using MuonSync.Features;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

try
{
    return args[0].ToLower() switch
    {
        "clean"       => CleanFeature.Run(),
        "integral"    => IntegralFeature.Run(),
        "coincidence" => CoincidenceFeature.Run(),
        "count"       => CountFeature.Run(),
        "combine"     => CombineFeature.Run(args.Length > 1 ? double.Parse(args[1]) : 0.0),
        _             => UnknownCommand(args[0])
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("MuonSync - Muon detector data synchronization tool");
    Console.WriteLine();
    Console.WriteLine("Usage: MuonSync <command> [options]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  clean               Remove zero-waveform rows from input CSV");
    Console.WriteLine("  integral            Replace 128-sample waveforms with integrated values");
    Console.WriteLine("  coincidence         Find multi-channel coincidences within 1 microsecond");
    Console.WriteLine("  count               Count coincidences per minute");
    Console.WriteLine("  combine [shift_sec] Combine event counts with GPS track (shift in seconds)");
}

static int UnknownCommand(string cmd)
{
    Console.Error.WriteLine($"Unknown command: {cmd}");
    PrintUsage();
    return 1;
}
