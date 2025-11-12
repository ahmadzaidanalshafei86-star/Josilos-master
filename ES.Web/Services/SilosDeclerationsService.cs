using ES.Web.Helpers;
using ES.Web.Models;
using ES.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ES.Web.Services
{
    public class SilosDeclerationsService
    {
        private readonly ApplicationDbContext _context;

        public SilosDeclerationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SilosDeclerationsViewModel> GetAllAsync()
        {
            var model = new SilosDeclerationsViewModel();

            model.SilosDecleration = await _context.SilosDeclerations
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SilosDeclerations
                {
                    Id = s.Id,
                    Subject = s.Subject,
                    SubjectAr = s.SubjectAr,
                    Description = s.Description,
                    DescriptionAr = s.DescriptionAr,
                    Notes = s.Notes,
                    NotesAr = s.NotesAr,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return model;
        }

        public async Task<SilosDeclerations?> GetByIdAsync(int id)
        {
            return await _context.SilosDeclerations
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
