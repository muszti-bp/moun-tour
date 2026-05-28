namespace MuonSync.Features;

internal static class IntegralFeature
{
    private const int WaveformLength = 128;

    public static int Run()
    {
        Console.WriteLine($"Input:  {Paths.CleanedCsv}");
        Console.WriteLine($"Output: {Paths.IntegratedCsv}");

        long processed = 0;

        using var reader = new StreamReader(Paths.CleanedCsv);
        using var writer = new StreamWriter(Paths.IntegratedCsv);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2 + WaveformLength)
                continue;

            ulong integral = 0;
            for (int i = 2; i < 2 + WaveformLength; i++)
            {
                if (ulong.TryParse(parts[i], out ulong sample))
                    integral += sample;
            }

            // Output: channel time integral
            writer.WriteLine($"{parts[0]} {parts[1]} {integral}");
            processed++;
        }

        Console.WriteLine($"Processed {processed:N0} rows.");
        return 0;
    }
}
