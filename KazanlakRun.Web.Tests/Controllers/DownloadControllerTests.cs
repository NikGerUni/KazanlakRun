using KazanlakRun.GCommon;
using KazanlakRun.Web.Areas.Public.Controllers;
using KazanlakRun.Web.Areas.Public.Services.IServices;
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
            _controller.Dispose();
            _controller = null!;
        }

        [Test]
        public void DownloadGPX_ShouldReturnViewResult()
        {
            var result = _controller.DownloadGPX();

            Assert.IsInstanceOf<ViewResult>(result);
            var vr = (ViewResult)result;
            Assert.IsNull(vr.ViewName, "By default, ViewResult.ViewName should be null (uses default view).");
        }

        [Test]
        public async Task Download10kmGPX_OnSuccess_ReturnsFileStreamResult()
        {
            var data = new byte[] { 9, 8, 7 };
            var ms = new MemoryStream(data);
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File10kmId))
                .ReturnsAsync((Stream: (Stream)ms, ContentType: "application/gpx+xml"));

            var result = await _controller.Download10kmGPX();

            Assert.IsInstanceOf<FileStreamResult>(result);
            var fsr = (FileStreamResult)result;
            Assert.AreEqual("application/gpx+xml", fsr.ContentType);
            Assert.AreEqual("KazanlakRun10km.gpx", fsr.FileDownloadName);

            using var reader = new MemoryStream();
            await fsr.FileStream.CopyToAsync(reader);
            CollectionAssert.AreEqual(data, reader.ToArray());
        }

        [Test]
        public async Task Download20kmGPX_OnServiceThrows_ReturnsNotFoundObjectResult()
        {
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File20kmId))
                .ThrowsAsync(new InvalidOperationException("fail"));

            var result = await _controller.Download20kmGPX();

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = (NotFoundObjectResult)result;
            StringAssert.Contains("could not be downloaded", notFound.Value?.ToString()!);
        }

        [Test]
        public async Task Download40kmGPX_OnSuccess_ReturnsCorrectFileName()
        {
            var dummy = new MemoryStream(new byte[] { 1 });
            _serviceMock
                .Setup(s => s.GetGpxFileAsync(_fileSettings.File40kmId))
                .ReturnsAsync((dummy as Stream, "application/gpx+xml"));

            var result = await _controller.Download40kmGPX();

            Assert.IsInstanceOf<FileStreamResult>(result);
            var fsr = (FileStreamResult)result;
            Assert.AreEqual("KazanlakRun40km.gpx", fsr.FileDownloadName);
        }
    }
}
