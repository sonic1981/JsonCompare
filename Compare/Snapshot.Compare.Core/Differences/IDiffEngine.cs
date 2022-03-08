using Snapshot.Contract.Compare;
using System.Collections.Generic;

namespace Snapshot.Compare.Core.Differences
{
    public interface IDiffEngine
    {
        IEnumerable<FileDiffResult> DiffFiles(string yaml1, string yaml2);
    }
}