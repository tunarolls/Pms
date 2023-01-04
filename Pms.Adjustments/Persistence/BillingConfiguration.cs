using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pms.Adjustments.Models;
using System;

namespace Pms.Adjustments.Persistence
{
    public class BillingConfiguration : IEntityTypeConfiguration<Billing>
    {

        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.ToTable("adjustment_billing").HasKey(pcv => pcv.BillingId);

            builder
               .HasOne(rec => rec.EE)
               .WithMany();

            //builder
            //    .HasOne(pcv => pcv.Record)
            //    .WithMany(rec => rec.Billings);

            builder.Property(cc => cc.EEId).HasColumnType("VARCHAR(8)").IsRequired();
            builder.Property(cc => cc.BillingId).HasColumnType("VARCHAR(45)").IsRequired();
            builder.Property(cc => cc.CutoffId).HasColumnType("VARCHAR(6)").IsRequired();
            builder.Property(cc => cc.RecordId).HasColumnType("VARCHAR(45)").IsRequired(false);
            builder.Property(cc => cc.AdjustmentType).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.AdjustmentOption).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.Deducted).HasColumnType("TINYINT").IsRequired();
            builder.Property(cc => cc.Amount).HasColumnType("DOUBLE(8,2)").IsRequired();
            builder.Property(cc => cc.DateCreated).HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
        }

    }
}
