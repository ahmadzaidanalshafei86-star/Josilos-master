using AKM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.Infrastructure.Configurations
{
    public class TenderTranslatesConfiguration : IEntityTypeConfiguration<TenderTranslate>
    {
        public void Configure(EntityTypeBuilder<TenderTranslate> builder)
        {
            builder.HasOne(e => e.Tender)
                   .WithMany(e => e.TenderTranslates)
                   .HasForeignKey(e => e.TenderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
