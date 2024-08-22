using System;
using System.Collections.Generic;
using System.Linq;

public class AsciiDiagramGenerator
{
    public static void GenerateBarChart(Dictionary<string, int> data, string title, int maxWidth = 50)
    {
        Console.WriteLine($"\n{title}");
        Console.WriteLine(new string('=', title.Length));
        
        // Determine the scale
        int maxValue = data.Values.Max();
        double scale = maxValue > maxWidth ? (double)maxWidth / maxValue : 1.0;

        foreach (var entry in data.OrderByDescending(kv => kv.Value))
        {
            int barLength = (int)(entry.Value * scale);
            string bar = new string('#', barLength);
            Console.WriteLine($"{entry.Key.PadRight(15)} | {bar} {entry.Value}");
        }
    }
}