using Microsoft.Extensions.Primitives;
using Snapshot.Contract;
using System;
using System.Threading.Tasks;

namespace Snapshot.Compare.Core
{
    public interface IGetMediator
    {
        Task<CallWrapperResponse> GetSnapShot(Guid tenantId, Guid resultId, StringValues acceptHeader);
    }
}