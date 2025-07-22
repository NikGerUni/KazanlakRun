using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DownloadController : Controller
    {
        private readonly DriveService _driveService;

        // ID-та на конкретните файлове в Google Drive (вместо папка)
        private const string File10kmId = "1pxsaBBU0M5_9I3Bun8xw32jfRwanzWVe";
        private const string File20kmId = "1eyfjuOsrE5yQ-3s0nbPr6HP8dWEErpvQ";
        private const string File40kmId = "1m6pAk-_XM968rKjsi2I_2IaUjcdI9nFs";

        // Алтернативно: ако искаш да използваш папката, промени ID-то на реалната папка
        private const string FolderId = "18a-0sHlc3JD6HcxZzrDWnUdBUXRFVvYG";

        public DownloadController(DriveService driveService)
            => _driveService = driveService;

        // GET: /Download/DownloadGPX
        [HttpGet]
        public IActionResult DownloadGPX()
            => View();

        // Вариант 1: Използване на директни файлови ID-та (препоръчително)
        [HttpGet]
        public async Task<IActionResult> Download10kmGPX()
            => await DownloadByFileIdAsync(File10kmId, "KazanlakRun10km.gpx");

        [HttpGet]
        public async Task<IActionResult> Download20kmGPX()
            => await DownloadByFileIdAsync(File20kmId, "KazanlakRun20km.gpx");

        [HttpGet]
        public async Task<IActionResult> Download40kmGPX()
            => await DownloadByFileIdAsync(File40kmId, "KazanlakRun40km.gpx");

        // Метод за сваляне по файлово ID (най-бърз и надежден)
        private async Task<IActionResult> DownloadByFileIdAsync(string fileId, string fileName)
        {
            try
            {
                var request = _driveService.Files.Get(fileId);
                var stream = new MemoryStream();
                await request.DownloadAsync(stream);
                stream.Position = 0;

                return File(stream, "application/gpx+xml", fileName);
            }
            catch (Google.GoogleApiException ex)
            {
                return NotFound($"Файлът '{fileName}' не може да бъде свален: {ex.Message}");
            }
        }


    }
}