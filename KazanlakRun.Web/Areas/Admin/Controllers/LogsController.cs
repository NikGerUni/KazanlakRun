using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        private readonly string _logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

        public IActionResult Index(string? searchTerm, int page = 1, int pageSize = 10)
        {
            if (!Directory.Exists(_logDir))
                return View(new LogFilesPageViewModel());

            var files = Directory.GetFiles(_logDir, "log*.txt");

            var matched = files
                .Where(file => string.IsNullOrEmpty(searchTerm) ||
                    System.IO.File.ReadAllText(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(file => new LogFileViewModel
                {
                    FileName = Path.GetFileName(file),
                    LastModified = System.IO.File.GetLastWriteTime(file),
                    SizeKB = new FileInfo(file).Length / 1024
                })
                .OrderByDescending(f => f.LastModified)
                .ToList();

            var total = matched.Count;
            var paged = matched.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(new LogFilesPageViewModel
            {
                Files = paged,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                SearchTerm = searchTerm ?? string.Empty
            });
        }

        public IActionResult Show(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("File name is required.");

            var filePath = Path.Combine(_logDir, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound($"File {fileName} not found.");

            try
            {
                string content;
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }

                ViewBag.FileName = fileName;
                return View("Show", content);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, title: "Error reading log file");
            }
        }


        public IActionResult DownloadFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("File name is required.");

            var filePath = Path.Combine(_logDir, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound($"File {fileName} not found.");

            try
            {
                byte[] bytes;
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    bytes = memory.ToArray();
                }

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, title: "Error downloading file");
            }
        }


        public IActionResult Delete(string fileName)
        {
            var filePath = Path.Combine(_logDir, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                TempData["Success"] = $"Deleted {fileName}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
