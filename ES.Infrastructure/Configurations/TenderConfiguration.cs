using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.Infrastructure.Configurations
{
    public class TenderConfiguration : IEntityTypeConfiguration<Tender>
    {
        public void Configure(EntityTypeBuilder<Tender> builder)
        {
            builder.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.NoAction);

          
        }
    }
}
