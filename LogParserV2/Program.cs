using System.Collections.Concurrent;

namespace LogParserV2;

// docker run --rm -v /c/temp:/app/logs logparser-v2
// docker run --rm -v /path/to/logs:/app/logs -e LOG_PATH=/app/logs logparser-v2 "/app/logs" "2024-08-01" "2024-08-31"

internal class IisLogParser
{
    private static void Main(string?[] args)
    {
        string? logFolderPath;
        if (File.Exists("/.dockerenv"))
        {
            Console.WriteLine("docker env found");
            logFolderPath = Environment.GetEnvironmentVariable("LOG_PATH") ?? args[0] ?? "./logs";
        }
        else
        {
            Console.WriteLine("docker env not found");
            logFolderPath = args[0];
        }

        if (string.IsNullOrEmpty(logFolderPath)) throw new Exception("log folder path is null or empty");

        var logFormat = LogFormatIdentifier.IdentifyLogFormat(logFolderPath);
        Console.WriteLine($"Identified log format: {logFormat}");

        DateTime? startDate = args.Length > 1 ? DateTime.Parse(args[1]) : null;
        DateTime? endDate = args.Length > 2 ? DateTime.Parse(args[2]) : null;

        ParseIisLogsFromFolder(logFolderPath, startDate, endDate);
    }

    private static void ParseIisLogsFromFolder(string? logFolderPath, DateTime? startDate, DateTime? endDate)
    {
        var logEntries = new ConcurrentBag<LogEntry>();

        var logFiles = Directory.GetFiles(logFolderPath, "u_ex*.log");

        Parallel.ForEach(logFiles, logFilePath =>
        {
            var lines = File.ReadLines(logFilePath);

            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue; // Skip comments and headers

                try
                {
                    var logEntry = LogEntry.ParseFromLogLine(line);

                    // If date range is specified, filter by date
                    if ((startDate.HasValue && logEntry.Date < startDate) ||
                        (endDate.HasValue && logEntry.Date > endDate)) continue;

                    logEntries.Add(logEntry);
                }
                catch (FormatException)
                {
                    // Handle or log the error if needed
                }
            }
        });

        // Regular analysis
        PrintUniqueUsers(logEntries);
        PrintAllUsers(logEntries);
        PrintIpAddresses(logEntries);
        PrintTimeTaken(logEntries);

        // Security operations analysis
        DetectSuspiciousIPs(logEntries);
        DetectUnsuccessfulLoginAttempts(logEntries);
        DetectAccessToSensitiveResources(logEntries, new[] { "/admin", "/login", "/config" });
    }

    private static void PrintUniqueUsers(ConcurrentBag<LogEntry> logEntries)
    {
        var uniqueUsers = logEntries
            .Where(entry => !string.IsNullOrEmpty(entry.Username))
            .GroupBy(entry => entry.Username)
            .ToDictionary(g => g.Key, g => g.Count());

        Console.WriteLine("Unique Users:");
        GenerateAsciiBarChart(uniqueUsers);
        Console.WriteLine($"\nTotal Unique Users: {uniqueUsers.Count}");
    }

    private static void PrintAllUsers(ConcurrentBag<LogEntry> logEntries)
    {
        var allUsers = logEntries
            .Select(entry => entry.Username ?? "Anonymous")
            .GroupBy(user => user)
            .ToDictionary(g => g.Key, g => g.Count());

        Console.WriteLine("\nAll Users:");
        GenerateAsciiBarChart(allUsers);
        Console.WriteLine($"\nTotal User Entries: {allUsers.Count}");
    }

    private static void PrintIpAddresses(ConcurrentBag<LogEntry> logEntries)
    {
        var ipAddresses = logEntries
            .GroupBy(entry => entry.ClientIP)
            .ToDictionary(g => g.Key, g => g.Count());

        Console.WriteLine("\nIP Addresses:");
        GenerateAsciiBarChart(ipAddresses);
        Console.WriteLine($"\nTotal Unique IP Addresses: {ipAddresses.Count}");
    }

    private static void GenerateAsciiBarChart(Dictionary<string, int> data, int maxWidth = 50)
    {
        if (data.Count == 0)
        {
            Console.WriteLine("No data available for chart.");
            return;
        }

        // Determine the scale
        var maxValue = data.Values.Max();
        var scale = maxValue > maxWidth ? (double)maxWidth / maxValue : 1.0;

        foreach (var entry in data.OrderByDescending(kv => kv.Value))
        {
            var barLength = (int)(entry.Value * scale);
            var bar = new string('#', barLength);
            Console.WriteLine($"{entry.Key.PadRight(15)} | {bar} {entry.Value}");
        }
    }

    private static void PrintTimeTaken(ConcurrentBag<LogEntry> logEntries)
    {
        Console.WriteLine("\nTime Taken (ms):");
        foreach (var time in logEntries.Select(entry => entry.TimeTaken)) Console.WriteLine(time);
    }

    // SecOps Functions

    private static void DetectSuspiciousIPs(ConcurrentBag<LogEntry> logEntries, int threshold = 100)
    {
        var ipCounts = logEntries
            .GroupBy(entry => entry.ClientIP)
            .Select(group => new { IP = group.Key, Count = group.Count() })
            .Where(g => g.Count > threshold);

        Console.WriteLine("\nSuspicious IP Addresses (more than {0} requests):", threshold);
        foreach (var ip in ipCounts) Console.WriteLine($"{ip.IP} - {ip.Count} requests");
    }

    private static void DetectUnsuccessfulLoginAttempts(ConcurrentBag<LogEntry> logEntries)
    {
        var failedLogins = logEntries
            .Where(entry => entry.UriStem.Contains("/login") && entry.Status >= 400 && entry.Status < 500)
            .GroupBy(entry => entry.ClientIP)
            .Select(group => new { IP = group.Key, Count = group.Count() });

        Console.WriteLine("\nUnsuccessful Login Attempts:");
        foreach (var ip in failedLogins) Console.WriteLine($"{ip.IP} - {ip.Count} failed login attempts");
    }

    private static void DetectAccessToSensitiveResources(ConcurrentBag<LogEntry> logEntries,
        string[] sensitiveResources)
    {
        var sensitiveAccesses = logEntries
            .Where(entry => sensitiveResources.Any(resource => entry.UriStem.StartsWith(resource)))
            .GroupBy(entry => entry.UriStem)
            .Select(group => new { Resource = group.Key, Count = group.Count() });

        Console.WriteLine("\nAccess to Sensitive Resources:");
        foreach (var access in sensitiveAccesses) Console.WriteLine($"{access.Resource} - {access.Count} accesses");
    }
}