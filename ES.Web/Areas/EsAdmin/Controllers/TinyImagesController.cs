namespace ES.Web.Controllers
{
    [ApiController]
    [Route("controller")]
    public class TinyImagesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<TinyImagesController> _logger;
        private readonly string _tinyMceImagePath;
        private readonly string _temperorayPath;

        public TinyImagesController(IWebHostEnvironment webHostEnvironment, ILogger<TinyImagesController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _tinyMceImagePath = $"{_webHostEnvironment.WebRootPath}/images/Tinymce/editor-images";
            _temperorayPath = $"{_webHostEnvironment.WebRootPath}/images/Tinymce/temp";
        }

        [HttpPost("/TinyImages/image/upload")]
        public async Task<IActionResult> UploadEditorImage()
        {
            string path = "";
            IFormFile? file = Request.Form.Files.FirstOrDefault();

            if (file is not null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                path = Path.Combine(_tinyMceImagePath, fileName);
                var temperoaryPath = Path.Combine(_temperorayPath, fileName);

                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);

                using var tempStream = System.IO.File.Create(temperoaryPath);
                await file.CopyToAsync(tempStream);
            }

            var Location = path.Replace(_webHostEnvironment.WebRootPath, "").Trim().Replace("\\", "/");
            return Json(new { location = Location });
        }

        [HttpDelete("/TinyImages/image/delete")]
        public IActionResult DeleteEditorImage(string src)
        {
            System.IO.File.Delete(_webHostEnvironment.WebRootPath + src);
            System.IO.File.Delete(_webHostEnvironment.WebRootPath + src.Replace("editor", "temp"));

            return Ok();
        }
    }
}
