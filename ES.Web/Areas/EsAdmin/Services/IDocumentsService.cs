namespace ES.Web.Areas.EsAdmin.Services
{
    public interface IDocumentsService
    {
        Task<(bool isUploaded, string? errorMessage)> UploadASync(IFormFile file, string fileName, string folderPath);
        void Delete(string filePath);
    }
}
