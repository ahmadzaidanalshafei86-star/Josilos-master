using ES.Web.Areas.EsAdmin.Helpers;

namespace ES.Web.Areas.EsAdmin.Controllers
{
    [Area("EsAdmin")]   
    public class NewsLetterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsLetterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var newsletters = await _context.NewsLetters
                .ToListAsync();

            return View(newsletters);
        }

        [HttpGet]
        public async Task<IActionResult> SaveExcel()
        {
            var newsletters = await _context.NewsLetters.ToListAsync();
            // Generate Excel file using the helper
            var fileContents = await ExcelExportHelper.GenerateExcel(newsletters, "Email", "Submitted At");

            // Return the file for download
            return File(fileContents,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NewsletterSubscriptions_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
