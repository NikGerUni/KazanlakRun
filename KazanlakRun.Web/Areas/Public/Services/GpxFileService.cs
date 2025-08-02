using Google.Apis.Drive.v3;
using KazanlakRun.Web.Areas.Public.Services.IServices;

namespace KazanlakRun.Web.Areas.Public.Services
{
    public class GpxFileService : IGpxFileService
    {
        private readonly DriveService _driveService;

        public GpxFileService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<(Stream Stream, string ContentType)> GetGpxFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;
            return (stream, "application/gpx+xml");
        }
    }
}

