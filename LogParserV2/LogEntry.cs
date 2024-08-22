namespace LogParserV2;
public class LogEntry
{
    public DateTime Date { get; set; }
    public string ServerIP { get; set; }
    public string HttpMethod { get; set; }
    public string UriStem { get; set; }
    public string UriQuery { get; set; }
    public int ServerPort { get; set; }
    public string Username { get; set; }
    public string ClientIP { get; set; }
    public string UserAgent { get; set; }
    public string Referer { get; set; }
    public int Status { get; set; }
    public int SubStatus { get; set; }
    public int Win32Status { get; set; }
    public int TimeTaken { get; set; }

    public static LogEntry ParseFromLogLine(string logLine)
    {
        var fields = logLine.Split(' ');
        if (fields.Length < 15) throw new FormatException("Invalid log line format.");

        return new LogEntry
        {
            Date = DateTime.ParseExact(fields[0] + " " + fields[1], "yyyy-MM-dd HH:mm:ss", null),
            ServerIP = fields[2],
            HttpMethod = fields[3],
            UriStem = fields[4],
            UriQuery = fields[5],
            ServerPort = int.Parse(fields[6]),
            Username = fields[7] != "-" ? fields[7] : null,
            ClientIP = fields[8],
            UserAgent = fields[9].Replace("+", " "), // Replacing "+" with spaces for better readability
            Referer = fields[10] != "-" ? fields[10] : null,
            Status = int.Parse(fields[11]),
            SubStatus = int.Parse(fields[12]),
            Win32Status = int.Parse(fields[13]),
            TimeTaken = int.Parse(fields[14])
        };
    }
}