using ES.Web.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ES.Web.Services
{
    public class DeclerationsService
    {
        private readonly ApplicationDbContext _context;

        public DeclerationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SilosDeclerations>> GetAllAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);
            return await _context.SilosDeclerations.ToListAsync();
        }

        public async Task<SilosDeclerations?> GetByIdAsync(int id)
        {
            return await _context.SilosDeclerations.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<SilosDeclerations> CreateAsync(SilosDeclerations declaration)
        {
            declaration.CreatedAt = DateTime.Now;
            await _context.SilosDeclerations.AddAsync(declaration);
            await _context.SaveChangesAsync();
            return declaration;
        }

        public async Task<SilosDeclerations?> UpdateAsync(int id, SilosDeclerations declaration)
        {
            var existing = await _context.SilosDeclerations.FindAsync(id);

            if (existing == null)
                return null;

            existing.Subject = declaration.Subject;
            existing.StartDate = declaration.StartDate;
            existing.EndDate = declaration.EndDate;
            existing.Description = declaration.Description;
            existing.Notes = declaration.Notes;
            existing.SubjectAr = declaration.SubjectAr;
            existing.DescriptionAr = declaration.DescriptionAr;
            existing.NotesAr = declaration.NotesAr;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var declaration = await _context.SilosDeclerations.FirstOrDefaultAsync(d=>d.Id == id);

            if (declaration == null)
                return false;

            _context.SilosDeclerations.Remove(declaration);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}