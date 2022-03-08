using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class UnmatchedIntentCompareTests
    {
        [TestMethod]
        public void Compare()
        {
            string line1 = "os:";
            string line2 = "patches:";

            var moqILineByLineReader1 = new Mock<TextReader>();
            var moqILineByLineReader2 = new Mock<TextReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("- platform: Win32NT")
                .Returns("  displayName: Windows 10 Enterprise")
                .Returns("  version: 10.0")
                .Returns("  versionAndBuild: 10.0.17134")
                .Returns("  servicePack: ''")
                .Returns("  releaseId: 1803")
                .Returns("  bitness: 64")
                .Returns("patches:")
                .Returns("- found: true");

            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("- found: true")
                .Returns("  patches:")
                .Returns("  - name: KB4100347")
                .Returns("    version: ''")
                .Returns("    publisher: ''")
                .Returns("    installDate: 2018-12-10T00:00:00")
                .Returns("  - name: KB4230204")
                .Returns("    version: ''");

            HybridLineByLineReader rd1 = new HybridLineByLineReader(moqILineByLineReader1.Object);
            HybridLineByLineReader rd2 = new HybridLineByLineReader(moqILineByLineReader2.Object);


            FileDiffResult result = UnmatchedIntentCompare.Compare(ref line1, ref line2,
                rd1, rd2);

            Assert.AreEqual(7, result.ObjectDifferences.Count());
            Assert.AreEqual("os", result.Intent);

            Assert.AreEqual("patches:", line1);
            Assert.AreEqual("- found: true", rd1.ReadLine());

            Assert.AreEqual("patches:", line2);
            Assert.AreEqual("- found: true", rd2.ReadLine());

        }

        [TestMethod]
        public void Compare_EmptyIntent()
        {
            string line1 = "sessions: []";
            string line2 = "sessions:";

            var moqILineByLineReader1 = new Mock<TextReader>();
            var moqILineByLineReader2 = new Mock<TextReader>();

            moqILineByLineReader1.SetupSequence(s => s.ReadLine())
                .Returns("smartdiskstatus:")
                .Returns("- index: 0")
                .Returns("  model: Microsoft Virtual Disk")
                .Returns("  type: Unspecified")
                .Returns("  interface: SCSI")
                .Returns("  serialNumber: '-'")
                .Returns("  status: OK")
                .Returns("  category: OK")
                .Returns("softwareinstallations: []");

            moqILineByLineReader2.SetupSequence(s => s.ReadLine())
                .Returns("- sessionId: 2")
                .Returns("  patches:")
                .Returns(@"  userName: LD\Liam.Hughes")
                .Returns("  sessionStartTime: 2021-01-18T11:17:59.8250000Z")
                .Returns("  logonTime: 2021-01-18T11:18:04.0270000Z")
                .Returns("  idleTimeMs: 7500")
                .Returns("  sessionState: Active")
                .Returns("  platform: PC")
                .Returns("  nodeType: Virtual (Microsoft Corporation)")
                .Returns("  protocol: RDP")
                .Returns("  osType: Workstation")
                .Returns("  isScreensaverRunning: false")
                .Returns(@"  foregroundAppProcessPath: C:\WINDOWS\system32\taskmgr.exe")
                .Returns("smartdiskstatus:");

            HybridLineByLineReader rd1 = new HybridLineByLineReader(moqILineByLineReader1.Object);
            HybridLineByLineReader rd2 = new HybridLineByLineReader(moqILineByLineReader2.Object);


            FileDiffResult result = UnmatchedIntentCompare.Compare(ref line1, ref line2,
                rd1, rd2);

            Assert.AreEqual(13, result.ObjectDifferences.Count());
            Assert.AreEqual("sessions", result.Intent);

            Assert.AreEqual("smartdiskstatus:", line1);

            Assert.AreEqual("smartdiskstatus:", line2);

        }
    }
}
