using System.Text.RegularExpressions;

namespace Advent_2020_16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataPath = "exampleData.txt";
            var tickets = ParseTickets(dataPath);
            var limits = ParseLimits(dataPath);
            var validTickets = GetValidTickets(tickets, limits);
        }

        static List<Ticket> GetValidTickets(List<Ticket> tickets, List<Limit> limits)
        {
            var result = new List<Ticket>();
            foreach (var ticket in tickets)
            {
                // TODO
            }
            return result;
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

        class Ticket
        {
            public List<int> Fields;
            public Ticket(List<int> fields) { 
                Fields = fields;
            }
            public bool CheckValidity(List<Limit> limits)
            {
                return true;
            }
        }

        static List<Ticket> ParseTickets(string dataPath)
        {
            var tickets = new List<Ticket>();
            string input = File.ReadAllText(dataPath);
            string splitInput = input.Split("nearby tickets:")[1].TrimStart();
            string[] lines = splitInput.Split('\n');
            foreach (string line in lines)
            {
                var fields = new List<int>();
                string pattern = @"\d+";
                var matches = Regex.Matches(line, pattern);
                foreach (Match match in matches) {
                    fields.Add(int.Parse(match.Value));
                }
                tickets.Add(new Ticket(fields));
            }
            return tickets;
        }
        static List<Limit> ParseLimits(string dataPath) {
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
