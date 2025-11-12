using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Controllers
{
    [Area("EsAdmin")]
    public class ProductDeliveries : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductDeliveries(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Permissions.Products.Read)]
        public async Task<IActionResult> Index()
        {
            var DeliveryZones = await _context.ProductDeliveries.ToListAsync();

            return View(DeliveryZones);
        }

        [HttpGet]
        [Authorize(Permissions.Products.Create)]
        public IActionResult Create()
        {
            ProductDeliveryFormViewModel model = new();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Create)]
        public async Task<IActionResult> Create(ProductDeliveryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model); // Re-render form with validation errors
            }

            var delivery = new ProductDelivery
            {
                Country = model.Country,
                City = model.City,
                Price = model.Price,
                IsAvailable = model.IsAvailable
            };

            await _context.ProductDeliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(int id)
        {
            var DeliveryZone = await _context.ProductDeliveries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (DeliveryZone is null)
                return NotFound();
            ProductDeliveryFormViewModel model = new()
            {
                Id = DeliveryZone.Id,
                Country = DeliveryZone.Country,
                City = DeliveryZone.City,
                Price = DeliveryZone.Price,
                IsAvailable = DeliveryZone.IsAvailable
            };
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(ProductDeliveryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model); // Re-render form with validation errors
            }
            var DeliveryZone = await _context.ProductDeliveries
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (DeliveryZone is null)
                return NotFound();
            DeliveryZone.Country = model.Country;
            DeliveryZone.City = model.City;
            DeliveryZone.Price = model.Price;
            DeliveryZone.IsAvailable = model.IsAvailable;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Products.Update))
                return StatusCode(403);
            var DeliveryZone = await _context.ProductDeliveries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (DeliveryZone is null)
                return StatusCode(402);

            DeliveryZone.IsAvailable = !DeliveryZone.IsAvailable;
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Products.Delete))
                return StatusCode(403);
            var DeliveryZone = await _context.ProductDeliveries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (DeliveryZone is null)
                return StatusCode(402);

            _context.ProductDeliveries.Remove(DeliveryZone);
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }

    }
}
