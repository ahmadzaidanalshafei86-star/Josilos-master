using Microsoft.Extensions.Localization;
using ES.Web.Models;

namespace ES.Web.Controllers
{
    public class NewsLetterClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<NewsLetterClientController> _localizer;

        public NewsLetterClientController(ApplicationDbContext context, IStringLocalizer<NewsLetterClientController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(NewsLetterFormViewModel model)
        {
            var emailExists = await _context.NewsLetters
                .AnyAsync(nl => nl.Email == model.Email);

            if (emailExists)
            {
                var msg = _localizer["This email is already subscribed."].Value;
                return Json(new
                {
                    success = false,
                    message = _localizer["This email is already subscribed."].Value,
                    title = _localizer["Error"].Value
                });

            }

            await _context.NewsLetters.AddAsync(new NewsLetter { Email = model.Email });
            await _context.SaveChangesAsync();

            var successMsg = _localizer["Subscription successful!"];
            return Json(new
            {
                success = true,
                message = _localizer["Subscription successful!"].Value,
                title = _localizer["Success"].Value
            });

        }

    }
}
