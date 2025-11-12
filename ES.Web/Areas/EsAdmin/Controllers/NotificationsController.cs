namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync();

            if (notification == null)
            {
                // Create an empty notification if none exists
                notification = new Notification();
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return View(notification);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return View(notification);
            }

            var existing = await _context.Notifications.FirstOrDefaultAsync();
            if (existing == null)
            {
                return NotFound();
            }

            existing.Email = notification.Email;
            existing.WhatsAppNumber = notification.WhatsAppNumber;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Notification settings updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
