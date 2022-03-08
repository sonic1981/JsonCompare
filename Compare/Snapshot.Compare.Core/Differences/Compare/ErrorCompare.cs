using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Compare
{
    public static class ErrorCompare
    {
        public static void Compare(ref string line1, ref string line2, ILineByLineReader rd1, ILineByLineReader rd2, FileDiffResult newResult)
        {
            //both sides in error
            if (line1.Equals(line2, StringComparison.OrdinalIgnoreCase))
            {
                newResult.InError1 = true;
                newResult.InError2 = true;

                if (ParsingRegexs.IsStringLiteral.IsMatch(line1))
                {
                    LiteralStringReader.ReadLiteral(ref line1, rd1);
                }
                else
                {
                    line1 = rd1.ReadLine();
                }

                if (ParsingRegexs.IsStringLiteral.IsMatch(line2))
                {
                    LiteralStringReader.ReadLiteral(ref line2, rd2);
                }
                else
                {
                    line2 = rd2.ReadLine();
                }
            }
            else
            {
                if (ParsingRegexs.IsErrorLine.IsMatch(line1))
                {
                    //line 1 is the error
                    newResult.InError1 = true;
                    fastForwardLine(ref line2, rd2);
                    line1 = rd1.ReadLine();
                }
                else
                {
                    // line 2 is the error
                    newResult.InError2 = true;
                    fastForwardLine(ref line1, rd1);
                    line2 = rd2.ReadLine();
                }

            }

            
            

            if (!ParsingRegexs.IntentRegex.IsMatch(line1) || !ParsingRegexs.IntentRegex.IsMatch(line2))
            {
                throw new Exception($"Expected an intent after error line. line1: {line1} line2: {line2}");
            }
        }

        private static void fastForwardLine(ref string line1, ILineByLineReader rd1)
        {
            while (!ParsingRegexs.IntentRegex.IsMatch(line1))
            {
                line1 = rd1.ReadLine();
            }
        }
    }
}
