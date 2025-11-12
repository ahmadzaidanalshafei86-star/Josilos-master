using ES.Core.Entities;
using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;




namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class TendersController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TendersRepository _TendersRepository;
        private readonly ILanguageService _languageService;
        private readonly IImageService _imageService;
        private readonly IFilesService _filesService;
        private readonly SlugService _slugService;
        private readonly LanguagesRepository _languagesRepository;

        
        private readonly RowPermission _rowPermission;
     
        public TendersController(TendersRepository TendersRepository,
            LanguagesRepository languagesRepository,
           
            ILanguageService languageService,
            IImageService imageService,
            IFilesService filesService,
            SlugService slugService,
            CategoriesRepository categoriesRepository,
            DocumentsRepository documentsRepository,
            RowPermission rowPermission,
            RoleManager<IdentityRole> roleManager)
        {
            _TendersRepository = TendersRepository;
            _languagesRepository = languagesRepository;
          
            _languageService = languageService;
            _imageService = imageService;
            _filesService = filesService;
            _slugService = slugService;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
            
        }

        [Authorize(Permissions.Tenders.Read)]
        public async Task<IActionResult> Index()
        {
            var Tenders = await _TendersRepository.GetAllTendersAsync();

            

            return View(Tenders);
        }
        [HttpGet]
        [Authorize(Permissions.Tenders.Create)]
        public async Task<IActionResult> Create()
        {
            var model = await _TendersRepository.InitializeTenderFormViewModelAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Tenders.Create)]
        public async Task<IActionResult> Create(TenderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _TendersRepository.InitializeTenderFormViewModelAsync();
                return View("Form", model);
            }

            Tender tender = new()
            {
                Title = model.Title,
                Slug = _slugService.GenerateUniqueSlug(model.Title, nameof(Tender)),
                Code = model.Code,
                CopyPrice = model.CopyPrice,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                EnvelopeOpeningDate = model.EnvelopeOpeningDate,
                LastCopyPurchaseDate = model.LastCopyPurchaseDate,
                CreatedDate = DateTime.Now,
                Details = model.Details,
                PricesOffered = model.PricesOffered,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                TenderImageUrl = model.TenderImageUrl,
                PricesOfferedAttachmentUrl = model.PricesOfferedAttachmentUrl,
                InitialAwardFileUrl = model.InitialAwardFileUrl,
                FinalAwardFileUrl = model.FinalAwardFileUrl,
                Publish = model.Publish,
                PublishPricesOffered = model.PublishPricesOffered,
                SpecialOfferBlink = model.SpecialOfferBlink,
                MoveToArchive = model.MoveToArchive,
                BlinkStartDate = model.BlinkStartDate,
                BlinkEndDate = model.BlinkEndDate,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };

         


            var tenderId = await _TendersRepository.AddTenderAsync(tender);
            // Handle TenderImage Image
            if (model.TenderImage is not null)
            {
                var TenderImageName = $"{tenderId}_TenderImage{Path.GetExtension(model.TenderImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.TenderImage, TenderImageName, "/images/Tenders");
                if (isUploaded)
                {
                    tender.TenderImageUrl = TenderImageName;
                    tender.TenderImageAltName = model.TenderImage.FileName;
                    _TendersRepository.Updatepage(tender);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.TenderImage), errorMessage!);
                    model = await _TendersRepository.InitializeTenderFormViewModelAsync();
                    return View("Form", model);
                }
            }
            ////Files
            //if (model.TenderFiles is not null)
            //{

            //    for (int i = 0; i < model.TenderFiles.Count; i++)
            //    {
            //        TenderFile tenderFile = new();
            //        var tenderFileName = $"{tenderId}_{i}{Path.GetExtension(model.TenderFiles[i].FileName)}";
            //        var (isUploaded, errorMessage) = await _filesService.UploadASync(model.TenderFiles[i], tenderFileName, "/documents/Tender");
            //        if (isUploaded)
            //        {
            //            tenderFile.FileUrl = tenderFileName;
            //            tenderFile.AltName = model.TenderFiles[i].FileName;
            //            tenderFile.TenderId = tender.Id;
            //            tenderFile.DisplayOrder = i;
            //            await _TenderFilesRepository.AddFileAsync(tenderFile);
            //        }
            //        else
            //        {
            //            ModelState.AddModelError(nameof(model.TenderImage), errorMessage!);
            //            model = await _TendersRepository.InitializeTenderFormViewModelAsync();
            //            return View("Form", model);
            //        }
            //    }
            //}

            return Json(new { success = true, id = tenderId });
        }

 
    }
}
