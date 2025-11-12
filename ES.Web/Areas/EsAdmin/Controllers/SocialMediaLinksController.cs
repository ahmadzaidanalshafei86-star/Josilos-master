namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class SocialMediaLinksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SocialMediaLinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Permissions.SocialMediaLinks.Read)]
        public IActionResult Index()
        {
            var socialMediaLinks = _context.SocialMediaLinks.ToList();

            return View(socialMediaLinks);
        }

        [Authorize(Permissions.SocialMediaLinks.Create)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(SocialMediaLink NewLink)
        {
            SocialMediaLink link = new()
            {
                Name = NewLink.Name,
                Url = NewLink.Url,
                IconClass = NewLink.IconClass,
                IconColor = NewLink.IconColor,
            };


            await _context.SocialMediaLinks.AddAsync(link);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Permissions.SocialMediaLinks.Update)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(SocialMediaLink Link)
        {
            var existingLink = await _context.SocialMediaLinks.FindAsync(Link.Id);

            if (existingLink is null)
                return NotFound();

            existingLink.Url = Link.Url;
            existingLink.IconClass = Link.IconClass;
            existingLink.IconColor = Link.IconColor;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.SocialMediaLinks.Update))
                return StatusCode(403);

            var link = await _context.SocialMediaLinks.FindAsync(id);

            if (link is null)
                return StatusCode(402);

            link.IsPublished = !link.IsPublished;
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }

    }
}

