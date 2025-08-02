using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        private readonly string _logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        private static readonly Regex ValidLogFileNameRegex = new(@"^log.*\.txt$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private bool IsValidLogFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var name = Path.GetFileName(fileName);
            if (!string.Equals(name, fileName, StringComparison.Ordinal)) return false;
            return ValidLogFileNameRegex.IsMatch(name);
        }

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
            if (!IsValidLogFileName(fileName))
                return BadRequest("Invalid or missing file name.");

            var filePath = Path.Combine(_logDir, fileName!);
            if (!System.IO.File.Exists(filePath))
                return NotFound($"File {fileName} not found.");

            try
            {
                string content;
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

                using var reader = new StreamReader(stream);
                content = reader.ReadToEnd();

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
            if (!IsValidLogFileName(fileName))
                return BadRequest("Invalid or missing file name.");

            var filePath = Path.Combine(_logDir, fileName!);
            if (!System.IO.File.Exists(filePath))
                return NotFound($"File {fileName} not found.");

            try
            {
                byte[] bytes;
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

                using var memory = new MemoryStream();
                stream.CopyTo(memory);
                bytes = memory.ToArray();

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, title: "Error downloading file");
            }
        }

      
        [HttpGet]
        public IActionResult Delete(string? fileName)
        {
            if (!IsValidLogFileName(fileName))
                return BadRequest("Invalid or missing file name.");

            var filePath = Path.Combine(_logDir, fileName!);
            if (!System.IO.File.Exists(filePath))
                return NotFound($"File {fileName} not found.");

            var vm = new LogFileViewModel
            {
                FileName = fileName!,
                LastModified = System.IO.File.GetLastWriteTime(filePath),
                SizeKB = new FileInfo(filePath).Length / 1024
            };

            return View("Delete", vm);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(string? fileName)
        {
            if (!IsValidLogFileName(fileName))
            {
                TempData["Error"] = "Invalid file name.";
                return RedirectToAction(nameof(Index));
            }

            var filePath = Path.Combine(_logDir, fileName!);
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = $"File {fileName} not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                System.IO.File.Delete(filePath);
                TempData["Success"] = $"Deleted {fileName}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete {fileName}: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
