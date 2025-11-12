using ES.Web.Helpers;
using ES.Web.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace ES.Web.Services
{
    public class PageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PageService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<PageViewModel?> GetPageBySlugAsync(string slug)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@Slug", SqlDbType.NVarChar) { Value = slug },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetPageBySlug";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main page ---
            if (!await reader.ReadAsync())
                return null;

            var page = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeatruedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeatruedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                LinkToUrl = reader.IsDBNull(reader.GetOrdinal("LinkToUrl")) ? null : reader.GetString(reader.GetOrdinal("LinkToUrl")),
                GalleryStyle = reader.IsDBNull(reader.GetOrdinal("GalleryStyle"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("GalleryStyle")), // ✅ fixed: string
                FormId = reader.IsDBNull(reader.GetOrdinal("FormId")) ? 0 : reader.GetInt32(reader.GetOrdinal("FormId"))
            };

            // --- Result set 2: Page translations ---
            await reader.NextResultAsync();
            string? translatedTitle = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    translatedTitle = reader.IsDBNull("Title") ? null : reader.GetString("Title");
                    translatedShortDesc = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
                    translatedLongDesc = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription");
                    break;
                }
            }

            // --- Result set 3: Gallery images ---
            await reader.NextResultAsync();
            var galleryImages = new List<GalleryImageViewModel>();

            while (await reader.ReadAsync())
            {
                galleryImages.Add(new GalleryImageViewModel
                {
                    GalleryImageAltName = reader.GetString("AltName"),
                    GalleryImageUrl = reader.GetString("GalleryImageUrl")
                });
            }

            var viewModel = new PageViewModel
            {
                Slug = page.Slug,
                Title = translatedTitle ?? page.Title,
                ShortDescription = translatedShortDesc ?? page.ShortDescription,
                LongDescription = translatedLongDesc ?? page.LongDescription,
                FeaturedImageUrl = page.FeaturedImageUrl,
                FeaturedImageAltName = page.FeaturedImageAltName,
                CoverImageUrl = page.CoverImageUrl,
                CoverImageAltName = page.CoverImageAltName,
                LinkUrl = page.LinkToUrl,
                GalleryImages = galleryImages,
                GalleryStyle = page.GalleryStyle // ✅ no .ToString()
            };

            // --- Results 4–9: Form Data ---
            if (page.FormId > 0)
            {
                await reader.NextResultAsync();

                if (await reader.ReadAsync())
                {
                    var form = new Form
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    };

                    // --- Result set 5: Form Translations ---
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(reader.GetOrdinal("LanguageId")) == languageId)
                        {
                            form.Title = reader.IsDBNull(reader.GetOrdinal("TranslatedTitle")) ? form.Title : reader.GetString(reader.GetOrdinal("TranslatedTitle"));
                            form.Description = reader.IsDBNull(reader.GetOrdinal("TranslatedDescription")) ? form.Description : reader.GetString(reader.GetOrdinal("TranslatedDescription"));
                            break;
                        }
                    }

                    // --- Result set 6: Form Fields ---
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

                    // --- Result set 7: Field Translations ---
                    await reader.NextResultAsync();
                    var fieldTranslations = new Dictionary<int, (string? DisplayName, string? FieldHint)>();
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(reader.GetOrdinal("LanguageId")) == languageId)
                        {
                            var fieldId = reader.GetInt32(reader.GetOrdinal("FieldId"));
                            fieldTranslations[fieldId] = (
                                reader.IsDBNull(reader.GetOrdinal("TranslatedText")) ? null : reader.GetString(reader.GetOrdinal("TranslatedText")),
                                reader.IsDBNull(reader.GetOrdinal("TranslatedFieldHint")) ? null : reader.GetString(reader.GetOrdinal("TranslatedFieldHint"))
                            );
                        }
                    }

                    // --- Result set 8: Field Options ---
                    await reader.NextResultAsync();
                    var fieldOptions = new Dictionary<int, List<FormOption>>();
                    while (await reader.ReadAsync())
                    {
                        var fieldId = reader.GetInt32(reader.GetOrdinal("FieldId"));
                        if (!fieldOptions.ContainsKey(fieldId))
                        {
                            fieldOptions[fieldId] = new List<FormOption>();
                        }

                        fieldOptions[fieldId].Add(new FormOption
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FieldId = fieldId,
                            OptionText = reader.GetString(reader.GetOrdinal("OptionText")),
                            Order = reader.GetInt32(reader.GetOrdinal("Order"))
                        });
                    }

                    // --- Result set 9: Option Translations ---
                    await reader.NextResultAsync();
                    var optionTranslations = new Dictionary<int, string>();
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(reader.GetOrdinal("LanguageId")) == languageId)
                        {
                            optionTranslations[reader.GetInt32(reader.GetOrdinal("OptionId"))] =
                                reader.GetString(reader.GetOrdinal("TranslatedText"));
                        }
                    }

                    // --- Apply translations ---
                    foreach (var field in fields)
                    {
                        if (fieldTranslations.TryGetValue(field.Id, out var translation))
                        {
                            field.DisplayName = translation.DisplayName ?? field.FieldName;
                            field.FieldHint = translation.FieldHint ?? field.FieldHint;
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

                    form.Fields = fields;
                    viewModel.HaveForm = true;
                    viewModel.PageForm = form;
                }
            }

            return viewModel;
        }

        public async Task<PageViewModel?> GetPageByIdAsync(int pageId)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@PageId", SqlDbType.Int) { Value = pageId },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetPageById";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // -------- Result set 1: Main Page --------
            if (!await reader.ReadAsync())
                return null;

            var page = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeatruedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeatruedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                LinkToUrl = reader.IsDBNull(reader.GetOrdinal("LinkToUrl")) ? null : reader.GetString(reader.GetOrdinal("LinkToUrl")),
                GalleryStyle = reader.IsDBNull(reader.GetOrdinal("GalleryStyle"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("GalleryStyle")),
            };

            // -------- Result set 2: Page Translations --------
            await reader.NextResultAsync();
            string? translatedTitle = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32(reader.GetOrdinal("LanguageId")) == languageId)
                {
                    translatedTitle = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title"));
                    translatedShortDesc = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription"));
                    translatedLongDesc = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription"));
                    break;
                }
            }

            // -------- Result set 3: Gallery Images --------
            await reader.NextResultAsync();
            var galleryImages = new List<GalleryImageViewModel>();

            while (await reader.ReadAsync())
            {
                galleryImages.Add(new GalleryImageViewModel
                {
                    GalleryImageAltName = reader.IsDBNull(reader.GetOrdinal("AltName")) ? null : reader.GetString(reader.GetOrdinal("AltName")),
                    GalleryImageUrl = reader.GetString(reader.GetOrdinal("GalleryImageUrl"))
                });
            }

            var viewModel = new PageViewModel
            {
                Slug = page.Slug,
                Title = translatedTitle ?? page.Title,
                ShortDescription = translatedShortDesc ?? page.ShortDescription,
                LongDescription = translatedLongDesc ?? page.LongDescription,
                FeaturedImageUrl = page.FeaturedImageUrl,
                FeaturedImageAltName = page.FeaturedImageAltName,
                CoverImageUrl = page.CoverImageUrl,
                CoverImageAltName = page.CoverImageAltName,
                LinkUrl = page.LinkToUrl,
                GalleryImages = galleryImages,
                GalleryStyle = page.GalleryStyle?.ToString(),
            };
            return viewModel;
        }


        public async Task<bool> SavePageFormAsync(int formId, Dictionary<string, string[]> formData, IFormFileCollection files)
        {
            try
            {

                var PageForm = await _context.Forms
                    .Include(f => f.Fields)
                        .ThenInclude(ff => ff.Options)
                    .FirstOrDefaultAsync(f => f.Id == formId);
                if (PageForm == null) throw new Exception("page form not found.");

                var formResponse = new FormResponse
                {
                    FormId = PageForm.Id,
                };
                _context.FormResponses.Add(formResponse);
                await _context.SaveChangesAsync();

                var responseDetails = new List<FormResponseDetail>();

                foreach (var field in PageForm.Fields)
                {
                    if (field.FieldType == "file")
                    {
                        var file = files.FirstOrDefault(f => f.Name == field.FieldName && f.Length > 0);
                        if (file != null)
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CMS", "documents", "Pages", fileName);

                            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists

                            using var fileStream = new FileStream(filePath, FileMode.Create);
                            await file.CopyToAsync(fileStream);

                            responseDetails.Add(new FormResponseDetail
                            {
                                ResponseId = formResponse.Id,
                                FieldId = field.Id,
                                ResponseValue = $"/CMS/documents/Pages/{fileName}"
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

                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(PageForm.Email))
                {
                    var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
                    if (smtpSettings != null)
                    {
                        await SendNotificationEmail(PageForm.Email, PageForm.Title, smtpSettings);
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

        private async Task SendNotificationEmail(string recipientEmail, string PageTitle, SmtpSettings smtpSettings)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.Email, "Email Solution Team"),
                    Subject = "New Form Submitted",
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
                            <h2>New Form Submission</h2>
                        </div>
                        <div class='content'>
                            <p>Dear Team,</p>
                            <p>A new form has been submitted for review in the CMS. Below are the details:</p>
                            <p><span class='label'>Position:</span> {System.Web.HttpUtility.HtmlEncode(PageTitle)}</p>
                            <p><span class='label'>Submitted:</span> {DateTime.Now:yyyy-MM-dd HH:mm:ss} </p>
                            <p>Please log in to the CMS to review the full application details and take appropriate action.</p>
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
