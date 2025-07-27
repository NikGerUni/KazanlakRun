using KazanlakRun.GCommon;
using KazanlakRun.Web.Controllers;
using KazanlakRun.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace KazanlakRun.Web.Tests.Controllers
{
    [TestFixture]
    public class DownloadControllerTests
    {
        private Mock<IGpxFileService> _serviceMock;
        private IOptions<GpxFileSettings> _options;
        private DownloadController _controller;

        private readonly GpxFileSettings _fileSettings = new()
        {
            File10kmId = "10-id",
            File20kmId = "20-id",
            File40kmId = "40-id"
        };

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IGpxFileService>();
            _options = Options.Create(_fileSettings);
            _controller = new DownloadController(_serviceMock.Object, _options);
        }
        [TearDown]
        public void TearDown()
        {
            // direct Dispose() call satisfies the analyzer
            _controller.Dispose();
            _controller = null!;
        }

        [Test]
        public void DownloadGPX_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.DownloadGPX();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var vr = (ViewResult)result;
            Assert.IsNull(vr.ViewName, "По подразбиране ViewResult.ViewName трябва да е null (използва подразбиращото се View).");
        }

        [Test]
        public async Task Download10kmGPX_OnSuccess_ReturnsFileStreamResult()
        {
            // Arrange
            var data = new byte[] { 9, 8, 7 };
            var ms = new MemoryStream(data);
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File10kmId))
                .ReturnsAsync((Stream: (Stream)ms, ContentType: "application/gpx+xml"));

            // Act
            var result = await _controller.Download10kmGPX();

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fsr = (FileStreamResult)result;
            Assert.AreEqual("application/gpx+xml", fsr.ContentType);
            Assert.AreEqual("KazanlakRun10km.gpx", fsr.FileDownloadName);

            // Претвърди съдържанието
            using var reader = new MemoryStream();
            await fsr.FileStream.CopyToAsync(reader);
            CollectionAssert.AreEqual(data, reader.ToArray());
        }

        [Test]
        public async Task Download20kmGPX_OnServiceThrows_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File20kmId))
                .ThrowsAsync(new InvalidOperationException("fail"));

            // Act
            var result = await _controller.Download20kmGPX();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = (NotFoundObjectResult)result;
            StringAssert.Contains("не можа да бъде свален", notFound.Value?.ToString()!);
        }

        [Test]
        public async Task Download40kmGPX_OnSuccess_ReturnsCorrectFileName()
        {
            // Arrange
            var dummy = new MemoryStream(new byte[] { 1 });
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File40kmId))
                .ReturnsAsync((dummy as Stream, "application/gpx+xml"));

            // Act
            var result = await _controller.Download40kmGPX();

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fsr = (FileStreamResult)result;
            Assert.AreEqual("KazanlakRun40km.gpx", fsr.FileDownloadName);
        }
    }
}
