using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Snapshot.Compare.Core.Differences.Compare;
using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Compare.Core.Differences
{
    public class DiffEngine : IDiffEngine
    {
        private readonly ILogger<DiffEngine> logger;

        public DiffEngine(ILogger<DiffEngine> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<FileDiffResult> DiffFiles(string json1, string json2)
        {
            string yaml1 = jsonToYaml(json1);
            string yaml2 = jsonToYaml(json2);

            List<string> procesedIntents = new List<string>();
            List<FileDiffResult> returnResults = new List<FileDiffResult>();

            using (HybridLineByLineReader rd1 = new HybridLineByLineReader(new StringReader(yaml1)))
            using (HybridLineByLineReader rd2 = new HybridLineByLineReader(new StringReader(yaml2)))
            {
                string line1 = rd1.ReadLine();
                string line2 = rd2.ReadLine();

                string intent = null;

                //loop intents
                while (!string.IsNullOrEmpty(line1) || !string.IsNullOrEmpty(line2))
                {
                    logger.LogDebug($"Looping line '{line1}' - '{line2}'");

                    Match lineIsIntent = string.IsNullOrEmpty(line1) ? ParsingRegexs.IntentRegex.Match(line2) : ParsingRegexs.IntentRegex.Match(line1);

                    //intent
                    if (lineIsIntent.Success)
                    {
                        //missing intent
                        if (string.IsNullOrEmpty(line1)  || !line1.Equals(line2, StringComparison.OrdinalIgnoreCase))
                        {
                            FileDiffResult newResult = UnmatchedIntentCompare.Compare(ref line1, ref line2, rd1, rd2);
                            returnResults.Add(newResult);
                        }
                        procesedIntents.Add(intent);
                        intent = lineIsIntent.Groups[1].Value;

                        line1 = rd1.ReadLine();
                        line2 = rd2.ReadLine();
                    }
                    //errors
                    else if (ParsingRegexs.IsErrorLine.IsMatch(line1) || ParsingRegexs.IsErrorLine.IsMatch(line2))
                    {
                        FileDiffResult newResult = new FileDiffResult()
                        {
                            Intent = intent
                        };

                        ErrorCompare.Compare(ref line1, ref line2, rd1, rd2, newResult);

                        returnResults.Add(newResult);
                    }
                    //object
                    else if (ParsingRegexs.ObjectProperty.IsMatch(line1))
                    {
                        FileDiffResult newResult = new FileDiffResult()
                        {
                            Intent = intent
                        };

                        //list of object properties, just do a simple string compare
                        LineByLineCompare.Compare(ref line1, ref line2, rd1, rd2, newResult);

                        if (newResult.ObjectDifferences.Any() || newResult.ArrayDifferences1.Any() || newResult.ArrayDifferences2.Any())
                        {

                            returnResults.Add(newResult);
                        }
                    }
                    //array
                    else if(ParsingRegexs.ArrayRegex.IsMatch(line1))
                    {
                        FileDiffResult newResult = new FileDiffResult()
                        {
                            Intent = intent,
                        };

                        //array
                        ArrayCompare.Compare(ref line1, ref line2, rd1, rd2, newResult);

                        if (newResult.ObjectDifferences.Any() || newResult.ArrayDifferences1.Any() || newResult.ArrayDifferences2.Any())
                        {
                            returnResults.Add(newResult);
                        }

                        
                    }
                    else
                    {
                        throw new Exception($"Cannot process line {line1}");
                    }
                }
            }

            return returnResults;
        }

        private static string jsonToYaml(string json)
        {

            ExpandoObjectConverter expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);


            var serializer = new YamlDotNet.Serialization.Serializer();
            return serializer.Serialize(deserializedObject);

        }
    }
}
