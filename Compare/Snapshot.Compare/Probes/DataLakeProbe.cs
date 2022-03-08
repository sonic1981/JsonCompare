using Microsoft.Extensions.Logging;
using Snapshot.Probe;
using Snapshot.Probe.DataLake;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Compare.Probes
{
    public class DataLakeProbe : BaseDataLakeProbe
    {
        public DataLakeProbe(ILogger<BaseDataLakeProbe> logger) : base(logger)
        {

        }

        protected override string containerName()
        {
            return Environment.GetEnvironmentVariable("DataLakeContainerName");
        }

        protected override string dataLakeKey()
        {
            return Environment.GetEnvironmentVariable("DataLakeKey");
        }

        protected override string dataLakeName()
        {
            return Environment.GetEnvironmentVariable("DataLakeName");
        }
    }
}
