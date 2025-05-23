using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent_2020_16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataPath = "trueData.txt";
            List<Ticket> tickets = ParseTickets(dataPath);
            List<Limit> limits = ParseLimits(dataPath);
            var myTicket = new Ticket(new List<int>() { 179, 101, 223, 107, 127, 211, 191, 61, 199, 193, 181, 131, 89, 109, 197, 59, 227, 53, 103, 97 });

            // Part 1
            List<Limit> consolidatedLimits = ConsolidateLimits(limits);
            int ticketScanningErrorRate = GetTicketErrorScanningRate(tickets, consolidatedLimits);
            Console.WriteLine($"Ticket Scanning Error Rate: {ticketScanningErrorRate}");

            // Part 2
            List<Ticket> validTickets = GetValidTickets(tickets, consolidatedLimits);
            List<String> fieldNames = GetFieldNamesFromLimits(limits);
            List<List<String>> possibleFieldNames = GeneratePossibleFieldNames(fieldNames);
            possibleFieldNames = RemovePossibleFieldnamesOutsideOfRanges(possibleFieldNames, validTickets, limits);
            string[] resolvedFields = ResolveFieldsByDeduction(possibleFieldNames);
            long result = CalculateResults(myTicket, resolvedFields);
            Console.WriteLine($"Part 2 result = {result}");
        }

        static long CalculateResults(Ticket myTicket, string[] resolvedFields)
        {
            long result = 1;
            for (int i = 0; i < resolvedFields.Length; i++)
            {
                if (resolvedFields[i].StartsWith("departure"))
                {
                    result *= myTicket.Fields[i];
                }
            }
            return result;
        }
        static string[] ResolveFieldsByDeduction(List<List<String>> possibleFieldNames)
        {
            var result = new string[possibleFieldNames.Count];
            while (possibleFieldNames.Any(p => p.Count > 1))
            {
                for (int i = 0; i < possibleFieldNames.Count; i++)
                {
                    if (possibleFieldNames[i].Count == 1)
                    {
                        result[i] = possibleFieldNames[i][0];
                        for (int j = 0; j < possibleFieldNames.Count; j++)
                        {
                            if (i != j)
                            {
                                possibleFieldNames[j].Remove(result[i]);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < possibleFieldNames.Count; i++)
            {
                if (possibleFieldNames[i].Count == 1)
                {
                    result[i] = possibleFieldNames[i][0];
                }
            }
            return result;
        }
        static List<List<String>> RemovePossibleFieldnamesOutsideOfRanges(List<List<String>> possibleFieldNames, List<Ticket> validTickets, List<Limit> limits)
        {
            for (int ticketIndex = 0; ticketIndex < validTickets.Count; ticketIndex++)
            {
                var ticket = validTickets[ticketIndex];

                for (int i = 0; i < ticket.Fields.Count; i++)
                {
                    int currentField = ticket.Fields[i];
                    var toRemove = new List<string>();

                    foreach (var fieldName in possibleFieldNames[i]) // iterate over possible field names for this position
                    {
                        var currentLimits = GetLimitsByFieldName(fieldName, limits);
                        bool isInRange1 = currentLimits[0].StartValue <= currentField && currentLimits[0].EndValue >= currentField;
                        bool isInRange2 = currentLimits[1].StartValue <= currentField && currentLimits[1].EndValue >= currentField;
                        if (!(isInRange1 || isInRange2))
                        {
                            toRemove.Add(fieldName); // mark for removal
                        }
                    }

                    foreach (var fieldName in toRemove)
                    {
                        possibleFieldNames[i].Remove(fieldName); // safe to remove now
                    }
                }
            }

            return possibleFieldNames;
        }

        static List<List<String>> GeneratePossibleFieldNames(List<String> fieldNames)
        {
            var result = new List<List<String>>();
            for (int i = 0; i < fieldNames.Count; i++)
            {
                result.Add(new List<string>());
                foreach (var fieldName2 in fieldNames)
                {
                    result[i].Add(fieldName2);
                }
            }
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
            limits.Sort((a, b) => a.StartValue.CompareTo(b.StartValue));
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
        class Limit
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
