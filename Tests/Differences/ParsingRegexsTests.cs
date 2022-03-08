using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snapshot.Compare.Core.Differences;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Snapshot.Tests.Compare.Differences
{
    [TestClass]
    public class ParsingRegexsTests
    {
        [TestMethod]
        public void ResultValueRegex()
        {

            Match m = ParsingRegexs.ValueRegex.Match("  - averageQueueLength: 0.0034670703476620043");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("averageQueueLength", m.Groups[1].Value);
            Assert.AreEqual("0.0034670703476620043", m.Groups[3].Value);

            m = ParsingRegexs.ValueRegex.Match("  processcpu: 7");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("processcpu", m.Groups[1].Value);
            Assert.AreEqual("7", m.Groups[3].Value);

            m = ParsingRegexs.ValueRegex.Match("- processid: 5848");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("processid", m.Groups[1].Value);
            Assert.AreEqual("5848", m.Groups[3].Value);

            m = ParsingRegexs.ValueRegex.Match("    remoteAddress: ");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("remoteAddress", m.Groups[1].Value);
            Assert.AreEqual(string.Empty, m.Groups[3].Value);

            m = ParsingRegexs.ValueRegex.Match("  adminMembers:");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("adminMembers", m.Groups[1].Value);
            Assert.AreEqual(string.Empty, m.Groups[3].Value);

            m = ParsingRegexs.ValueRegex.Match("    averageQueueLength: 0.30161898043125868");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("averageQueueLength", m.Groups[1].Value);
            Assert.AreEqual("0.30161898043125868", m.Groups[3].Value);
        }

        [TestMethod]
        public void ArrayRegex()
        {
            Match m = ParsingRegexs.ArrayRegex.Match("  - protocol: TCP");
            Assert.IsTrue(m.Success);

            m = ParsingRegexs.ArrayRegex.Match("- totalPhysical: 3220168704");
            Assert.IsTrue(m.Success);

            m = ParsingRegexs.ArrayRegex.Match("- computerInfo:");
            Assert.IsTrue(m.Success);

            m = ParsingRegexs.ArrayRegex.Match("  - averageQueueLength: 0.0034670703476620043");
            Assert.IsTrue(m.Success);
        }


        [TestMethod]
        public void IntentRegex()
        {
            Match m = ParsingRegexs.IntentRegex.Match("firewall:");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("firewall", m.Groups[1].Value);

            m = ParsingRegexs.IntentRegex.Match("failedlogons: []");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("failedlogons", m.Groups[1].Value);
            //https://ivanti.visualstudio.com/Uno/_workitems/edit/781378
            Assert.IsTrue(m.Groups[2].Success);
        }


        [TestMethod]
        public void ObjectProperty()
        {
            Match m = ParsingRegexs.ObjectProperty.Match("  vendor: Microsoft corporation");
            Assert.IsTrue(m.Success);

            m = ParsingRegexs.ObjectProperty.Match("  osResult: ");
            Assert.IsTrue(m.Success);

            m = ParsingRegexs.ObjectProperty.Match("    deviceOsName: Windows 10 Enterprise");
            Assert.IsTrue(m.Success);


            m = ParsingRegexs.ObjectProperty.Match("- processid: 5848");
            Assert.IsFalse(m.Success);
        }

        [TestMethod]
        public void NestingRegex()
        {
            Assert.AreEqual(1, ParsingRegexs.NestingRegex.Matches("  instances:").Count);
            Assert.AreEqual(2, ParsingRegexs.NestingRegex.Matches("    averageQueueLength: 0.0034670703476620043").Count);

        }

        [TestMethod]
        public void IsErrorLine()
        {
            
            Assert.IsTrue(ParsingRegexs.IsErrorLine.IsMatch("  error: User name not specified"));
            Assert.IsFalse(ParsingRegexs.IsErrorLine.IsMatch("applicationerrors:"));
            Assert.IsTrue(ParsingRegexs.IsErrorLine.IsMatch("  error: >+"));

        }

        [TestMethod]
        public void IsStringLiteral()
        {

            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: >+"));
            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: >-"));
            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: >"));
            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: |"));
            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: |+"));
            Assert.IsTrue(ParsingRegexs.IsStringLiteral.IsMatch("  error: |-"));
            Assert.IsFalse(ParsingRegexs.IsStringLiteral.IsMatch("  error: test <te>"));

        }

        [TestMethod]
        public void EmptyArrayProperty()
        {
            Assert.IsTrue(ParsingRegexs.EmptyArrayProperty.IsMatch("  records: []"));
        }

    }
}
