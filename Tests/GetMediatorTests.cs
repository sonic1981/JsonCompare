using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core;
using Snapshot.Contract;
using Snapshot.DataLake;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot.Tests.Compare
{
    [TestClass]
    public class GetMediatorTests
    {
        [TestMethod]
        public async Task GetSnapShot_Yaml()
        {
            Guid tenantId = Guid.NewGuid();
            Guid resultId = Guid.NewGuid();


            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqILogger = new Mock<ILogger<GetMediator>>();

            moqIDataLakeReader.Setup(s => s.GetSnapshot(tenantId, resultId))
                .ReturnsAsync(@"{""results"":[""test1"",""test2""]}");

            GetMediator testFixture = new GetMediator(moqIDataLakeReader.Object, moqILogger.Object);

            
            StringValues stringValues = new StringValues("text/vnd.yaml");
            CallWrapperResponse result = await testFixture.GetSnapShot(tenantId, resultId, stringValues);

            Assert.AreEqual(@"results:
- test1
- test2
", result.Content);

            Assert.AreEqual("text/vnd.yaml", result.MediaType);
        }

        [TestMethod]
        public async Task GetSnapShot_Yaml_Date()
        {
            Guid tenantId = Guid.NewGuid();
            Guid resultId = Guid.NewGuid();


            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqILogger = new Mock<ILogger<GetMediator>>();

            moqIDataLakeReader.Setup(s => s.GetSnapshot(tenantId, resultId))
                .ReturnsAsync("{\"results\":{\"date\":\"2021-03-08\"}}");

            GetMediator testFixture = new GetMediator(moqIDataLakeReader.Object, moqILogger.Object);


            StringValues stringValues = new StringValues("text/vnd.yaml");
            CallWrapperResponse result = await testFixture.GetSnapShot(tenantId, resultId, stringValues);

            Assert.AreEqual(@"results:
  date: 2021-03-08
", result.Content);

            Assert.AreEqual("text/vnd.yaml", result.MediaType);
        }

        [TestMethod]
        public async Task GetSnapShot_Json()
        {
            Guid tenantId = Guid.NewGuid();
            Guid resultId = Guid.NewGuid();


            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqILogger = new Mock<ILogger<GetMediator>>();

            moqIDataLakeReader.Setup(s => s.GetSnapshot(tenantId, resultId))
                .ReturnsAsync(@"{""results"":[""test1"",""test2""]}");

            GetMediator testFixture = new GetMediator(moqIDataLakeReader.Object, moqILogger.Object);


            StringValues stringValues = new StringValues("application/json");
            CallWrapperResponse result = await testFixture.GetSnapShot(tenantId, resultId, stringValues);

            Assert.AreEqual(@"{""results"":[""test1"",""test2""]}", result.Content);

            Assert.AreEqual("application/json", result.MediaType);
        }

        [TestMethod]
        public async Task GetSnapShot_NoHeader()
        {
            Guid tenantId = Guid.NewGuid();
            Guid resultId = Guid.NewGuid();


            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqILogger = new Mock<ILogger<GetMediator>>();

            moqIDataLakeReader.Setup(s => s.GetSnapshot(tenantId, resultId))
                .ReturnsAsync(@"{""results"":[""test1"",""test2""]}");

            GetMediator testFixture = new GetMediator(moqIDataLakeReader.Object, moqILogger.Object);


            StringValues stringValues = StringValues.Empty;
            CallWrapperResponse result = await testFixture.GetSnapShot(tenantId, resultId, stringValues);

            Assert.AreEqual(@"{""results"":[""test1"",""test2""]}", result.Content);

            Assert.AreEqual("application/json", result.MediaType);
        }
    }
}
