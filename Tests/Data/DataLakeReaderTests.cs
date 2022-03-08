using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Snapshot.Common.Exceptions;
using Snapshot.DataLake;
using Snapshot.DataLake.Extensions;

namespace Snapshot.Tests.Compares.Data
{
    [TestClass]
    public class DataLakeReaderTests
    {
        [TestMethod]
        public async Task GetSnapshot()
        {
            Guid tenantId = new Guid("54153fe9-2bf8-4991-bb6e-88d729741ee4");
            Guid resultId = new Guid("9fb5ef8b-bb7c-4b27-a18d-185abccb0079");


            var moqILogger = new Mock<ILogger<DataLakeReader>>();
            var moqIDataLakeServiceFactory = new Mock<IDataLakeServiceFactory>();
            var moqIEnvironmentVariables = new Mock<IDataLakeEnvironmentVariables>();

            moqIEnvironmentVariables.SetupGet(s => s.DataLakeContainerName).Returns("snapshots");
            moqIEnvironmentVariables.SetupGet(s => s.DataLakeName).Returns("snapshotdevdatalake1");
            moqIEnvironmentVariables.SetupGet(s => s.DataLakeKey).Returns("LcT1X4/T7rLHVV95eY+F3FX5cvjX8ce13Ol+AvioEAz6HwwYAhcjfXUaVo9qGhmgn1klMFb5h5+9HE2f6Zxp9Q==");

            var moqDataLakeFileClient = new Mock<DataLakeFileClient>();

            moqIDataLakeServiceFactory.Setup(s => s.GetFileClient("snapshotdevdatalake1",
                "LcT1X4/T7rLHVV95eY+F3FX5cvjX8ce13Ol+AvioEAz6HwwYAhcjfXUaVo9qGhmgn1klMFb5h5+9HE2f6Zxp9Q==",
                "snapshots", "54153fe9-2bf8-4991-bb6e-88d729741ee4", "9fb5ef8b-bb7c-4b27-a18d-185abccb0079.json"))
                    .ReturnsAsync(moqDataLakeFileClient.Object);


            var moqExistsResponse = new Mock<Response<bool>>();
            moqExistsResponse.SetupGet(s => s.Value).Returns(true);

            moqDataLakeFileClient.Setup(s => s.Exists(It.IsAny<CancellationToken>()))
                .Returns(moqExistsResponse.Object);


            using (FileStream fs = new FileStream(@"Compare\23a19953-a0d0-41ed-8616-0adf4a041aa8.json", FileMode.Open))
            {

                moqDataLakeFileClient.Setup(s => s.OpenReadAsync(It.IsAny<long>(), It.IsAny<int?>(), It.IsAny<DataLakeRequestConditions>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(fs);


                DataLakeReader testFixture = new DataLakeReader(moqILogger.Object, moqIDataLakeServiceFactory.Object, moqIEnvironmentVariables.Object);

                string result = await testFixture.GetSnapshot(tenantId, resultId);

                Assert.IsFalse(string.IsNullOrEmpty(result));
            }

            moqDataLakeFileClient.Verify(v => v.Exists(It.IsAny<CancellationToken>()), Times.Once);

        }

        [TestMethod]
        public async Task GetSnapshot_FileDoesNotExist()
        {
            Guid tenantId = new Guid("54153fe9-2bf8-4991-bb6e-88d729741ee4");
            Guid resultId = new Guid("9fb5ef8b-bb7c-4b27-a18d-185abccb0079");


            var moqILogger = new Mock<ILogger<DataLakeReader>>();
            var moqIDataLakeServiceFactory = new Mock<IDataLakeServiceFactory>();
            var moqIEnvironmentVariables = new Mock<IDataLakeEnvironmentVariables>();

            moqIEnvironmentVariables.SetupGet(s => s.DataLakeContainerName).Returns("snapshots");
            moqIEnvironmentVariables.SetupGet(s => s.DataLakeName).Returns("snapshotdevdatalake1");
            moqIEnvironmentVariables.SetupGet(s => s.DataLakeKey).Returns("LcT1X4/T7rLHVV95eY+F3FX5cvjX8ce13Ol+AvioEAz6HwwYAhcjfXUaVo9qGhmgn1klMFb5h5+9HE2f6Zxp9Q==");

            var moqDataLakeFileClient = new Mock<DataLakeFileClient>();
            var moqDataLakeLeaseClient = new Mock<DataLakeLeaseClient>();

            moqIDataLakeServiceFactory.Setup(s => s.GetFileClient("snapshotdevdatalake1",
                "LcT1X4/T7rLHVV95eY+F3FX5cvjX8ce13Ol+AvioEAz6HwwYAhcjfXUaVo9qGhmgn1klMFb5h5+9HE2f6Zxp9Q==",
                "snapshots", "54153fe9-2bf8-4991-bb6e-88d729741ee4", "9fb5ef8b-bb7c-4b27-a18d-185abccb0079.json"))
                    .ReturnsAsync(moqDataLakeFileClient.Object);


            var moqExistsResponse = new Mock<Response<bool>>();
            moqExistsResponse.SetupGet(s => s.Value).Returns(false);

            moqDataLakeFileClient.Setup(s => s.Exists(It.IsAny<CancellationToken>()))
                .Returns(moqExistsResponse.Object);

            moqIDataLakeServiceFactory.Setup(s => s.GetDataLakeLeaseClient(It.IsAny<DataLakeFileClient>()))
                .Returns(moqDataLakeLeaseClient.Object);



                DataLakeReader testFixture = new DataLakeReader(moqILogger.Object, moqIDataLakeServiceFactory.Object, moqIEnvironmentVariables.Object);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => testFixture.GetSnapshot(tenantId, resultId));


            moqDataLakeFileClient.Verify(v => v.Exists(It.IsAny<CancellationToken>()), Times.Once);
            moqDataLakeLeaseClient.Verify(v => v.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()), Times.Never);
            moqDataLakeLeaseClient.Verify(v => v.ReleaseAsync(It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
