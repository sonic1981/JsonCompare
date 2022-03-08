using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Snapshot.Compare.Core.Differences;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snapshot.Tests.Compare.Differences
{
    [TestClass]
    public class DiffEngineTests
    {
        [TestMethod]
        public void DiffFiles()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\c6a248e0-0f62-45f8-8803-1537adda4b15.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            HashSet<string> expectedIntents = new HashSet<string>() { "cpu", "diskqueuelength", "drives", "ipconfig", "memory", "netstat",
                "networkadapters", "processes", "tlsconnections", "uptime", "networkdrives" };

            HashSet<string> actualIntents = new HashSet<string>(results.Select(s => s.Intent));

            actualIntents.SymmetricExceptWith(expectedIntents);
            Assert.IsFalse(actualIntents.Any(), "Intents list does not match");

            FileDiffResult cpuResult = results.FirstOrDefault(f => f.Intent == "cpu");
            Assert.AreEqual(6, cpuResult.ObjectDifferences.Count());

            FileDiffResult diskQResult = results.FirstOrDefault(f => f.Intent == "diskqueuelength");
            Assert.AreEqual(1, diskQResult.ObjectDifferences.Count());
            Assert.AreEqual("instances.averageQueueLength", diskQResult.ObjectDifferences.First().Key);

            FileDiffResult drivesResult = results.FirstOrDefault(f => f.Intent == "drives");
            Assert.IsFalse(drivesResult.ArrayDifferences1.Any());
            Assert.AreEqual(1, drivesResult.ObjectDifferences.Count());

            FileDiffResult ipConfig = results.FirstOrDefault(f => f.Intent == "ipconfig");
            Assert.IsFalse(ipConfig.ArrayDifferences1.Any());
            Assert.AreEqual(1, ipConfig.ObjectDifferences.Count());

            FileDiffResult memory = results.FirstOrDefault(f => f.Intent == "memory");
            Assert.IsFalse(memory.ArrayDifferences1.Any());
            Assert.AreEqual(2, memory.ObjectDifferences.Count());

            FileDiffResult netstat = results.FirstOrDefault(f => f.Intent == "netstat");
            Assert.AreEqual(3, netstat.ArrayDifferences1.Count());
            Assert.AreEqual(2, netstat.ArrayDifferences2.Count());

            FileDiffResult networkadapters = results.FirstOrDefault(f => f.Intent == "networkadapters");
            Assert.AreEqual(0, networkadapters.ArrayDifferences1.Count());
            Assert.AreEqual(2, networkadapters.ArrayDifferences2.Count());

            FileDiffResult processes = results.FirstOrDefault(f => f.Intent == "processes");
            Assert.AreEqual(11, processes.ArrayDifferences1.Count());
            Assert.AreEqual(8, processes.ArrayDifferences2.Count());

            FileDiffResult tlsconnections = results.FirstOrDefault(f => f.Intent == "tlsconnections");
            Assert.AreEqual(4, tlsconnections.ArrayDifferences1.Count());
            Assert.AreEqual(6, tlsconnections.ArrayDifferences2.Count());

            FileDiffResult uptime = results.FirstOrDefault(f => f.Intent == "uptime");
            Assert.IsFalse(uptime.ArrayDifferences1.Any());
            Assert.AreEqual(2, uptime.ObjectDifferences.Count());
            Assert.IsTrue(uptime.ObjectDifferences.Any(a => a.Key == "osResult.deviceOsBitness"));

            FileDiffResult networkdrives = results.FirstOrDefault(f => f.Intent == "networkdrives");
            Assert.IsNotNull(networkdrives);
            Assert.IsTrue(networkdrives.InError1);
            Assert.IsTrue(networkdrives.InError2);

        }


        [TestMethod]
        public void DiffFiles_Reversed()
        {
            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\c6a248e0-0f62-45f8-8803-1537adda4b15.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml2, yaml1);

            HashSet<string> expectedIntents = new HashSet<string>() { "cpu", "diskqueuelength", "drives", "ipconfig", "memory", "netstat",
                "networkadapters", "processes", "tlsconnections", "uptime", "networkdrives" };

            HashSet<string> actualIntents = new HashSet<string>(results.Select(s => s.Intent));

            actualIntents.SymmetricExceptWith(expectedIntents);
            Assert.IsFalse(actualIntents.Any(), "Intents list does not match");

            FileDiffResult cpuResult = results.FirstOrDefault(f => f.Intent == "cpu");
            Assert.AreEqual(6, cpuResult.ObjectDifferences.Count());

            FileDiffResult diskQResult = results.FirstOrDefault(f => f.Intent == "diskqueuelength");
            Assert.AreEqual(1, diskQResult.ObjectDifferences.Count());
            


            FileDiffResult drivesResult = results.FirstOrDefault(f => f.Intent == "drives");
            Assert.IsFalse(drivesResult.ArrayDifferences2.Any());
            Assert.AreEqual(1, drivesResult.ObjectDifferences.Count());

            FileDiffResult ipConfig = results.FirstOrDefault(f => f.Intent == "ipconfig");
            Assert.IsFalse(ipConfig.ArrayDifferences2.Any());
            Assert.AreEqual(1, ipConfig.ObjectDifferences.Count());

            FileDiffResult memory = results.FirstOrDefault(f => f.Intent == "memory");
            Assert.IsFalse(memory.ArrayDifferences2.Any());
            Assert.AreEqual(2, memory.ObjectDifferences.Count());

            FileDiffResult netstat = results.FirstOrDefault(f => f.Intent == "netstat");
            Assert.AreEqual(3, netstat.ArrayDifferences2.Count());
            Assert.AreEqual(2, netstat.ArrayDifferences1.Count());

            FileDiffResult networkadapters = results.FirstOrDefault(f => f.Intent == "networkadapters");
            Assert.AreEqual(0, networkadapters.ArrayDifferences2.Count());
            Assert.AreEqual(2, networkadapters.ArrayDifferences1.Count());

            FileDiffResult processes = results.FirstOrDefault(f => f.Intent == "processes");
            Assert.AreEqual(11, processes.ArrayDifferences2.Count());
            Assert.AreEqual(8, processes.ArrayDifferences1.Count());

            FileDiffResult tlsconnections = results.FirstOrDefault(f => f.Intent == "tlsconnections");
            Assert.AreEqual(4, tlsconnections.ArrayDifferences2.Count());
            Assert.AreEqual(6, tlsconnections.ArrayDifferences1.Count());

            FileDiffResult uptime = results.FirstOrDefault(f => f.Intent == "uptime");
            Assert.IsFalse(uptime.ArrayDifferences2.Any());
            Assert.AreEqual(2, uptime.ObjectDifferences.Count());
        }

        [TestMethod]
        public void DiffFiles_Idential()
        {
            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            testFixture.DiffFiles(yaml1, yaml1);
        }

        [TestMethod]
        public void DiffFiles_MissingIntent()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8_missing.json");
            string yaml2 = File.ReadAllText(@"Compare\c6a248e0-0f62-45f8-8803-1537adda4b15_missing.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            HashSet<string> expectedIntents = new HashSet<string>() { "os", "cpu", "diskqueuelength", "drives", "ipconfig", "memory", "netstat",
                "networkadapters", "processes", "tlsconnections", "uptime", "antivirus", "bsod", "networkdrives" };

            HashSet<string> actualIntents = new HashSet<string>(results.Select(s => s.Intent));

            actualIntents.SymmetricExceptWith(expectedIntents);
            Assert.IsFalse(actualIntents.Any(), "Intents list does not match");

            FileDiffResult cpuResult = results.FirstOrDefault(f => f.Intent == "cpu");
            Assert.AreEqual(6, cpuResult.ObjectDifferences.Count());

            FileDiffResult diskQResult = results.FirstOrDefault(f => f.Intent == "diskqueuelength");
            Assert.AreEqual(1, diskQResult.ObjectDifferences.Count());
            Assert.AreEqual("instances.averageQueueLength", diskQResult.ObjectDifferences.First().Key);

            FileDiffResult drivesResult = results.FirstOrDefault(f => f.Intent == "drives");
            Assert.IsFalse(drivesResult.ArrayDifferences1.Any());
            Assert.AreEqual(1, drivesResult.ObjectDifferences.Count());

            FileDiffResult ipConfig = results.FirstOrDefault(f => f.Intent == "ipconfig");
            Assert.IsFalse(ipConfig.ArrayDifferences1.Any());
            Assert.AreEqual(1, ipConfig.ObjectDifferences.Count());

            FileDiffResult memory = results.FirstOrDefault(f => f.Intent == "memory");
            Assert.IsFalse(memory.ArrayDifferences1.Any());
            Assert.AreEqual(2, memory.ObjectDifferences.Count());

            FileDiffResult netstat = results.FirstOrDefault(f => f.Intent == "netstat");
            Assert.AreEqual(3, netstat.ArrayDifferences1.Count());
            Assert.AreEqual(2, netstat.ArrayDifferences2.Count());

            FileDiffResult networkadapters = results.FirstOrDefault(f => f.Intent == "networkadapters");
            Assert.AreEqual(0, networkadapters.ArrayDifferences1.Count());
            Assert.AreEqual(2, networkadapters.ArrayDifferences2.Count());

            FileDiffResult processes = results.FirstOrDefault(f => f.Intent == "processes");
            Assert.AreEqual(11, processes.ArrayDifferences1.Count());
            Assert.AreEqual(8, processes.ArrayDifferences2.Count());

            FileDiffResult tlsconnections = results.FirstOrDefault(f => f.Intent == "tlsconnections");
            Assert.AreEqual(4, tlsconnections.ArrayDifferences1.Count());
            Assert.AreEqual(6, tlsconnections.ArrayDifferences2.Count());

            FileDiffResult uptime = results.FirstOrDefault(f => f.Intent == "uptime");
            Assert.IsFalse(uptime.ArrayDifferences1.Any());
            Assert.AreEqual(2, uptime.ObjectDifferences.Count());
            Assert.IsTrue(uptime.ObjectDifferences.Any(a => a.Key == "osResult.deviceOsBitness"));

            FileDiffResult antivirus = results.FirstOrDefault(f => f.Intent == "antivirus");
            Assert.IsNotNull(antivirus);
            Assert.AreEqual(7, antivirus.ObjectDifferences.Count());
            Assert.IsTrue(antivirus.ObjectDifferences.All(a => string.IsNullOrEmpty(a.Value2)));

            FileDiffResult bsod = results.FirstOrDefault(f => f.Intent == "bsod");
            Assert.IsNotNull(bsod);
            Assert.AreEqual(2, bsod.ObjectDifferences.Count());
            CompareLineResult record = bsod.ObjectDifferences.First(f => f.Key == "records");
            Assert.IsFalse(string.IsNullOrEmpty(record.Value1));

            FileDiffResult os = results.FirstOrDefault(f => f.Intent == "os");
            Assert.IsNotNull(os);
            Assert.AreEqual(7, os.ObjectDifferences.Count());
            Assert.IsTrue(os.ObjectDifferences.Any(a => !string.IsNullOrEmpty(a.Value1)));
        }

        [TestMethod]
        public void DiffFiles_MissingBegining()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8_missing_begining.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "activeports"));
        }

        [TestMethod]
        public void DiffFiles_MissingEnd()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8_missing_end.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "uptime"));
            Assert.AreEqual(12, results.First(f => f.Intent == "uptime").ObjectDifferences.Count());
            Assert.IsTrue(results.First(f => f.Intent == "uptime").ObjectDifferences.Any(a => !string.IsNullOrEmpty(a.Value1)));
        }

        [TestMethod]
        public void DiffFiles_MissingEnd_Flipped()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8_missing_end.json");
            string yaml2 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "uptime"));
            Assert.AreEqual(12, results.First(f => f.Intent == "uptime").ObjectDifferences.Count());
            Assert.IsTrue(results.First(f => f.Intent == "uptime").ObjectDifferences.Any(a => !string.IsNullOrEmpty(a.Value1)));
        }

        [TestMethod]
        public void DiffFiles_PLIPRODEV()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\cc5d46f7-186a-4b22-8f90-52a73f6733ad.json");
            string yaml2 = File.ReadAllText(@"Compare\6c5e8df1-dad5-4d46-961b-7b304ca2d54a.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

        }

        /// <summary>
        /// https://ivanti.visualstudio.com/Uno/_workitems/edit/781378
        /// </summary>
        [TestMethod]
#if !DEBUG
        [Timeout(3000)]
#endif
        public void DiffFiles_781378()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\dd9a5aa0-9bf4-438e-84e2-35656d18d67a.json");
            string yaml2 = File.ReadAllText(@"Compare\ccc7cf08-d08a-4ede-b95f-ba21b8a6390f.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "sessions"));
        }

        /// <summary>
        /// https://ivanti.visualstudio.com/Uno/_workitems/edit/781378
        /// </summary>
        [TestMethod]
        public void DiffFiles_781378_2()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\6d8e99b6-a7c4-4aea-a686-18d381e95049.json");
            string yaml2 = File.ReadAllText(@"Compare\1e787964-57b9-4c0d-80c9-4d62f238362c.json");

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "bsod"));
            Assert.AreEqual(8, results.First(f => f.Intent == "bsod").ObjectDifferences.Count());
            Assert.IsTrue(results.Any(a => a.Intent == "certs"));
            Assert.AreEqual(3, results.First(f => f.Intent == "certs").ArrayDifferences2.Count());
        }

        /// <summary>
        /// https://ivanti.visualstudio.com/Uno/_workitems/edit/791773
        /// </summary>
        [TestMethod]
        public void DiffFiles_791773()
        {


            var moqILogger = new Mock<ILogger<DiffEngine>>();

            string yaml1 = File.ReadAllText(@"Compare\407f2f3a-76af-4251-9b1a-35c161ff07d2.json");
            string yaml2 = File.ReadAllText(@"Compare\38f4e5c8-77f3-4dc4-b51c-7e86587eeeb5.json"); 

            DiffEngine testFixture = new DiffEngine(moqILogger.Object);

            IEnumerable<FileDiffResult> results = testFixture.DiffFiles(yaml1, yaml2);

            Assert.IsTrue(results.Any(a => a.Intent == "reboothistory"));
            FileDiffResult result = results.First(f => f.Intent == "reboothistory");

            Assert.AreEqual(8, result.ObjectDifferences.Count());
        }
    }
}
