namespace ES.Web.ViewComponents
{
    public class SocialMediaIconsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public SocialMediaIconsViewComponent( ApplicationDbContext context)
        {
               _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _context.SocialMediaLinks
                    .AsNoTracking() // for preformence optimization
                    .Where(s => s.IsPublished)
                    .Select(s => new SocialMediaLink
                    {
                        IconClass = s.IconClass,
                        IconColor = s.IconColor,
                        Url = s.Url,
                        Name = s.Name
                    })
                    .ToListAsync();

            return View(model);
        }
    }
}
