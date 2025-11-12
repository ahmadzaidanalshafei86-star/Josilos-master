namespace ES.Web.Areas.EsAdmin.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private List<string> _allowedExtentions = new() { ".jpg", ".png", ".jpeg",
            ".JPG",".PNG",".JPEG",".WEBP",".webp",".svg",".SVG",".gif",".GIF" ,".tiff", ".tif",".TIFF", ".TIF"};
        private int _maxAllowedSize = 20 * 1024 * 1024; // 20MB

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<(bool isUploaded, string? errorMessage)> UploadASync(IFormFile image, string imageName, string folderPath)
        {

            var extention = Path.GetExtension(image.FileName);

            if (image.Length > _maxAllowedSize)
                return (isUploaded: false, errorMessage: Errors.Filesize);

            if (!_allowedExtentions.Contains(extention))
                return (isUploaded: false, errorMessage: Errors.NotAllowedExtension);

            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}", imageName);

            using var stream = File.Create(path);
            await image.CopyToAsync(stream);
            stream.Dispose();

            return (isUploaded: true, errorMessage: null);

        }

        public void Delete(string imagePath)
        {
            var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}{imagePath}");
            if (File.Exists(oldImagePath))
                File.Delete(oldImagePath);
        }
    }
}

