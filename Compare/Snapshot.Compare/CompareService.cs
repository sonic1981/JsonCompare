using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Snapshot.Contract;
using Snapshot.Compare.Core;
using System.Collections.Generic;
using Snapshot.Contract.Compare;
using System.Diagnostics.CodeAnalysis;
using Snapshot.Auth;
using Snapshot.Probe.Model;
using Snapshot.Probe;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Primitives;
using Snapshot.Swagger.Contract.Attributes;
using Microsoft.OpenApi.Models;
using Snapshot.Swagger.Contract.Enums;
using Snapshot.Contract.SwaggerContract;

namespace Snapshot.Compare
{
    [ExcludeFromCodeCoverage]
    public class CompareService : ICompareService
    {
        private readonly ICompareMediator compareMediator;
        private readonly IAuthenticatedCallWrapper authenticatedCallWrapper;
        private readonly IProbe probe;
        private readonly IGetMediator getMediator;
        private readonly ILogger<CompareService> logger;

        public CompareService(ICompareMediator compareMediator, IAuthenticatedCallWrapper authenticatedCallWrapper, IProbe probe, IGetMediator getMediator, ILogger<CompareService> logger)
        {
            this.compareMediator = compareMediator;
            this.authenticatedCallWrapper = authenticatedCallWrapper;
            this.probe = probe;
            this.getMediator = getMediator;
            this.logger = logger;
        }

        
        [FunctionName("Compare")]
        public async Task<IActionResult> Compare(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            logger.LogDebug("Recieved Http request to compare snapshots");

            CompareRequest request = getRequest(req);

            return await authenticatedCallWrapper.CallWithAuthentication<IEnumerable<FileDiffResult>>((JWTDecoder decoded) =>
                compareMediator.PerformCompare(request, decoded),
            req);

        }

        
        [FunctionName("Get")]
        public async Task<HttpResponseMessage> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get/{id:guid}")] HttpRequest req, Guid id)
        {
            logger.LogDebug("Recieved Http request to compare snapshots");

            StringValues acceptHeader;
            req.Headers.TryGetValue("accept", out acceptHeader);

            return await authenticatedCallWrapper.CallWithAuthentication((JWTDecoder decoded) =>
                getMediator.GetSnapShot(decoded.UnoTenantId, id, acceptHeader),
            req);

        }

        private static CompareRequest getRequest(HttpRequest req)
        {

            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(req.Body))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<CompareRequest>(jsonTextReader);
            }
        }

        [FunctionName("Probe")]
        public IActionResult Probe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req)
        {
            logger.LogDebug("Recieved Http request to probe");

            ProbeResult result = probe.PerformProbe();

            return new OkObjectResult(result);
        }
    }
}
