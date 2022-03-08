using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Compare.Core.Differences.Compare
{
    public static class UnmatchedIntentCompare
    {

        public static FileDiffResult Compare(ref string line1, ref string line2, HybridLineByLineReader rd1, HybridLineByLineReader rd2)
        {
            if (!string.IsNullOrEmpty(line1))
            {
                Match line1IntentMatch = ParsingRegexs.IntentRegex.Match(line1);
                if (line1IntentMatch.Success && line1IntentMatch.Groups[2].Success)
                    line1 = rd1.ReadLine();
            }


            if (!string.IsNullOrEmpty(line2) && !ParsingRegexs.IntentRegex.IsMatch(line2))
                throw new Exception($"Unmatched intent lines: Line1: {line1} Line2: {line2}");

            Queue<string> replay1;
            Queue<string> replay2;
            int recordContatingAdditionalIntent = readForwardTillMatch(line1, line2, rd1, rd2, out replay1, out replay2);

            if (recordContatingAdditionalIntent == 1)
            {
                //set these to the next value for processing after buildDiffResult below
                line2 = replay2.Peek();
                line1 = replay2.Dequeue();
                

                //put the read ahead back into the reader to be read again
                rd2.SetQueue(replay2);
                return buildDiffResult(replay1);
            }
            else
            {
                //set these to the next value for processing after buildDiffResult below
                line1 = replay1.Peek();
                line2 = replay1.Dequeue();

                //put the read ahead back into the reader to be read again
                rd1.SetQueue(replay1);
                return buildDiffResult(replay2);
            }
        }

        private static FileDiffResult buildDiffResult(Queue<string> replay)
        {
            string line1 = replay.Dequeue();
            string line2 = string.Empty;

            Match intentMatch = ParsingRegexs.IntentRegex.Match(line1);

            FileDiffResult returnVal = new FileDiffResult()
            {
                Intent = intentMatch.Groups[1].Value
            };

            line1 = replay.Dequeue();


            // EmptyLineByLineReader here ensures that ALL properties/array values are returned as we have no matching value on the other side
            if (ParsingRegexs.ObjectProperty.IsMatch(line1))
            {
                LineByLineCompare.Compare(ref line1, ref line2, new QueueLineByLineReader(replay), new EmptyLineByLineReader(), returnVal);
            }
            else
            {
                ArrayCompare.Compare(ref line1, ref line2, new QueueLineByLineReader(replay), new EmptyLineByLineReader(), returnVal);
            }

            return returnVal;
        }

        private static int readForwardTillMatch(string lineToMatch1, string lineToMatch2, ILineByLineReader rd1, ILineByLineReader rd2, 
            out Queue<string> linesToReply1, out Queue<string> linesToReply2)
        {
            linesToReply1 = new Queue<string>();
            linesToReply2 = new Queue<string>();

            linesToReply1.Enqueue(lineToMatch1);
            linesToReply2.Enqueue(lineToMatch2);

            //move forward till we get paritry again
            while(linesToReply1.Last() != lineToMatch2 && linesToReply2.Last() != lineToMatch1)
            {
                linesToReply1.Enqueue(rd1.ReadLine());
                linesToReply2.Enqueue(rd2.ReadLine());

            }



            //which line record has the additional intent in it?
            if (linesToReply1.Last() != lineToMatch2)
                return 2;
            else
                return 1;
        }
    }
}
