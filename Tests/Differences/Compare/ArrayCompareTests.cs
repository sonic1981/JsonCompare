using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core.Differences;
using Snapshot.Compare.Core.Differences.Compare;
using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snapshot.Tests.Compare.Differences.Compare
{
    [TestClass]
    public class ArrayCompareTests
    {
        [TestMethod]
        public void Compare_OneProperty()
        {
            string line1 = "  - averageQueueLength: 0.0034670703476620043";
            string line2 = "  - averageQueueLength: 0.30161898043125868";

            var moqTextReader1 = new Mock<TextReader>();
            var moqTextReader2 = new Mock<TextReader>();

            moqTextReader1.SetupSequence(s => s.ReadLine())
                .Returns("    busType: 10")
                .Returns("    physicalDrive: '0 C:'")
                .Returns("dotnetframework:")
                .Returns("- maxVersion: 4.7.2");

            moqTextReader2.SetupSequence(s => s.ReadLine())
                .Returns("    busType: 10")
                .Returns("    physicalDrive: '0 C:'")
                .Returns("dotnetframework:")
                .Returns("- maxVersion: 4.7.2");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "diskqueuelength"
            };

            ArrayCompare.Compare(ref line1, ref line2, new TextReaderLineByLineReader(moqTextReader1.Object), new TextReaderLineByLineReader(moqTextReader2.Object), result);

            moqTextReader1.Verify(ValueTuple => ValueTuple.ReadLine(), Times.Exactly(3));
            moqTextReader2.Verify(ValueTuple => ValueTuple.ReadLine(), Times.Exactly(3));

            //even though one line were flagging each array item as being totally different
            Assert.AreEqual(1, result.ObjectDifferences.Count());
            Assert.AreEqual("averageQueueLength", result.ObjectDifferences.First().Key);
            Assert.AreEqual("0.0034670703476620043", result.ObjectDifferences.First().Value1);
            Assert.AreEqual("0.30161898043125868", result.ObjectDifferences.First().Value2);

            Assert.IsFalse(result.ArrayDifferences1.Any());
        }

        [TestMethod]
        public void Compare_ArrayItems()
        {
            string line1 = "  - protocol: TCP";
            string line2 = "  - protocol: TCP";

            var moqTextReader1 = new Mock<TextReader>();
            var moqTextReader2 = new Mock<TextReader>();

            moqTextReader1.SetupSequence(s => s.ReadLine())
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 62796")
                        .Returns("    remoteAddress: 40.67.254.36")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 443")
                        .Returns("    remotePortIanaServiceName: https")
                        .Returns("    state: ESTABLISHED")
                        .Returns("  - protocol: TCP")
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 63039")
                        .Returns("    remoteAddress: 10.38.24.15")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 135")
                        .Returns("    remotePortIanaServiceName: epmap")
                        .Returns("    state: TIME_WAIT")
                        .Returns("  - protocol: TCP")
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 63040")
                        .Returns("    remoteAddress: 10.38.24.15")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 49671")
                        .Returns("    remotePortIanaServiceName:")
                        .Returns("    state: TIME_WAIT")
                        .Returns("  - protocol: TCP")
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 63041")
                        .Returns("    remoteAddress: 10.38.24.15")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 49671")
                        .Returns("    remotePortIanaServiceName:")
                        .Returns("    state: TIME_WAIT")
                        .Returns("networkadapters:");

            moqTextReader2.SetupSequence(s => s.ReadLine())
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 62796")
                        .Returns("    remoteAddress: 40.67.254.36")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 443")
                        .Returns("    remotePortIanaServiceName: https")
                        .Returns("    state: ESTABLISHED")
                        .Returns("  - protocol: TCP")
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 63166")
                        .Returns("    remoteAddress: 10.38.24.15")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 135")
                        .Returns("    remotePortIanaServiceName: epmap")
                        .Returns("    state: TIME_WAIT")
                        .Returns("  - protocol: TCP")
                        .Returns("    localAddress: 10.38.24.240")
                        .Returns("    localPort: 63167")
                        .Returns("    remoteAddress: 10.38.24.15")
                        .Returns("    remoteHostName:")
                        .Returns("    remotePort: 49671")
                        .Returns("    remotePortIanaServiceName:")
                        .Returns("    state: TIME_WAIT")
                        .Returns("networkadapters:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "diskqueuelength"
            };

            ArrayCompare.Compare(ref line1, ref line2, new TextReaderLineByLineReader(moqTextReader1.Object), new TextReaderLineByLineReader(moqTextReader2.Object), result);

            
            Assert.IsFalse(result.ArrayDifferences1.Any(a => a.Values.Any(a => a.Key == "localPort" && a.Value == "62796")));
            Assert.IsFalse(result.ArrayDifferences2.Any(a => a.Values.Any(a => a.Key == "localPort" && a.Value == "62796")));

            //Assert.IsFalse(result.ArrayDifferences1.Any(a => a.ArrayItems1.Any(a => a.Value == "62796" || a.Value == "62796")));

            Assert.AreEqual(3, result.ArrayDifferences1.Count());
            Assert.AreEqual(2, result.ArrayDifferences2.Count());
        }
    }
}
