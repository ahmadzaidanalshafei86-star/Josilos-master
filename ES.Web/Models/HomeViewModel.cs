namespace ES.Web.Models.Client
{
    public class HomeViewModel
    {
        public IList<PageViewModel> MainSlider { get; set; } = new List<PageViewModel>();
        public CategoryWithPagesViewModel HomeCounter { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel HomeWelcome { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel SliderArticles { get; set; } = new CategoryWithPagesViewModel();
        public List<PageViewModel> Pages { get; set; } = new List<PageViewModel>();
        public CategoryWithPagesViewModel NewsTicker { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel HomeProducts { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel HomeServices { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel HomeNews { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel OurBlogs { get; set; } = new CategoryWithPagesViewModel();
        public CategoryWithPagesViewModel FooterHomeSlider { get; set; } = new CategoryWithPagesViewModel();
    }
}




