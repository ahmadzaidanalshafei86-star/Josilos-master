namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class LanguagesRepository
    {
        private readonly ApplicationDbContext _context;
        public LanguagesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddLanguage(Language language)
        {
            _context.Languages.Add(language);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Language>> GetAllLanguages()
        {
            return await _context.Languages.ToListAsync();
        }

        public async Task<int> GetLanguageByCode(string code)
        {
            return await _context.Languages
                  .Where(l => l.Code == code)
                  .Select(l => l.Id)
                  .SingleAsync();
        }
    }
}
