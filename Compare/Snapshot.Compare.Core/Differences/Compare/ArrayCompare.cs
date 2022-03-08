using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using Snapshot.Compare.Core.Differences.Result.Array;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Compare.Core.Differences.Compare
{
    public static class ArrayCompare
    {

        public static void Compare(ref string line1, ref string line2, ILineByLineReader rd1, ILineByLineReader rd2, FileDiffResult result, List<string> parentProperties = null)
        {
            Queue<string> readLines1;
            Queue<string> readLines2;

            List<ArrayItem> values1 = PopulateArray(ref line1, rd1, out readLines1);
            List<ArrayItem> values2 = PopulateArray(ref line2, rd2, out readLines2);

            //If this is an array of one then there i no need to do the complicated array comparision, just drop to line by line
            if (values1.Count() <= 1 && values2.Count() <= 1)
            {
                CompareSingleArrayItem(readLines1, readLines2, result, parentProperties);
            }
            else
            {

                HashSet<int> hs1 = new HashSet<int>(values1.Select(s => s.GetHashCode()));
                HashSet<int> hs2 = new HashSet<int>(values2.Select(s => s.GetHashCode()));

                //hs1 now only contains the differences
                hs1.SymmetricExceptWith(hs2);


                result.ArrayDifferences1 = ArrayResultToDifferences(values1, hs1).ToList();
                result.ArrayDifferences2 = ArrayResultToDifferences(values2, hs1).ToList();
            }
        }

        private static IEnumerable<ArrayResults> ArrayResultToDifferences(List<ArrayItem> values1, HashSet<int> hs1)
        {
            foreach (ArrayItem val in values1)
            {
                if (hs1.Contains(val.GetHashCode()))
                {
                    yield return new ArrayResults()
                    {
                        Values = val.Lines
                    };
                }
            }
        }

        /// <summary>
        /// Treat single arrays, an arrys with one item on both sides, as a simple line by line compare
        /// </summary>
        private static void CompareSingleArrayItem(Queue<string> readLines1, Queue<string> readLines2, FileDiffResult result, List<string> parentProperties = null)
        {
            //got back over the data that's already been processed by PopulateArray
            string line1 = readLines1.Any() ? readLines1.Dequeue() : string.Empty;
            string line2 = readLines2.Any() ? readLines2.Dequeue() : string.Empty;

            //get rid of the array designator as it confuses the regexs
            line1 = line1.Replace("-", " ");
            line2 = line2.Replace("-", " ");

            ILineByLineReader reader1 = generateLineReader(readLines1);
            ILineByLineReader reader2 = generateLineReader(readLines2);

            LineByLineCompare.Compare(ref line1, ref line2, reader1, reader2, result, parentProperties);

            if (readLines1.Any() || readLines2.Any())
                throw new Exception("Not all lines have been dequeued");

        }

        private static ILineByLineReader generateLineReader(Queue<string> readLines1)
        {
            ILineByLineReader reader1;
            if (readLines1.Any())
            {
                reader1 = new QueueLineByLineReader(readLines1);
            }
            else
            {
                reader1 = new EmptyLineByLineReader();
            }

            return reader1;
        }

        private static List<ArrayItem> PopulateArray(ref string line, ILineByLineReader rd, out Queue<string> readLines)
        {
            readLines = new Queue<string>();
            List<ArrayItem> values = new List<ArrayItem>();

            if (!string.IsNullOrEmpty(line))
            {
                readLines.Enqueue(line);
                

                while (!string.IsNullOrEmpty(line) && !ParsingRegexs.IntentRegex.IsMatch(line))
                {
                    if (ParsingRegexs.ArrayRegex.IsMatch(line))
                    {
                        values.Add(new ArrayItem());
                    }

                    SingleLineResult sr = getLineResult(line);
                    values.Last().Lines.Add(sr);

                    line = rd.ReadLine();
                    readLines.Enqueue(line);
                }
            }

            return values;
        }

        private static SingleLineResult getLineResult(string line)
        {
            Match line1ValueMatch = ParsingRegexs.ValueRegex.Match(line);

            if (!line1ValueMatch.Success)
                throw new Exception($"Failed to parse line value: {line}");


            SingleLineResult lineResult = new SingleLineResult()
            {
                Key = line1ValueMatch.Groups[1].Value,
                Value = line1ValueMatch.Groups[3].Value,
            };
            return lineResult;
        }


    }
}
