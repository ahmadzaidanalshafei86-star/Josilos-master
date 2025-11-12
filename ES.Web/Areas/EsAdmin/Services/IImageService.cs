namespace ES.Web.Areas.EsAdmin.Services
{
    public interface IImageService
    {
        Task<(bool isUploaded, string? errorMessage)> UploadASync(IFormFile image, string imageName, string folderPath);
        void Delete(string imagePath);
    }
}
