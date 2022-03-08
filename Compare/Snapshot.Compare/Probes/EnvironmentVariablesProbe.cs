using Microsoft.Extensions.Logging;
using Snapshot.Compare.Core;
using Snapshot.Probe;
using Snapshot.Probe.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Compare.Probes
{
    public class EnvironmentVariablesProbe : BaseEnvironmentalVariableProbe<EnvironmentVariables>
    {
        public EnvironmentVariablesProbe(Probe.Wrappers.IProbeEnvironmentWrapper environmentWrapper, ILogger<BaseEnvironmentalVariableProbe<EnvironmentVariables>> logger) :
            base(environmentWrapper, logger)
        {

        }

    }
}
