namespace ES.Web.Areas.EsAdmin.Services
{
    public interface IFilesService
    {
        Task<(bool isUploaded, string? errorMessage)> UploadASync(IFormFile file, string fileName, string folderPath);
        void Delete(string filePath);
    }
}
