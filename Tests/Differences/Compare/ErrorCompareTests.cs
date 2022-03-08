using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core.Differences.Compare;
using Snapshot.Compare.Core.Differences.Readers;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Tests.Compare.Differences.Compare
{
    [TestClass]
    public class ErrorCompareTests
    {
        [TestMethod]
        public void Compare()
        {
            string line1 = "  error: User name not specified";
            string line2 = "  error: User name not specified";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.Setup(s => s.ReadLine()).Returns("os:");
            moqILineByLineReader2.Setup(s => s.ReadLine()).Returns("os:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "networkdrives"
            };

            ErrorCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            Assert.IsTrue(result.InError1);
            Assert.IsTrue(result.InError2);
        }

        [TestMethod]
        public void Compare_oneSided()
        {
            string line1 = "- platform: Win32NT";
            string line2 = "  error: User name not specified";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("  displayName: Windows 10 Enterprise")
                .Returns("  version: 10.0")
                .Returns("  versionAndBuild: 10.0.17134")
                .Returns("  servicePack: ''")
                .Returns("  releaseId: 1803")
                .Returns("  bitness: 64")
                .Returns("patches:");


            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("patches:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "os"
            };

            ErrorCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            Assert.IsFalse(result.InError1);
            Assert.IsTrue(result.InError2);
            

            Assert.AreEqual("patches:", line1);
            Assert.AreEqual("patches:", line2);
        }

        [TestMethod]
        public void Compare_blockScaler()
        {
            string line1 = "  error: >+";
            string line2 = "  error: >+";

            var moqILineByLineReader1 = new Mock<ILineByLineReader>();
            var moqILineByLineReader2 = new Mock<ILineByLineReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("    Failed executing query 'adusergroups': The server is not operational.")
                .Returns("")
                .Returns("antivirus:");


            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("    Failed executing query 'adusergroups': The server is not operational.")
                .Returns("")
                .Returns("antivirus:");

            FileDiffResult result = new FileDiffResult()
            {
                Intent = "adusergroups"
            };

            ErrorCompare.Compare(ref line1, ref line2, moqILineByLineReader1.Object, moqILineByLineReader2.Object, result);

            Assert.IsTrue(result.InError1);
            Assert.IsTrue(result.InError2);

            Assert.AreEqual("antivirus:", line1);
            Assert.AreEqual("antivirus:", line2);
        }
    }
}
