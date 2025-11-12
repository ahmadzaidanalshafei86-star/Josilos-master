using ES.Web.Helpers;
using ES.Web.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ES.Web.Services
{
    public class CareerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _careerFormTitle = "Career Application Form";

        public CareerService(ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CareersViewModel> GetCareersAndFormAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId },
                new SqlParameter("@CareerFormTitle", SqlDbType.NVarChar) { Value = _careerFormTitle }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetCareersAndForm";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main Career Data ---
            var careers = new List<Career>();
            while (await reader.ReadAsync())
            {
                careers.Add(new Career
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RefNumber = reader.IsDBNull(reader.GetOrdinal("RefNumber")) ? null : reader.GetString(reader.GetOrdinal("RefNumber")),
                    JobTitle = reader.GetString(reader.GetOrdinal("JobTitle")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? null : reader.GetDecimal(reader.GetOrdinal("Salary")),
                    EmploymentType = reader.IsDBNull(reader.GetOrdinal("EmploymentType")) ? null : reader.GetString(reader.GetOrdinal("EmploymentType")),
                    EnviromentType = reader.IsDBNull(reader.GetOrdinal("EnviromentType")) ? null : reader.GetString(reader.GetOrdinal("EnviromentType")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }

            // --- Result set 2: Career Translations ---
            await reader.NextResultAsync();
            var careerTranslations = new Dictionary<int, (string? JobTitle, string? Description, string? Location)>();
            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    var careerId = reader.GetInt32("CareerId");
                    careerTranslations[careerId] = (
                        reader.IsDBNull("JobTitle") ? null : reader.GetString("JobTitle"),
                        reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                        reader.IsDBNull("Location") ? null : reader.GetString("Location")
                    );
                }
            }

            // --- Result set 3: Career Application Form ---
            await reader.NextResultAsync();
            Form? careerForm = null;
            if (await reader.ReadAsync())
            {
                careerForm = new Form
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
            }

            if (careerForm == null)
                throw new Exception("Career application form not found. check the title of Career Form or activation");

            // --- Result set 4: Form Translations ---
            await reader.NextResultAsync();
            string? formTranslatedTitle = null, formTranslatedDescription = null;
            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    formTranslatedTitle = reader.IsDBNull("TranslatedTitle") ? null : reader.GetString("TranslatedTitle");
                    formTranslatedDescription = reader.IsDBNull("TranslatedDescription") ? null : reader.GetString("TranslatedDescription");
                    break;
                }
            }

            // --- Result set 5: Form Fields ---
            await reader.NextResultAsync();
            var fields = new List<FormField>();
            while (await reader.ReadAsync())
            {
                fields.Add(new FormField
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FormId = reader.GetInt32(reader.GetOrdinal("FormId")),
                    FieldName = reader.GetString(reader.GetOrdinal("FieldName")),
                    FieldHint = reader.IsDBNull(reader.GetOrdinal("FieldHint")) ? null : reader.GetString(reader.GetOrdinal("FieldHint")),
                    FieldType = reader.GetString(reader.GetOrdinal("FieldType")),
                    IsRequired = reader.GetBoolean(reader.GetOrdinal("IsRequired")),
                    IsPublished = reader.GetBoolean(reader.GetOrdinal("IsPublished")),
                    Order = reader.GetInt32(reader.GetOrdinal("Order"))
                });
            }

            // --- Result set 6: Field Translations ---
            await reader.NextResultAsync();
            var fieldTranslations = new Dictionary<int, (string? DisplayName, string? FieldHint)>();
            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    var fieldId = reader.GetInt32("FieldId");
                    fieldTranslations[fieldId] = (
                        reader.IsDBNull("TranslatedText") ? null : reader.GetString("TranslatedText"),
                        reader.IsDBNull("TranslatedFieldHint") ? null : reader.GetString("TranslatedFieldHint")
                    );
                }
            }

            // --- Result set 7: Field Options ---
            await reader.NextResultAsync();
            var fieldOptions = new Dictionary<int, List<FormOption>>();
            while (await reader.ReadAsync())
            {
                var fieldId = reader.GetInt32("FieldId");
                if (!fieldOptions.ContainsKey(fieldId))
                {
                    fieldOptions[fieldId] = new List<FormOption>();
                }

                fieldOptions[fieldId].Add(new FormOption
                {
                    Id = reader.GetInt32("Id"),
                    FieldId = fieldId,
                    OptionText = reader.GetString("OptionText"),
                    Order = reader.GetInt32("Order")
                });
            }

            // --- Result set 8: Option Translations ---
            await reader.NextResultAsync();
            var optionTranslations = new Dictionary<int, string>();
            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    optionTranslations[reader.GetInt32("OptionId")] = reader.GetString("TranslatedText");
                }
            }

            // Apply translations to form
            careerForm.Title = formTranslatedTitle ?? careerForm.Title;
            careerForm.Description = formTranslatedDescription ?? careerForm.Description;

            // Apply translations to fields and options
            foreach (var field in fields)
            {
                if (fieldTranslations.TryGetValue(field.Id, out var fieldTranslation))
                {
                    field.DisplayName = fieldTranslation.DisplayName ?? field.FieldName;
                    field.FieldHint = fieldTranslation.FieldHint ?? field.FieldHint;
                }
                else
                {
                    field.DisplayName = field.FieldName;
                }

                if (fieldOptions.TryGetValue(field.Id, out var options))
                {
                    foreach (var option in options)
                    {
                        if (optionTranslations.TryGetValue(option.Id, out var optionTranslation))
                        {
                            option.OptionText = optionTranslation;
                        }
                    }
                    field.Options = options.OrderBy(o => o.Order).ToList();
                }
            }

            careerForm.Fields = fields;

            // Apply translations to careers
            var localizedCareers = careers.Select(c =>
            {
                if (careerTranslations.TryGetValue(c.Id, out var translation))
                {
                return new Career
                {
                    Id = c.Id,
                    RefNumber = c.RefNumber,
                        JobTitle = translation.JobTitle ?? c.JobTitle,
                        Description = translation.Description ?? c.Description,
                        Location = translation.Location ?? c.Location,
                    Salary = c.Salary,
                    EmploymentType = c.EmploymentType,
                    EnviromentType = c.EnviromentType,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                };
                }
                return c;
            }).ToList();

            return new CareersViewModel
            {
                Careers = localizedCareers,
                CareerForm = careerForm
            };
        }

        public async Task<bool> SaveCareerApplicationAsync(int careerId, Dictionary<string, string[]> formData, IFormFileCollection files)
        {
            try
            {
                var career = await _context.Careers
                    .FirstOrDefaultAsync(c => c.Id == careerId && c.IsActive);
                if (career == null) return false;

                var careerForm = await _context.Forms
                    .Include(f => f.Fields)
                        .ThenInclude(ff => ff.Options)
                    .FirstOrDefaultAsync(f => f.Title == _careerFormTitle);
                if (careerForm == null) throw new Exception("Career application form not found.");

                var formResponse = new FormResponse
                {
                    FormId = careerForm.Id,
                    SubmittedAt = DateTime.Now
                };
                _context.FormResponses.Add(formResponse);
                await _context.SaveChangesAsync();

                var responseDetails = new List<FormResponseDetail>();

                foreach (var field in careerForm.Fields)
                {
                    if (field.FieldType == "file")
                    {
                        var file = files.FirstOrDefault(f => f.Name == field.FieldName && f.Length > 0);
                        if (file != null)
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CMS", "documents", "CareersApplications", fileName);

                            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists

                            using var fileStream = new FileStream(filePath, FileMode.Create);
                            await file.CopyToAsync(fileStream);

                            responseDetails.Add(new FormResponseDetail
                            {
                                ResponseId = formResponse.Id,
                                FieldId = field.Id,
                                ResponseValue = $"/CMS/documents/CareersApplications/{fileName}"
                            });
                        }
                    }
                    else if (formData.TryGetValue(field.FieldName, out var values) && values != null)
                    {
                        var filteredValues = field.FieldType == "checkbox" ? values.Where(v => !string.IsNullOrEmpty(v)) : new[] { values.FirstOrDefault() };
                        foreach (var value in filteredValues.Where(v => !string.IsNullOrEmpty(v)))
                        {
                            responseDetails.Add(new FormResponseDetail
                            {
                                ResponseId = formResponse.Id,
                                FieldId = field.Id,
                                ResponseValue = value
                            });
                        }
                    }
                }

                _context.FormResponseDetails.AddRange(responseDetails);

                var careerApplication = new CareerApplication
                {
                    CareerId = careerId,
                    FormResponseId = formResponse.Id
                };
                _context.CareerApplications.Add(careerApplication);

                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(careerForm.Email))
                {
                    var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
                    if (smtpSettings != null)
                    {
                        await SendNotificationEmail(careerForm.Email, career.Id, career.JobTitle, careerApplication.Id, smtpSettings);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No SMTP settings found for sending notification email.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No email address provided in form for sending notification.");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveCareerApplicationAsync: {ex}");
                return false;
            }
        }

        private async Task SendNotificationEmail(string recipientEmail, int careerId, string careerName, int CareerApplicationId, SmtpSettings smtpSettings)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.Email, "Email Solution Team"),
                    Subject = "New Career Application Submitted",
                    Body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333333; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f5f5f5; padding: 15px; text-align: center; border-radius: 5px 5px 0 0; }}
                        .header h2 {{ margin: 0; color: #005555; }}
                        .content {{ padding: 20px; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 0 0 5px 5px; }}
                        .content p {{ margin: 10px 0; }}
                        .label {{ font-weight: bold; color: #005555; }}
                        .footer {{ text-align: center; font-size: 12px; color: #777777; margin-top: 20px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #005555; color: #ffffff; text-decoration: none; border-radius: 5px; margin-top: 15px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>New Career Application</h2>
                        </div>
                        <div class='content'>
                            <p>Dear Team,</p>
                            <p>A new application has been submitted for review in the CMS. Below are the details:</p>
                            <p><span class='label'>Position:</span> {System.Web.HttpUtility.HtmlEncode(careerName)}</p>
                            <p><span class='label'>Application ID:</span> {CareerApplicationId}</p>
                            <p><span class='label'>Submitted:</span> {DateTime.Now:yyyy-MM-dd HH:mm:ss} </p>
                            <p>Please log in to the CMS to review the full application details and take appropriate action.</p>
                            <a href='https://localhost:44383/Careers/ViewApplications?careerId={careerId}' class='button'>View Application</a>
                        </div>
                        <div class='footer'>
                            <p>This is an automated notification from Email Solution portal.</p>
                            <p>Please do not reply to this email. Contact support@es.jo for assistance.</p>
                        </div>
                    </div>
                </body>
                </html>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(recipientEmail);

                using var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(smtpSettings.Email, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl
                };

                await smtpClient.SendMailAsync(mailMessage);
                System.Diagnostics.Debug.WriteLine("Notification email sent successfully.");
            }
            catch (Exception ex)
            {
                // Log the error but don't throw to avoid disrupting the application process
                System.Diagnostics.Debug.WriteLine($"Failed to send notification email: {ex.Message}");
            }
        }
    }
}
