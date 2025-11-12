using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class ProductAttributesController : Controller
    {
        private readonly ProductAttributesRepository _productAttributesRepository;
        private readonly LanguagesRepository _languagesRepository;
        private readonly ILanguageService _languageService;

        public ProductAttributesController(ProductAttributesRepository productAttributesRepository,
            LanguagesRepository languagesRepository,
            ILanguageService languageService)
        {
            _productAttributesRepository = productAttributesRepository;
            _languagesRepository = languagesRepository;
            _languageService = languageService;
        }

        [HttpGet]
        [Authorize(Permissions.ProductAttributes.Read)]
        public async Task<IActionResult> Index()
        {
            var attributes = await _productAttributesRepository.GetAllAsync();
            return View(attributes);
        }


        [HttpPost]
        public async Task<IActionResult> AddAttribute(string name)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Create))
                return StatusCode(403);

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { success = false, message = "Attribute name is required." });

            ProductAttribute attribute = new()
            {
                Name = name,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };
            await _productAttributesRepository.AddAsync(attribute);

            return Ok();
        }

        // POST: Add Value to Attribute (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddValue(int attributeId, string value, int order)
        {

            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Create))
                return StatusCode(403);

            if (string.IsNullOrWhiteSpace(value))
                return BadRequest(new { success = false, message = "Value is required." });

            var attribute = await _productAttributesRepository.GetByIdAsync(attributeId);
            if (attribute == null)
                return NotFound(new { success = false, message = "Attribute not found." });

            ProductAttributeValue newValue = new()
            {
                Value = value,
                Order = order,
                ProductAttributeId = attributeId,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };

            await _productAttributesRepository.AddValueAsync(newValue);

            return Ok(new { success = true, message = "Value added successfully!" });
        }

        // POST: Update Attribute text (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateAttribute(int id, string name)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Update))
                return StatusCode(403);

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name cannot be empty");

            var ProductAttribute = await _productAttributesRepository.GetAttributeEntityByIdAsync(id);
            if (ProductAttribute == null)
                return NotFound();

            ProductAttribute.Name = name;
            await _productAttributesRepository.UpdateAttributeAsync(ProductAttribute);

            return Ok();
        }

        // POST: Update Value Text (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateValueText(int id, string NewValueText)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Update))
                return StatusCode(403);

            if (id <= 0 || string.IsNullOrWhiteSpace(NewValueText))
                return BadRequest("Invalid data.");

            var AttibuteValue = await _productAttributesRepository.GetValueByIdAsync(id);

            if (AttibuteValue == null)
                return NotFound("Value not found.");

            AttibuteValue.Value = NewValueText;
            await _productAttributesRepository.UpdateValueAsync(AttibuteValue);

            return Ok();
        }

        // POST: Delete Attribute (AJAX)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Delete))
                return StatusCode(403);

            var attribute = await _productAttributesRepository.GetAttributeEntityByIdAsync(id);

            if (attribute == null)
                return NotFound();

            await _productAttributesRepository.DeleteAttirbuteAsync(attribute);
            return Ok();
        }

        // POST: Delete Value (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteValue(int id)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Delete))
                return StatusCode(403);

            var value = await _productAttributesRepository.GetValueByIdAsync(id);
            if (value == null)
                return NotFound("Value not found.");

            await _productAttributesRepository.DeleteValueAsync(value);
            return StatusCode(200);
        }

        // POST: Update Value Order (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateValueOrder([FromBody] List<AttributeValueOrderViewModel> orderData)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Update))
                return StatusCode(403);

            if (!orderData.Any())
                return BadRequest("Invalid data.");

            foreach (var item in orderData)
            {
                var value = await _productAttributesRepository.GetValueByIdAsync(item.Id);
                if (value != null)
                    value.Order = item.Order;

            }

            await _productAttributesRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
