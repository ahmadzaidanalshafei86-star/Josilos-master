using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ES.Web.Areas.EsAdmin.Models;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class SmtpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SmtpController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.SmtpSettings.FirstOrDefaultAsync();

            SmtpSettingsViewModel model = new SmtpSettingsViewModel
            {
                Host = settings?.Host ?? string.Empty,
                Email = settings?.Email ?? string.Empty,
                Port = settings?.Port ?? 587,
                EnableSsl = settings?.EnableSsl ?? false,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] SmtpSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data. Please check the fields." });

            var existingSettings = await _context.SmtpSettings.FirstOrDefaultAsync();

            if (existingSettings == null)
            {
                existingSettings = new SmtpSettings
                {
                    Host = model.Host,
                    Port = model.Port,
                    Email = model.Email,
                    Password = model.Password!,
                    EnableSsl = model.EnableSsl
                };
                _context.SmtpSettings.Add(existingSettings);
            }
            else
            {
                existingSettings.Host = model.Host;
                existingSettings.Port = model.Port;
                existingSettings.Email = model.Email;
                existingSettings.Password = model.Password!;
                existingSettings.EnableSsl = model.EnableSsl;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "SMTP settings updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> TestSMTP([FromBody] TestSmtpRequest request)
        {
            var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
            if (smtpSettings == null)
            {
                return StatusCode(500, new { success = false, message = "SMTP settings are missing." });
            }

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("CMS System", smtpSettings.Email));
                email.To.Add(new MailboxAddress("", request.Email));
                email.Subject = "SMTP Test Email";
                email.Body = new TextPart("plain") { Text = "This is a test email from your CMS SMTP settings." };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpSettings.Host, smtpSettings.Port, smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None); // ✅ Use Port & SSL
                await smtp.AuthenticateAsync(smtpSettings.Email, smtpSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return Ok(new { success = true, message = "Test email sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"SMTP Test Failed: {ex.Message}" });
            }
        }



        public class TestSmtpRequest
        {
            [EmailAddress]
            public string Email { get; set; } = null!;
        }

    }
}
