using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snapshot.Compare.Core;
using Snapshot.Compare.Core.Differences;
using Snapshot.Contract;
using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Snapshot.Auth;
using Snapshot.Tests;
using Snapshot.DataLake;

namespace Snapshot.Tests.Compare
{
    [TestClass]
    public class CompareMediatorTests
    {
        [TestMethod]
        public async Task PerformCompare()
        {
            Guid request1 = new Guid("a986ac61-b648-48e6-a061-a18274f8a215");
            Guid request2 = new Guid("3e08792c-5603-4dbb-a974-2ce868ab4ed1");

            var moqILogger = new Mock<ILogger<CompareMediator>>();
            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqIDiffEngine = new Mock<IDiffEngine>();

            CompareMediator testFixture = new CompareMediator(moqILogger.Object, moqIDataLakeReader.Object, moqIDiffEngine.Object);

            JWTDecoder jWTDecoder = new JWTDecoder(Constants.AccessToken);

            CompareRequest request = new CompareRequest()
            {
                Result1 = request1,
                Result2 = request2
            };

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\c6a248e0-0f62-45f8-8803-1537adda4b15.json");

            moqIDataLakeReader.Setup(s => s.GetSnapshot(Constants.TenantId, request1))
                .ReturnsAsync(yaml1);

            moqIDataLakeReader.Setup(s => s.GetSnapshot(Constants.TenantId, request2))
                .ReturnsAsync(yaml2);

            IEnumerable<FileDiffResult> results = await testFixture.PerformCompare(request, jWTDecoder);

            moqIDataLakeReader.Verify(v => v.GetSnapshot(Constants.TenantId, It.IsAny<Guid>()), Times.Exactly(2));

            moqIDiffEngine.Verify(v => v.DiffFiles(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task PerformCompare_Exception()
        {
            Guid request1 = new Guid("a986ac61-b648-48e6-a061-a18274f8a215");
            Guid request2 = new Guid("3e08792c-5603-4dbb-a974-2ce868ab4ed1");

            var moqILogger = new Mock<ILogger<CompareMediator>>();
            var moqIDataLakeReader = new Mock<IDataLakeReader>();
            var moqIDiffEngine = new Mock<IDiffEngine>();

            CompareMediator testFixture = new CompareMediator(moqILogger.Object, moqIDataLakeReader.Object, moqIDiffEngine.Object);

            JWTDecoder jWTDecoder = new JWTDecoder(Constants.AccessToken);

            CompareRequest request = new CompareRequest()
            {
                Result1 = request1,
                Result2 = request2
            };

            string yaml1 = File.ReadAllText(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json");
            string yaml2 = File.ReadAllText(@"Compare\c6a248e0-0f62-45f8-8803-1537adda4b15.json");

            moqIDataLakeReader.Setup(s => s.GetSnapshot(Constants.TenantId, request1))
                .ReturnsAsync(yaml1);

            moqIDataLakeReader.Setup(s => s.GetSnapshot(Constants.TenantId, request2))
                .ReturnsAsync(yaml2);

            moqIDiffEngine.Setup(s => s.DiffFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            await Assert.ThrowsExceptionAsync<Exception>(() => testFixture.PerformCompare(request, jWTDecoder));

            moqIDataLakeReader.Verify(v => v.GetSnapshot(Constants.TenantId, It.IsAny<Guid>()), Times.Exactly(2));

            moqIDiffEngine.Verify(v => v.DiffFiles(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


    }
}
