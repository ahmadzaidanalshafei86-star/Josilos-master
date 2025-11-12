namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductDeliveryFormViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; } = null!;
        //public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public decimal Price { get; set; } = 0.0m;
        public bool IsAvailable { get; set; } = true;
    }
}
