using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class CareersRepository
    {

        private readonly ApplicationDbContext _context;
        public CareersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Career?> GetCareerByIdAsync(int id)
        {
            return await _context.Careers.FindAsync(id);
        }

        public async Task<IEnumerable<CareerViewModel>> GetCareersAsync()
        {
            return await _context.Careers
                .Select(c => new CareerViewModel
                {
                    Id = c.Id,
                    JobTitle = c.JobTitle,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<Career?> GetCareerWithTranslationAsync(int CareerId)
        {
            return await _context.Careers
                .Include(c => c.Language)
                .Include(c => c.CareerTranslates!)
                    .ThenInclude(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == CareerId);
        }

        public async Task<int> AddCareerAsync(Career Career)
        {
            await _context.Careers.AddAsync(Career);
            await _context.SaveChangesAsync();
            return Career.Id;
        }

        public async Task DeleteCareerAsync(Career Career)
        {
            _context.Careers.Remove(Career);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<CareerApplicationViewModel>, int)> GetApplicationsByCareerIdAsync(int careerId, int page, int pageSize, string search
            , string sort, bool showArchived, bool showUnreviewedOnly = false)
        {
            IQueryable<CareerApplication> query = _context.CareerApplications
                .Where(ca => ca.CareerId == careerId);

            if (!showArchived)
                query = query.Where(ca => !ca.IsArchived);

            if (showUnreviewedOnly)
                query = query.Where(ca => !ca.IsReviewed);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(ca => ca.FormResponse != null &&
                                          ca.FormResponse.ResponseDetails != null &&
                                          ca.FormResponse.ResponseDetails
                                          .Any(rd => rd.ResponseValue != null
                                          && EF.Functions.Like(rd.ResponseValue.ToLower(), $"%{search.ToLower()}%")));

            // Sorting
            if (sort == "date-asc")
                query = query.OrderBy(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);
            else if (sort == "date-desc")
                query = query.OrderByDescending(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);
            else
                query = query.OrderByDescending(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // Apply Includes after filtering and sorting
            query = query
                .Include(ca => ca.Career)
                .Include(ca => ca.FormResponse)
                    .ThenInclude(fr => fr.ResponseDetails)
                        .ThenInclude(rd => rd.Field);

            // Apply pagination and projection
            var applications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ca => new CareerApplicationViewModel
                {
                    ApplicationId = ca.Id,
                    SubmittedAt = ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue,
                    CareerName = ca.Career.JobTitle,
                    IsReviewed = ca.IsReviewed,
                    IsArchived = ca.IsArchived,
                    Details = ca.FormResponse != null && ca.FormResponse.ResponseDetails != null
                        ? ca.FormResponse.ResponseDetails.Select(rd => new ApplicationDetailViewModel
                        {
                            FieldName = rd.Field.FieldName,
                            FieldType = rd.Field.FieldType,
                            ResponseValue = rd.ResponseValue,
                            FileUrl = rd.Field.FieldType == "file" ? rd.ResponseValue : null
                        }).ToList()
                        : new List<ApplicationDetailViewModel>()
                })
                .ToListAsync();

            return (applications, totalCount);
        }
        public async Task MarkAsReviewedAsync(int[] applicationIds)
        {
            var applications = await _context.CareerApplications
                .Where(ca => applicationIds.Contains(ca.Id))
                .ToListAsync();
            foreach (var app in applications)
                app.IsReviewed = !app.IsReviewed;
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveApplicationsAsync(int[] applicationIds)
        {
            var applications = await _context.CareerApplications
                .Where(ca => applicationIds.Contains(ca.Id))
                .ToListAsync();
            foreach (var app in applications)
                app.IsArchived = !app.IsArchived;
            await _context.SaveChangesAsync();
        }
    }
}
