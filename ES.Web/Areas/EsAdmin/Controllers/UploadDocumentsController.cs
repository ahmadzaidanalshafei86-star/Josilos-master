using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;
using System.Globalization;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class UploadDocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;
        private readonly DocumentsRepository _documentsRepository;

        public UploadDocumentsController(IDocumentsService documentsService, DocumentsRepository documentsRepository)
        {
            _documentsService = documentsService;
            _documentsRepository = documentsRepository;
        }

        [Authorize(Permissions.Documents.Read)]
        public IActionResult Index()
        {
            IEnumerable<DocumentsViewModel> documents = _documentsRepository
               .GetAllDocumentsAsync()
               .Result
               .Select(d => new DocumentsViewModel
               {
                   Id = d.Id,
                   DocumentName = d.OriginalName,
                   DocumentURL = d.FullUrl
               });
            return View(documents);
        }


        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> documents)
        {
            if (!User.HasClaim("Permission", Permissions.Documents.Create))
                return StatusCode(403);

            if (documents.Count == 0)
                return BadRequest("No files were uploaded!");

            var currentDomain = $"{Request.Scheme}://{Request.Host.Value}";

            foreach (var doc in documents)
            {
                var slug = SlugHelper.GenerateSlug(Path.GetFileNameWithoutExtension(doc.FileName));
                var timestamp = DateTime.UtcNow.ToString("mmssfff", CultureInfo.InvariantCulture); //"yyyyMMddHHmmssfff"
                var docName = $"{slug}-{timestamp}{Path.GetExtension(doc.FileName)}";
                var fullDocPath = $"/CMS/documents/General/{docName}";

                var (isUploaded, errorMessage) = await _documentsService.UploadASync(doc, docName, "/CMS/documents/General");
                if (isUploaded)
                {
                    Document document = new()
                    {
                        OriginalName = doc.FileName,
                        GenratedName = docName,
                        FullUrl = $"{currentDomain}{fullDocPath}",
                        UploadedOn = DateTime.Now
                    };
                    await _documentsRepository.AddDocumentAsync(document);
                }
                else
                    return BadRequest(errorMessage);
            }


            return Ok(new { message = $"{documents.Count} file(s) uploaded successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Documents.Delete))
                return StatusCode(403);


            var document = await _documentsRepository.GetDocByIdAsync(id);

            if (document is null)
                return NotFound();

            _documentsService.Delete($"/CMS/documents/General/{document.GenratedName}");
            _documentsRepository.DeleteDocument(document);

            return StatusCode(200);
        }
    }
}
