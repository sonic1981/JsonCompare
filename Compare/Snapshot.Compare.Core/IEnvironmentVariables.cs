using Snapshot.Auth;
using Snapshot.DataLake.Extensions;

namespace Snapshot.Compare.Core
{
    public interface IEnvironmentVariables: IAuthEnvironmentVariables, IDataLakeEnvironmentVariables
    {
        string DataLakeContainerName { get; }
        string DataLakeName { get; }
        string DataLakeKey { get; }
    }
}