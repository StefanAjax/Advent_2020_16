using System.Text.RegularExpressions;

namespace Advent_2020_16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataPath = "trueData.txt";
            var tickets = ParseTickets(dataPath);
            var limits = ParseLimits(dataPath);

            // Part 1
            var consolidatedLimits = ConsolidateLimits(limits);
            int ticketScanningErrorRate = GetTicketErrorScanningRate(tickets, consolidatedLimits);
            Console.WriteLine($"Ticket Scanning Error Rate: {ticketScanningErrorRate}");


            // Part 2
            var validTickets = GetValidTickets(tickets, consolidatedLimits);
            
                        
        }

        static List<string> GetFieldNamesInOrder(List<Ticket> validTickets, List<Limit> limits)
        {
            var result = new List<string>();
            var fieldNames = GetFieldNamesFromLimits(limits);
            


            return result;
        }

        static List<string> GetFieldNamesFromLimits(List<Limit> limits)
        {
            var result = new List<string>();
            foreach (var limit in limits) 
            { 
                if (!result.Contains(limit.FieldName)) result.Add(limit.FieldName);
            }
            return result;
        }

        static List<Limit> GetLimitsByFieldName(string fieldName, List<Limit> limitsInput)
        {
            List<Limit> result = new List<Limit>();
            foreach (var limit in limitsInput) {
                if (limit.FieldName == fieldName) result.Add(limit);    
            }
            return result;
        }

        static int GetTicketErrorScanningRate(List<Ticket> tickets, List<Limit> consolidatedLimits)
        {
            int result = 0;


            foreach (var ticket in tickets)
            {
                int fieldCount = ticket.Fields.Count;
                foreach (var field in ticket.Fields)
                {
                    bool fieldIsValid = false;
                    foreach (var limit in consolidatedLimits)
                    {
                        if (field >= limit.StartValue && field <= limit.EndValue)
                        {
                            fieldIsValid = true;
                            break;
                        }
                    }
                    if (!fieldIsValid) result += field;
                }
            }

            return result;
        }
        
        static List<Limit> ConsolidateLimits(List<Limit> input)
        {
            var result = new List<Limit>();
            input.Sort((a, b) => a.StartValue.CompareTo(b.StartValue));

            for (int i = 0; i < input.Count; i++)
            {
                int startValue = input[i].StartValue;
                int endValue = input[i].EndValue;   
                while (i+1<input.Count && input[i + 1].StartValue <= input[i].EndValue + 1)
                {
                    endValue = input[i + 1].EndValue;
                    i++;
                }
                result.Add(new Limit("???", startValue, endValue));
            }
            return result;
        }

        static List<Ticket> GetValidTickets(List<Ticket> tickets, List<Limit> limits)
        {
            var result = new List<Ticket>();
            foreach (var ticket in tickets) {
                int fieldCount = ticket.Fields.Count;
                int validFieldsCount = 0;
                foreach (var field in ticket.Fields)
                {
                    foreach (var limit in limits)
                    {
                        if (limit.IsWithinLimit(field))
                        {
                            validFieldsCount++;
                            break;
                        }
                    }
                }
                if (validFieldsCount == fieldCount) result.Add(ticket);
            }
            return result;
        }
        struct Limit
        {
            public string FieldName;
            public int StartValue;
            public int EndValue;

            public Limit (string fieldName, int startValue, int endValue)
            {
                StartValue = startValue;
                EndValue = endValue;
                FieldName = fieldName;
            }

            public bool IsWithinLimit(int input)
            {
                if (input >= StartValue && input <= EndValue) return true;
                else return false;
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
            string pattern = @"(?'fieldname'[a-z, ]+): (?'from1'\d+)-(?'to1'\d+) or (?'from2'\d+)-(?'to2'\d+)";
            var rg = new Regex(pattern, RegexOptions.Multiline);
            var matches = rg.Matches(input);
            foreach (Match match in matches) {
                if (match.Success) {
                    result.Add(new Limit(match.Groups["fieldname"].Value, int.Parse(match.Groups["from1"].Value), int.Parse(match.Groups["to1"].Value)));
                    result.Add(new Limit(match.Groups["fieldname"].Value, int.Parse(match.Groups["from2"].Value), int.Parse(match.Groups["to2"].Value)));
                }
            }
            return result;
        }
    }
}
