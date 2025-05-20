using System.Text.RegularExpressions;

namespace Advent_2020_16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataPath = "exampleData.txt";
            var limits = parseData(dataPath);
            //limits.Sort((a,b) => a.Value.CompareTo(b.Value));
            foreach (var limit in limits) {
                Console.WriteLine($"{limit.StartValue} - {limit.EndValue}");
            }
        }

        struct Limit
        {
            public int StartValue;
            public int EndValue;

            public Limit (int startValue, int endValue)
            {
                StartValue = startValue;
                EndValue = endValue;
            }
        }

        static List<Limit> parseData(string dataPath) {
            var result = new List<Limit>();
            string input = File.ReadAllText(dataPath);
            string pattern = @"(?'from1'\d+)-(?'to1'\d+) or (?'from2'\d+)-(?'to2'\d+)";
            var rg = new Regex(pattern, RegexOptions.Multiline);
            var matches = rg.Matches(input);
            foreach (Match match in matches) {
                if (match.Success) {
                    result.Add(new Limit(int.Parse(match.Groups["from1"].Value), int.Parse(match.Groups["to1"].Value)));
                    result.Add(new Limit(int.Parse(match.Groups["from2"].Value), int.Parse(match.Groups["to2"].Value)));
                }
            }

            return result;
        }
    }
}
