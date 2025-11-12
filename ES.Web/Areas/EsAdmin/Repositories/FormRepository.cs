using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class FormRepository
    {
        private readonly ApplicationDbContext _context;

        public FormRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormsViewModel>> GetFormsAsync()
        {
            return await _context.Forms
                 .Select(f => new FormsViewModel
                 {
                     Id = f.Id,
                     Title = f.Title,
                     CreatedAt = f.CreatedAt,
                     IsActive = f.IsActive
                 })
                 .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetFormsNamesAsync()
        {
            return await _context.Forms
             .Select(pc => new SelectListItem
             {
                 Value = pc.Id.ToString(),
                 Text = pc.Title
             })
             .ToListAsync();
        }

        public async Task<Form?> GetFormWithResponsesAsync(int formId)
        {
            var form = await _context.Forms
                .Include(f => f.Fields) // Include Fields if needed
                .FirstOrDefaultAsync(f => f.Id == formId);

            if (form == null)
                return null;

            // Fetch responses separately
            var responses = await _context.FormResponses
                .Where(r => r.FormId == formId)
                .Include(r => r.ResponseDetails)
                .ThenInclude(rd => rd.Field)
                .ToListAsync();

            return form;
        }
        public async Task<List<FormResponse>> GetResponsesByFormIdAsync(int formId)
        {
            return await _context.FormResponses
                .Where(r => r.FormId == formId)
                .Include(r => r.ResponseDetails)
                .ThenInclude(rd => rd.Field)
                .ToListAsync();
        }

        public async Task<Form> GetFormWithTranslationsAsync(int formId)
        {
            var form = await _context.Forms
                .Include(f => f.Language)
                .Include(f => f.Translations!)
                .ThenInclude(ft => ft.Language)
                .SingleOrDefaultAsync(f => f.Id == formId);

            if (form is null)
                return null;

            return form;
        }

        public async Task<Form> GetFullFormByIdAsync(int id)
        {
            var Form = await _context.Forms
                .Include(f => f.Fields)
                .ThenInclude(ff => ff.Options)
                .SingleOrDefaultAsync(f => f.Id == id);

            if (Form == null)
                return null;

            return Form;
        }

        public async Task<Form> FindByIdAsync(int id)
        {
            return await _context.Forms.FindAsync(id);
        }

        public async Task AddAsync(Form form)
        {
            await _context.Forms.AddAsync(form);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Form form)
        {
            var existingForm = await _context.Forms
                .Include(f => f.Fields)
                    .ThenInclude(f => f.Options)
                .FirstOrDefaultAsync(f => f.Id == form.Id);

            if (existingForm == null)
                throw new InvalidOperationException("Form not found");

            // Update form-level properties
            _context.Entry(existingForm).CurrentValues.SetValues(form);

            // Update fields
            foreach (var field in form.Fields)
            {
                var existingField = existingForm.Fields.FirstOrDefault(f => f.Id == field.Id);

                if (existingField != null)  // Update existing field
                {
                    _context.Entry(existingField).CurrentValues.SetValues(field);

                    // Update options
                    foreach (var option in field.Options)
                    {
                        var existingOption = existingField.Options.FirstOrDefault(o => o.Id == option.Id);
                        if (existingOption != null)
                        {
                            _context.Entry(existingOption).CurrentValues.SetValues(option);
                        }
                        else
                        {
                            existingField.Options.Add(option); // Add new option
                        }
                    }

                    // Remove deleted options
                    var optionsToRemove = existingField.Options
                        .Where(o => !field.Options.Any(fo => fo.Id == o.Id))
                        .ToList();
                    foreach (var removedOption in optionsToRemove)
                    {
                        _context.FormOptions.Remove(removedOption);
                    }
                }
                else  // Add new field
                {
                    existingForm.Fields.Add(field);
                }
            }

            // Remove deleted fields
            var fieldsToRemove = existingForm.Fields
                .Where(f => !form.Fields.Any(ff => ff.Id == f.Id))
                .ToList();
            foreach (var removedField in fieldsToRemove)
            {
                _context.FormFields.Remove(removedField);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database update failed. Check foreign key constraints.", ex);
            }
        }


        public async Task DeleteAsync(Form form)
        {
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Form form)
        {
            _context.Update(form);
            await _context.SaveChangesAsync();
        }
    }
}
