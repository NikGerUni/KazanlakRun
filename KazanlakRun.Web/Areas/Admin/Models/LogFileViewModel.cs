namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class LogFileViewModel
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public long SizeKB { get; set; }
    }

    public class LogFilesPageViewModel
    {
        public List<LogFileViewModel> Files { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }
}
