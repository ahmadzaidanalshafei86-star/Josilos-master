namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class PageFilesRepository
    {
        private readonly ApplicationDbContext _context;
        public PageFilesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddFileAsync(PageFile file)
        {
            await _context.PageFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            return file.Id;
        }

        public void DeleteRangeFiles(IList<PageFile> files)
        {
            _context.PageFiles.RemoveRange(files);
            _context.SaveChanges();
        }

        public async Task<IList<PageFile>> GetFilesOfPageAsync(int pageId)
        {
            return await _context.PageFiles.Where(pf => pf.PageId == pageId).ToListAsync();
        }
    }
}
