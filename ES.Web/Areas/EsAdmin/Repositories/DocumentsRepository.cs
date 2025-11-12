namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class DocumentsRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Document?> GetDocByIdAsync(int docId)
        {
            return await _context.Documents.FindAsync(docId);
        }
        public async Task<IList<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }
        public async Task<int> AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
            return document.Id;
        }

        public void DeleteDocument(Document document)
        {
            _context.Documents.Remove(document);
            _context.SaveChanges();
        }


    }
}
