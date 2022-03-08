using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    [ExcludeFromCodeCoverage]
    public class EmptyLineByLineReader : ILineByLineReader
    {
        public void Dispose()
        {
            //Do nothing
        }

        public string ReadLine()
        {
            return string.Empty;
        }
    }
}
