namespace LogParserV2;

class LogFormatIdentifier
{
    private static readonly Dictionary<string, List<string>> KnownFormats = new Dictionary<string, List<string>>()
    {
        { "W3C", new List<string> { "date", "time", "s-ip", "cs-method", "cs-uri-stem", "cs-uri-query", "s-port", "cs-username", "c-ip", "cs(User-Agent)", "cs(Referer)", "sc-status", "sc-substatus", "sc-win32-status", "time-taken" } },
        // Add more known formats here with their corresponding fields
    };

    public static string IdentifyLogFormat(string logFilePath)
    {
        int readMaxLines = 2000;
        var logFiles = Directory.GetFiles(logFilePath, "u_ex*.log");
        foreach (var logFile in logFiles)
        {
            using (var reader = new StreamReader(logFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#Fields:"))
                    {
                        var fields = line.Substring(8).Trim().Split(' ').ToList();
                        return MatchKnownFormat(fields);
                    }
                    
                }
            }

                 
        }
        throw new FileNotFoundException("Unable to match log file");
    }

    private static string MatchKnownFormat(List<string> fields)
    {
        foreach (var format in KnownFormats)
        {
            if (fields.SequenceEqual(format.Value))
            {
                return format.Key;
            }
        }

        return "Unknown format";
    }
}