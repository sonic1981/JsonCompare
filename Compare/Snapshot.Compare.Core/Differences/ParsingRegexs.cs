using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Compare.Core.Differences
{
    public static class ParsingRegexs
    {
        //These are compiled as they are used recursively and as such compiling should provide the fastest execution
        public static readonly Regex IntentRegex = new Regex(@"^(\w+):( \[\])?$", RegexOptions.Compiled);
        public static readonly Regex ArrayRegex = new Regex(@"- \w+:", RegexOptions.Compiled);
        public static readonly Regex ValueRegex = new Regex(@"\s(\w+):( (.+)?)?$", RegexOptions.Compiled);
        public static readonly Regex ObjectProperty = new Regex(@"^(\s\s)+\w+:", RegexOptions.Compiled);
        public static readonly Regex NestingRegex = new Regex(@"(\s\s)|(-\s)", RegexOptions.Compiled);
        public static readonly Regex IsErrorLine = new Regex(@"\s+error:", RegexOptions.Compiled);
        public static readonly Regex EmptyArrayProperty = new Regex(@"^(\s\s)+\w+:\s\[\]$", RegexOptions.Compiled);
        /// <summary>
        /// https://stackoverflow.com/a/21699210/542251
        /// </summary>
        public static readonly Regex IsStringLiteral = new Regex(@":\s[>\|][+-]?$", RegexOptions.Compiled);
    }
}
