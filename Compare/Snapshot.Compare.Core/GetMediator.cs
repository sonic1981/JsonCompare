using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Snapshot.Contract;
using Snapshot.DataLake;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Snapshot.Compare.Core
{
    public class GetMediator : IGetMediator
    {
        private const string jsonAcceptHeader = "application/json";
        private readonly string[] yamlAcceptHeader = new string[] {
            "application/x-yaml",
            "text/yaml",
            "text/vnd.yaml",
            "text/x-yaml",
            "application/yaml",
        };




        private readonly IDataLakeReader dataLakeReader;
        private readonly ILogger<GetMediator> logger;

        public GetMediator(IDataLakeReader dataLakeReader, ILogger<GetMediator> logger)
        {
            this.dataLakeReader = dataLakeReader;
            this.logger = logger;
        }

        public async Task<CallWrapperResponse> GetSnapShot(Guid tenantId, Guid resultId, StringValues acceptHeader)
        {
            string json = await dataLakeReader.GetSnapshot(tenantId, resultId);

            if (acceptHeader != StringValues.Empty)
            {
                string firstHeader = acceptHeader.First();

                if (yamlAcceptHeader.Any(a => a.Equals(firstHeader, StringComparison.OrdinalIgnoreCase)))
                {
                    return new CallWrapperResponse()
                    {
                        Content = convertJsonToYaml(json),
                        MediaType = "text/vnd.yaml"
                    };
                }

                if (firstHeader.Equals(jsonAcceptHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return new CallWrapperResponse()
                    {
                        Content = json,
                        MediaType = jsonAcceptHeader
                    };
                }


            }

            return new CallWrapperResponse()
            {
                Content = json,
                MediaType = jsonAcceptHeader
            }; ;
        }

        private static string convertJsonToYaml(string json)
        {

            var expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);

            var serializer = new YamlDotNet.Serialization.Serializer();
            return serializer.Serialize(deserializedObject);
        }
    }
}
