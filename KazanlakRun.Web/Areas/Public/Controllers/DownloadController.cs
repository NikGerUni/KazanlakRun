using KazanlakRun.GCommon;
using KazanlakRun.Web.Areas.Public.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KazanlakRun.Web.Areas.Public.Controllers
{
    [Area("Public")]
    [Authorize(Roles = "Admin,User")]
    public class DownloadController : Controller
    {
        private readonly IGpxFileService _gpxFileService;
        private readonly GpxFileSettings _fileSettings;

        public DownloadController(
            IGpxFileService gpxFileService,
            IOptions<GpxFileSettings> fileSettings)
        {
            _gpxFileService = gpxFileService;
            _fileSettings = fileSettings.Value;
        }

        [HttpGet]
        public IActionResult DownloadGPX() => View();

        [HttpGet]
        public async Task<IActionResult> Download10kmGPX()
            => await DownloadFile(_fileSettings.File10kmId, "KazanlakRun10km.gpx");

        [HttpGet]
        public async Task<IActionResult> Download20kmGPX()
            => await DownloadFile(_fileSettings.File20kmId, "KazanlakRun20km.gpx");

        [HttpGet]
        public async Task<IActionResult> Download40kmGPX()
            => await DownloadFile(_fileSettings.File40kmId, "KazanlakRun40km.gpx");

        private async Task<IActionResult> DownloadFile(string fileId, string fileName)
        {
            try
            {
                var (stream, contentType) = await _gpxFileService.GetGpxFileAsync(fileId);
                return File(stream, contentType, fileName);
            }
            catch
            {
                return NotFound($"The file '{fileName}' could not be downloaded.");
            }
        }
    }
}
