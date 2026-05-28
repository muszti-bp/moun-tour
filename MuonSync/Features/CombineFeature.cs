using System.Xml.Linq;

namespace MuonSync.Features;

internal static class CombineFeature
{
    private static readonly XNamespace GpxNs = "http://www.topografix.com/GPX/1/1";

    public static int Run(double shiftSeconds)
    {
        Console.WriteLine($"GPS input:    {Paths.InputGpx}");
        Console.WriteLine($"Counts input: {Paths.EventCountsCsv}");
        Console.WriteLine($"Output:       {Paths.TunnelGpx}");
        Console.WriteLine($"Shift:        {shiftSeconds} seconds");

        var eventCounts = LoadEventCounts();
        Console.WriteLine($"Loaded {eventCounts.Count} minute entries.");

        var doc = XDocument.Load(Paths.InputGpx);

        var trackPoints = doc.Descendants(GpxNs + "trkpt").ToList();
        DateTime? baseTime = null;
        int replaced = 0;

        foreach (var trkpt in trackPoints)
        {
            var timeElem = trkpt.Element(GpxNs + "time");
            if (timeElem == null) continue;

            if (!DateTime.TryParse(timeElem.Value, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime pointTime))
                continue;

            pointTime = pointTime.ToUniversalTime();
            baseTime ??= pointTime;

            // Elapsed seconds from GPS start, adjusted by the shift parameter
            double elapsed = (pointTime - baseTime.Value).TotalSeconds + shiftSeconds;
            if (elapsed < 0) continue;

            long minute = (long)(elapsed / 60.0);

            var eleElem = trkpt.Element(GpxNs + "ele");
            if (eleElem != null && eventCounts.TryGetValue(minute, out var entry))
            {
                eleElem.Value = (entry.Count * 10).ToString();
                replaced++;
            }
        }

        doc.Save(Paths.TunnelGpx);
        Console.WriteLine($"Replaced elevation on {replaced} of {trackPoints.Count} track points.");
        return 0;
    }

    private static Dictionary<long, (long Count, ulong AvgIntegral)> LoadEventCounts()
    {
        var dict = new Dictionary<long, (long Count, ulong AvgIntegral)>();
        using var reader = new StreamReader(Paths.EventCountsCsv);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;
            if (!long.TryParse(parts[0], out long minute)) continue;
            if (!long.TryParse(parts[1], out long count)) continue;
            ulong avgIntegral = parts.Length > 2 && ulong.TryParse(parts[2], out ulong ai) ? ai : 0;
            dict[minute] = (count, avgIntegral);
        }
        return dict;
    }
}
