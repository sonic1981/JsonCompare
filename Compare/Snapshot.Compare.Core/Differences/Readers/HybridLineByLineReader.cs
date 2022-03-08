using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    public class HybridLineByLineReader : ILineByLineReader
    {
        private readonly TextReader textReader;
        private Queue<string> additionalQueue;

        public HybridLineByLineReader(TextReader textReader)
        {
            this.textReader = textReader;
            this.additionalQueue = new Queue<string>();
        }

        public void AddToQueue(string line)
        {
            this.additionalQueue.Enqueue(line);
        }

        public void SetQueue(Queue<string> queue)
        {
            this.additionalQueue = queue;
        }

        public void Dispose()
        {
            this.textReader.Dispose();
        }

        public string ReadLine()
        {
            if (this.additionalQueue.Any())
                return this.additionalQueue.Dequeue();
            else
                return this.textReader.ReadLine();
        }
    }
}
