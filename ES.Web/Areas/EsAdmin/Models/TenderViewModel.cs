

using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class TenderViewModel
    {
        public int Id { get; set; }

       
        public string Title { get; set; } = null!;


        public string Slug { get; set; } = null!;


        public string Code { get; set; } = null!;

      
        public string CopyPrice { get; set; } = null!;

        // Dates
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       
       
        // Flags
        public bool Publish { get; set; } = false;
        

        // 🔹 Move to archive (instead of deleting)
        public bool MoveToArchive { get; set; } = false;
    }
}
