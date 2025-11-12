using AKM.Core.Entities;
using System.Reflection;


namespace ES.Infastructure.Presistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // General CMS Tables
        public DbSet<Career> Careers { get; set; }
        public DbSet<CareerTranslate> CareerTranslates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslate> CategoriesTranslate { get; set; }


        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialTranslate> MaterialsTranslate { get; set; }

        public DbSet<Tender> Tenders { get; set; }
        public DbSet<TenderFile> TendersFiles { get; set; }
        public DbSet<TenderOtherAttachment> TenderOtherAttachments { get; set; }
        public DbSet<TenderMaterial> TenderMaterials { get; set; }
        public DbSet<TenderTranslate> TenderTranslates { get; set; }


        public DbSet<Document> Documents { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemTranslate> MenuItemTranslates { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageCategory> PageCategories { get; set; }
        public DbSet<PageGalleryImage> PageGalleryImages { get; set; }
        public DbSet<PageFile> PageFiles { get; set; }
        public DbSet<PageTranslate> PageTranslates { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<RowLevelPermission> RowLevelPermissions { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }

        public DbSet<SmtpSettings> SmtpSettings { get; set; }

        //Form Builder Tables
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormOption> FormOptions { get; set; }
        public DbSet<FormResponse> FormResponses { get; set; }
        public DbSet<FormResponseDetail> FormResponseDetails { get; set; }
        public DbSet<FormTranslation> FormTranslations { get; set; }
        public DbSet<FieldTranslation> FieldTranslations { get; set; }
        public DbSet<OptionTranslation> OptionTranslations { get; set; }
        public DbSet<CareerApplication> CareerApplications { get; set; }

        //E-Commerce Tables
        public DbSet<EcomCategory> EcomCategories { get; set; }
        public DbSet<EcomCategoryTranslate> EcomCategoriesTranslate { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductLinked> ProductLinks { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductTranslate> ProductTranslates { get; set; }
        public DbSet<ProductGalleryImage> ProductGalleryImages { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandTranslate> BrandTranslates { get; set; }

        public DbSet<ProductLabel> ProductLabels { get; set; }
        public DbSet<ProductLabelTranslate> ProductLabelTranslations { get; set; }
        public DbSet<ProductDelivery> ProductDeliveries { get; set; }

        // Product Attributes tables
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeTranslation> ProductAttributeTranslations { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductAttributeValueTranslation> ProductAttributeValueTranslations { get; set; }
        public DbSet<ProductAttributeMapping> ProductAttributeMappings { get; set; }
        public DbSet<ProductTab> ProductTabs { get; set; }
        public DbSet<ProductTabTranslation> ProductTabTranslations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemAttribute> OrderItemAttributes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SocialMediaLink> SocialMediaLinks { get; set; }
        public DbSet<SilosDeclerations> SilosDeclerations { get; set; }

        //View Models
        public DbSet<CategoryViewModel> CategoryViewModel { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Apply entity configurations in configrations folder
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<CategoryViewModel>().HasNoKey().ToView(null);
        }

    }
}