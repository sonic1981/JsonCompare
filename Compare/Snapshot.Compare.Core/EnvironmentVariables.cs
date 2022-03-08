using Snapshot.DataLake.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Snapshot.Compare.Core
{
    [ExcludeFromCodeCoverage]
    public class EnvironmentVariables : IEnvironmentVariables
    {
        public string DataLakeContainerName => Environment.GetEnvironmentVariable("DataLakeContainerName");

        public string DataLakeName => Environment.GetEnvironmentVariable("DataLakeName");

        public string DataLakeKey => Environment.GetEnvironmentVariable("DataLakeKey");

        public string TenantIdClaimType => Environment.GetEnvironmentVariable("TenantIdClaimType");

        public string TenantIdClaimTypeLegacy => Environment.GetEnvironmentVariable("TenantIdClaimTypeLegacy");

        public string APPINSIGHTS_INSTRUMENTATIONKEY => Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

    }
}
