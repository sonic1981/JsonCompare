using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core.Differences.Compare;
using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snapshot.Tests.Compare.Differences.Compare
{
    [TestClass]
    public class LineByLineCompareTests
    {
        [TestMethod]
        public void Compare()
        {
            string line1 = "  instances:";
            string line2 = "  instances:";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("  - averageQueueLength: 0.0034670703476620043")
                .Returns("    busType: 10")
                .Returns("    physicalDrive: '0 C:'")
                .Returns("dotnetframework:");

            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("  - averageQueueLength: 0.30161898043125868")
                .Returns("    busType: 10")
                .Returns("    physicalDrive: '0 C:'")
                .Returns("dotnetframework:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "diskqueuelength"
            };

            LineByLineCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            Assert.AreEqual("instances.averageQueueLength", result.ObjectDifferences.First().Key);
        }

        [TestMethod]
        public void Compare_781378()
        {
            string line1 = "  records: []";
            string line2 = "  records: []";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("  osResult:")
                .Returns("    deviceOsName: Windows 10 Enterprise")
                .Returns("    deviceOsBitness: 64")
                .Returns("    deviceOsPlatform: Win32NT")
                .Returns("    deviceOsServicePack: ''")
                .Returns("    deviceOsVersion: 10.0.18363")
                .Returns("    deviceOsReleaseId: 1909")
                .Returns("    isServer: false")
                .Returns("certs:");

            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("  osResult: ")
                .Returns("certs:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "diskqueuelength"
            };

            LineByLineCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            moqILineByLineReader2.Verify(v => v.ReadLine(), Times.Exactly(2));

            Assert.AreEqual(8, result.ObjectDifferences.Count());
            Assert.IsTrue(result.ObjectDifferences.All(a => string.IsNullOrEmpty(a.Value2)));
            Assert.AreEqual("certs:", line1);
            Assert.AreEqual("certs:", line2);
        }

        [TestMethod]
        public void Compare_781378_inverted()
        {
            string line1 = "  records: []";
            string line2 = "  records: []";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("  osResult: ")
                .Returns("certs:");

            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("  osResult:")
                .Returns("    deviceOsName: Windows 10 Enterprise")
                .Returns("    deviceOsBitness: 64")
                .Returns("    deviceOsPlatform: Win32NT")
                .Returns("    deviceOsServicePack: ''")
                .Returns("    deviceOsVersion: 10.0.18363")
                .Returns("    deviceOsReleaseId: 1909")
                .Returns("    isServer: false")
                .Returns("certs:");

            

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "diskqueuelength"
            };

            LineByLineCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            moqILineByLineReader2.Verify(v => v.ReadLine(), Times.Exactly(9));

            Assert.AreEqual(8, result.ObjectDifferences.Count());
            Assert.IsTrue(result.ObjectDifferences.All(a => string.IsNullOrEmpty(a.Value1)));
            Assert.AreEqual("certs:", line1);
            Assert.AreEqual("certs:", line2);
        }
    }
}
