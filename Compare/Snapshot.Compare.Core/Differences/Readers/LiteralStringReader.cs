using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    public static class LiteralStringReader
    {

        /// <summary>
        /// Will only process 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="rd1"></param>
        /// <returns></returns>
        public static string ReadLiteral(ref string line, ILineByLineReader rd1)
        {
            StringBuilder error = new StringBuilder();

            do
            {
                line = rd1.ReadLine().Trim();
                error.Append(line);
            }
            while (line == string.Empty || (ParsingRegexs.ObjectProperty.IsMatch(line) == false &&
                    ParsingRegexs.IntentRegex.IsMatch(line) == false &&
                    ParsingRegexs.ArrayRegex.IsMatch(line) == false));

            return error.ToString();
        }

    }
}
