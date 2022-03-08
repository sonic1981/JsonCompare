using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core.Differences.Readers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Tests.Compare.Differences.Compare
{
    [TestClass]
    public class LiteralStringReaderTests
    {
        [TestMethod]
        public void ReadLiteral()
        {
            string line = "  error: >+";
            var moqILineByLineReader = new Mock<ILineByLineReader>();
            moqILineByLineReader.SetupSequence(s => s.ReadLine())
                .Returns("    Failed executing query 'adusergroups': The server is not operational.")
                .Returns("")
                .Returns("antivirus:");

            LiteralStringReader.ReadLiteral(ref line, moqILineByLineReader.Object);

            Assert.AreEqual("antivirus:", line);
        }
    }
}
