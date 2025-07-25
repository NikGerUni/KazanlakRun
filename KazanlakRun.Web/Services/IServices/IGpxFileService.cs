namespace KazanlakRun.Web.Services.IServices
{
    public interface IGpxFileService
    {
        Task<(Stream Stream, string ContentType)> GetGpxFileAsync(string fileId);
    }
}
