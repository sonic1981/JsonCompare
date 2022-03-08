using Snapshot.Auth;
using Snapshot.Contract;
using Snapshot.Contract.Compare;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snapshot.Compare.Core
{
    public interface ICompareMediator
    {
        Task<IEnumerable<FileDiffResult>> PerformCompare(CompareRequest data, JWTDecoder jWTDecoder);
    }
}