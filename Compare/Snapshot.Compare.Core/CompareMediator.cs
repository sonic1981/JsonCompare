using Microsoft.Extensions.Logging;
using Snapshot.Compare.Core.Differences;
using Snapshot.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Snapshot.Contract.Compare;
using Snapshot.Auth;
using Snapshot.DataLake;

namespace Snapshot.Compare.Core
{
    public class CompareMediator : ICompareMediator
    {
        private readonly ILogger<CompareMediator> logger;
        private readonly IDataLakeReader dataLakeReader;
        private readonly IDiffEngine diffEngine;

        public CompareMediator(ILogger<CompareMediator> logger, IDataLakeReader dataLakeReader, IDiffEngine diffEngine)
        {
            this.logger = logger;
            this.dataLakeReader = dataLakeReader;
            this.diffEngine = diffEngine;
        }

        public async Task<IEnumerable<FileDiffResult>> PerformCompare(CompareRequest data, JWTDecoder jWTDecoder)
        {
            if (jWTDecoder == null)
                throw new ArgumentNullException("jWTDecoder");

            logger.LogDebug("Getting yaml for comparision");
            Task<string> yaml1Task = dataLakeReader.GetSnapshot(jWTDecoder.UnoTenantId, data.Result1);
            Task<string> yaml2Task = dataLakeReader.GetSnapshot(jWTDecoder.UnoTenantId, data.Result2);

            await Task.WhenAll(yaml1Task, yaml2Task);

            string yaml1 = yaml1Task.Result;
            string yaml2 = yaml2Task.Result;

            logger.LogDebug("Got yaml files");

            try
            {
                return diffEngine.DiffFiles(yaml1, yaml2);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error diffing files TenantId: {jWTDecoder.UnoTenantId} Result1: {data.Result1} Result2: {data.Result2}", ex);
            }

        }
    }
}
