namespace MuonSync.Features;

internal static class CleanFeature
{
    private const int WaveformLength = 128;
    private const int ZeroThreshold = WaveformLength / 2; // skip rows with >50% zero waveform samples

    public static int Run()
    {
        Console.WriteLine($"Input:  {Paths.InputCsv}");
        Console.WriteLine($"Output: {Paths.CleanedCsv}");

        long read = 0, written = 0;

        using var reader = new StreamReader(Paths.InputCsv);
        using var writer = new StreamWriter(Paths.CleanedCsv);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            read++;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Must have at least: channel + time + 128 waveform values
            if (parts.Length < 2 + WaveformLength)
                continue;

            int zeros = 0;
            for (int i = 2; i < 2 + WaveformLength; i++)
            {
                if (parts[i] == "0") zeros++;
            }

            if (zeros > ZeroThreshold)
                continue;

            // Write channel, time, and exactly 128 waveform columns (drop the trailing empty column)
            writer.Write(parts[0]);
            writer.Write(' ');
            writer.Write(parts[1]);
            for (int i = 2; i < 2 + WaveformLength; i++)
            {
                writer.Write(' ');
                writer.Write(parts[i]);
            }
            writer.WriteLine();
            written++;
        }

        Console.WriteLine($"Read {read:N0} rows, wrote {written:N0} rows (removed {read - written:N0} rows).");
        return 0;
    }
}
