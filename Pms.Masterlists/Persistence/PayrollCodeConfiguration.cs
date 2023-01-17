using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pms.Common;
using Pms.Masterlists.ValueObjects;

namespace Pms.Masterlists.Persistence
{
    public class PayrollCodeConfiguration : IEntityTypeConfiguration<PayrollCode>
    {
        public void Configure(EntityTypeBuilder<PayrollCode> builder)
        {
            builder.HasKey(ts => ts.PayrollCodeId);

            builder.Property(cc => cc.PayrollCodeId).HasColumnType("VARCHAR(12)").IsRequired();
            builder.Property(cc => cc.CompanyId).HasColumnType("VARCHAR(35)").IsRequired();
            builder.Property(cc => cc.Name).HasColumnType("VARCHAR(10)").IsRequired();
            builder.Property(cc => cc.Site).HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(cc => cc.Process).HasColumnType("TINYINT");
        }
    }
}
