namespace MuonSync.Features;

internal static class CountFeature
{
    private const ulong NsPerMinute = 60_000_000_000UL;

    public static int Run()
    {
        Console.WriteLine($"Input:  {Paths.CoincidenceCsv}");
        Console.WriteLine($"Output: {Paths.EventCountsCsv}");

        using var reader = new StreamReader(Paths.CoincidenceCsv);
        using var writer = new StreamWriter(Paths.EventCountsCsv);

        bool hasBase = false;
        ulong baseTime = 0;
        long currentMinute = 0;
        long count = 0;
        ulong integralSum = 0;
        long totalMinutesWritten = 0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;
            if (!ulong.TryParse(parts[0], out ulong timeNs)) continue;
            if (!ulong.TryParse(parts[1], out ulong avgIntegral)) continue;

            if (!hasBase)
            {
                baseTime = timeNs;
                hasBase = true;
            }

            long minute = (long)((timeNs - baseTime) / NsPerMinute);

            // Fill any gap minutes with zero counts before advancing
            while (currentMinute < minute)
            {
                ulong minuteAvg = count > 0 ? integralSum / (ulong)count : 0;
                writer.WriteLine($"{currentMinute} {count} {minuteAvg}");
                totalMinutesWritten++;
                currentMinute++;
                count = 0;
                integralSum = 0;
            }

            count++;
            integralSum += avgIntegral;
        }

        // Write the final minute (even if zero events arrived after the last flush)
        if (hasBase)
        {
            ulong minuteAvg = count > 0 ? integralSum / (ulong)count : 0;
            writer.WriteLine($"{currentMinute} {count} {minuteAvg}");
            totalMinutesWritten++;
        }

        Console.WriteLine($"Wrote {totalMinutesWritten} minute entries.");
        return 0;
    }
}
