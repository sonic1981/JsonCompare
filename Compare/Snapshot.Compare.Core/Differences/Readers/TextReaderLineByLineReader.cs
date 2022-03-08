using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    [ExcludeFromCodeCoverage]
    public class TextReaderLineByLineReader : ILineByLineReader
    {
        private readonly TextReader textReader;

        public TextReaderLineByLineReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public void Dispose()
        {
            this.textReader.Dispose();
        }

        public string ReadLine()
        {
            return this.textReader.ReadLine();
        }
    }
}
