using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    [ExcludeFromCodeCoverage]
    public class QueueLineByLineReader : ILineByLineReader
    {
        private Queue<string> lines;

        public QueueLineByLineReader(Queue<string> lines)
        {
            this.lines = lines;
        }

        public void Dispose()
        {
            this.lines = null;
        }

        public string ReadLine()
        {
            return this.lines.Dequeue();
        }
    }
}
