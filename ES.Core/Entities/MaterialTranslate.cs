using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.Core.Entities
{
    public class MaterialTranslate
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}
