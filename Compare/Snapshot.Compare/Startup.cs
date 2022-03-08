using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Snapshot.Compare.Core.Differences;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Snapshot.Compare.Core;
using Snapshot.Auth.Extensions;
using Snapshot.Probe.Extensions;
using Snapshot.Compare.Probes;
using Snapshot.Probe.DataLake;
using Snapshot.DataLake.Extensions;

[assembly: FunctionsStartup(typeof(Snapshot.Compare.Startup))]
namespace Snapshot.Compare
{

    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {


        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddDataLake<EnvironmentVariables>();

            builder.Services.AddAuth<EnvironmentVariables>();

            builder.Services.AddSingleton<IEnvironmentVariables, EnvironmentVariables>();

            builder.Services.AddSingleton<IDiffEngine, DiffEngine>();
            builder.Services.AddSingleton<ICompareMediator, CompareMediator>();
            builder.Services.AddSingleton<IGetMediator, GetMediator>();

            registerProbeDepedancies(builder);
        }

        private static void registerProbeDepedancies(IFunctionsHostBuilder builder)
        {
            builder.Services.AddProbe<EnvironmentVariablesProbe>();
            builder.Services.AddDataLakeProbe<DataLakeProbe>();
        }
    }
}
