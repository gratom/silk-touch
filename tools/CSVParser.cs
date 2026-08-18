using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tools
{
    public static class CSVParser
    {
        public static int GetLinesCountFromFile(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllLines(filePath).Skip(1).Count() : 0;
        }

        public static List<List<string>> GetArrayFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return ParseArrayFromStringLines(File.ReadAllLines(filePath));
            }

            return null;
        }

        public static List<List<string>> ParseArrayFromString(string text)
        {
            return ParseArrayFromStringLines(text.Split('\n'));
        }

        public static List<List<string>> ParseArrayFromStringLines(string[] lines)
        {
            string[] cvsLines = lines.Skip(1).ToArray();
            List<List<string>> returnedArray = new List<List<string>>(cvsLines.Length);
            foreach (string line in cvsLines)
            {
                returnedArray.Add(Regex.Split(line, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)")
                    .Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            }

            return returnedArray;
        }
    }
}