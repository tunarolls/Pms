using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pms.Adjustments.Models;
using System;

namespace Pms.Adjustments.Persistence
{
    public class BillingRecordConfiguration : IEntityTypeConfiguration<BillingRecord>
    {

        public void Configure(EntityTypeBuilder<BillingRecord> builder)
        {
            builder.HasKey(pcv => pcv.RecordId);

            builder
               .HasOne(rec => rec.EE)
               .WithMany();

            builder.Property(cc => cc.EEId).HasColumnType("VARCHAR(8)").IsRequired();
            builder.Property(cc => cc.RecordId).HasColumnType("VARCHAR(45)").IsRequired();
            builder.Property(cc => cc.Status).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.DeductionOption).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.AdjustmentType).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.Status).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.Advances).HasColumnType("DOUBLE(8,2)").IsRequired();
            builder.Property(cc => cc.Balance).HasColumnType("DOUBLE(8,2)").IsRequired();
            builder.Property(cc => cc.Amortization).HasColumnType("DOUBLE(8,2)").IsRequired();
            builder.Property(cc => cc.EffectivityDate).HasColumnType("DATE");
            builder.Property(cc => cc.DateCreated).HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
        }

    }
}
