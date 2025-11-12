using Microsoft.AspNetCore.Mvc;
using ES.Web.Models;

namespace ES.Web.Controllers
{
    public class CustomerRegistrationController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            var model = new CustomerRegistrationViewModel
            {
                AvailableMaterials = GetMaterials()
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult Register(CustomerRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill all required fields correctly." });
            }

            // Check email, phone, company uniqueness here
            if (EmailExists(model.Email))
                return Json(new { success = false, message = "Email already exists." });

            if (PhoneExists(model.Phone))
                return Json(new { success = false, message = "Phone number already exists." });

            if (CompanyNameExists(model.CompanyName))
                return Json(new { success = false, message = "Company/Person already exists." });

            // Save registration to database
            int customerId = SaveCustomer(model);

            // Save selected materials
            foreach (var materialId in model.SelectedMaterials)
            {
                SaveCustomerMaterial(customerId, materialId);
            }

            return Json(new { success = true });
        }

        private List<MaterialItem> GetMaterials() => new List<MaterialItem>
        {
            new MaterialItem{ Id=1, Name="Steel" },
            new MaterialItem{ Id=2, Name="Wood" },
            new MaterialItem{ Id=0, Name="Other" }
        };

        private bool EmailExists(string email) => false;
        private bool PhoneExists(string phone) => false;
        private bool CompanyNameExists(string name) => false;
        private int SaveCustomer(CustomerRegistrationViewModel model) => 1;
        private void SaveCustomerMaterial(int customerId, int materialId) { }
    }
}
