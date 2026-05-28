namespace MuonSync.Features;

internal static class CoincidenceFeature
{
    private const ulong CoincidenceWindowNs = 1_000; // 1 microsecond
    private const int NumChannels = 8;

    private readonly record struct Event(int Channel, ulong TimeNs, ulong Integral);

    public static int Run()
    {
        Console.WriteLine($"Input:  {Paths.IntegratedCsv}");
        Console.WriteLine($"Output: {Paths.CoincidenceCsv}");

        long coincidences = 0;

        using var reader = new StreamReader(Paths.IntegratedCsv);
        using var writer = new StreamWriter(Paths.CoincidenceCsv);

        // Group consecutive events that all fall within 1 µs of the group's first event.
        // The 20-event rolling window ensures no coincidence group exceeds 20 entries.
        var group = new List<Event>(20);
        ulong groupStartTime = 0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) continue;
            if (!int.TryParse(parts[0], out int ch)) continue;
            if (!ulong.TryParse(parts[1], out ulong t)) continue;
            if (!ulong.TryParse(parts[2], out ulong integral)) continue;

            var evt = new Event(ch, t, integral);

            if (group.Count == 0)
            {
                group.Add(evt);
                groupStartTime = t;
            }
            else if (t - groupStartTime <= CoincidenceWindowNs)
            {
                group.Add(evt);
            }
            else
            {
                if (group.Count >= 2)
                {
                    WriteCoincidence(writer, group);
                    coincidences++;
                }
                group.Clear();
                group.Add(evt);
                groupStartTime = t;
            }
        }

        // Flush the last group
        if (group.Count >= 2)
        {
            WriteCoincidence(writer, group);
            coincidences++;
        }

        Console.WriteLine($"Found {coincidences:N0} coincidences.");
        return 0;
    }

    private static void WriteCoincidence(StreamWriter writer, List<Event> group)
    {
        ulong firstTime = group[0].TimeNs;

        ulong integralSum = 0;
        foreach (var e in group) integralSum += e.Integral;
        ulong avgIntegral = integralSum / (ulong)group.Count;

        var channels = new int[NumChannels];
        foreach (var e in group)
        {
            if (e.Channel >= 0 && e.Channel < NumChannels)
                channels[e.Channel] = 1;
        }

        // Format: time avgIntegral ch0 ch1 ch2 ch3 ch4 ch5 ch6 ch7
        writer.WriteLine(
            $"{firstTime} {avgIntegral} " +
            $"{channels[0]} {channels[1]} {channels[2]} {channels[3]} " +
            $"{channels[4]} {channels[5]} {channels[6]} {channels[7]}");
    }
}
