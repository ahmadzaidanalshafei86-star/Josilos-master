namespace ES.Web.Areas.EsAdmin.Services
{
    public interface ILanguageService
    {
        Task<string?> GetDefaultDbCultureAsync();
    }
}
