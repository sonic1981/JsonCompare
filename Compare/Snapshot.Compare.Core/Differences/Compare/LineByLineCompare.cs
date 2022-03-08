using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Compare.Core.Differences.Compare
{
    public static class LineByLineCompare
    {

        public static void Compare(ref string line1, ref string line2, ILineByLineReader rd1, ILineByLineReader rd2, FileDiffResult newResult, List<string> parentProperties = null)
        {
            if (parentProperties == null)
                parentProperties = new List<string>();

            List<CompareLineResult> returnVal = new List<CompareLineResult>();


            while (lineValidForLoop(line1) ||
                lineValidForLoop(line2))
            {

                Match line1M = ParsingRegexs.ValueRegex.Match(line1);

                string key = calculateNestedProperties(line1, parentProperties, line1M);


                if (!line1.Equals(line2, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(line1) || string.IsNullOrEmpty(line2))
                    {
                        Match line2M = ParsingRegexs.ValueRegex.Match(line2);

                        returnVal.Add(new CompareLineResult()
                        {
                            Key = key,
                            Value1 = line1M.Groups[3].Value,
                            Value2 = line2M.Groups[3].Value
                        });
                    }
                    //ignore empty array lines as it's the content that matters
                    else if (line1.Contains("[]") == false && line2.Contains("[]") == false)
                    {
                        Match line2M = ParsingRegexs.ValueRegex.Match(line2);

                        returnVal.Add(new CompareLineResult()
                        {
                            Key = key,
                            Value1 = line1M.Groups[3].Value,
                            Value2 = line2M.Groups[3].Value
                        });
                    }
                    
                }


                //Stop reading if we hit the intent on the other file
                //all subsequent loops should return a diff now
                if (!ParsingRegexs.IntentRegex.IsMatch(line1))
                    line1 = rd1.ReadLine();


                if (!ParsingRegexs.IntentRegex.IsMatch(line2))
                    line2 = rd2.ReadLine();


            }
            newResult.ObjectDifferences = returnVal;

            if (!string.IsNullOrEmpty(line1) && ParsingRegexs.ArrayRegex.IsMatch(line1))
            {
                ArrayCompare.Compare(ref line1, ref line2, rd1, rd2, newResult, parentProperties);
            }

            if (!string.IsNullOrEmpty(line2) && ParsingRegexs.ArrayRegex.IsMatch(line2))
            {
                ArrayCompare.Compare(ref line1, ref line2, rd1, rd2, newResult, parentProperties);
            }

        }

        private static bool lineValidForLoop(string line)
        {
            return string.IsNullOrEmpty(line) == false && 
                ParsingRegexs.ObjectProperty.IsMatch(line);
        }

        private static string calculateNestedProperties(string line1, List<string> nestingQueue, Match line1M)
        {
            int amountOfNesting = ParsingRegexs.NestingRegex.Matches(line1).Count;

            if (nestingQueue.Count == amountOfNesting)
            {
                if (nestingQueue.Any())
                    nestingQueue.RemoveAt(nestingQueue.Count - 1);

                nestingQueue.Add(line1M.Groups[1].Value);
            }
            else if (nestingQueue.Count < amountOfNesting)
            {
                nestingQueue.Add(line1M.Groups[1].Value);
            }

            return string.Join(".", nestingQueue);
        }

    }
}
