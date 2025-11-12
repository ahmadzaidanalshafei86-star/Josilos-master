namespace ES.Web.Areas.EsAdmin.Services
{
    public class FilesService : IFilesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private List<string> _allowedExtentions = new() { ".pdf", ".PDF", ".DOCX", ".docx" };
        private int _maxAllowedSize = 5242880;// 5MB(inBytes) 5 * 1024 * 1024

        public FilesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public void Delete(string filePath)
        {
            var oldFilePath = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}");
            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);
        }

        public async Task<(bool isUploaded, string? errorMessage)> UploadASync(IFormFile file, string fileName, string folderPath)
        {
            var extention = Path.GetExtension(file.FileName);

            if (file.Length > _maxAllowedSize)
                return (isUploaded: false, errorMessage: Errors.Filesize);

            if (!_allowedExtentions.Contains(extention))
                return (isUploaded: false, errorMessage: Errors.NotAllowedExtension);

            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}", fileName);

            using var stream = File.Create(path);
            await file.CopyToAsync(stream);
            stream.Dispose();

            return (isUploaded: true, errorMessage: null);
        }
    }
}
