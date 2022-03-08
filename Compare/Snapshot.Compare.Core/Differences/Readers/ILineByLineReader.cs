using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Readers
{
    public interface ILineByLineReader: IDisposable
    {
        public string ReadLine();
    }
}
