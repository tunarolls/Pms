using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pms.Payrolls.Persistence
{
    public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
    {
        public void Configure(EntityTypeBuilder<Payroll> builder)
        {
            builder.ToTable("payroll").HasKey(ts => ts.PayrollId);


            builder.Property(cc => cc.PayrollId).HasColumnType("VARCHAR(35)").IsRequired();
            builder.Property(cc => cc.CompanyId).HasColumnType("VARCHAR(35)").IsRequired();
            builder.Property(cc => cc.CutoffId).HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(cc => cc.EEId).HasColumnType("VARCHAR(8)").IsRequired();
            builder.Property(cc => cc.YearCovered).HasColumnType("INT(4)").IsRequired();

            builder.Property(cc => cc.PayrollCode).HasColumnType("VARCHAR(6)");

            builder.Property(cc => cc.RegHours).HasColumnType("DOUBLE(6,2)").IsRequired();
            builder.Property(cc => cc.Overtime).HasColumnType("DOUBLE(6,2)").IsRequired();
            builder.Property(cc => cc.RestDayOvertime).HasColumnType("DOUBLE(6,2)").IsRequired();
            builder.Property(cc => cc.HolidayOvertime).HasColumnType("DOUBLE(6,2)").IsRequired();
            builder.Property(cc => cc.NightDifferential).HasColumnType("DOUBLE(6,2)").IsRequired();
            builder.Property(cc => cc.AbsTar).HasColumnType("DOUBLE(6,2)").IsRequired();

            builder.Property(cc => cc.GrossPay).HasColumnType("DOUBLE(9,2)").IsRequired();
            builder.Property(cc => cc.RegularPay).HasColumnType("DOUBLE(9,2)").IsRequired();
            builder.Property(cc => cc.NetPay).HasColumnType("DOUBLE(9,2)").IsRequired();
            builder.Property(cc => cc.EmployeeSSS).HasColumnType("DOUBLE(9,2)").IsRequired();
            builder.Property(cc => cc.EmployeePhilHealth).HasColumnType("DOUBLE(9,2)").IsRequired();
            builder.Property(cc => cc.EmployeePagibig).HasColumnType("DOUBLE(9,2)").IsRequired();

            builder.Property(cc => cc.WithholdingTax).HasColumnType("DOUBLE(9,2)").IsRequired();

            builder.Property(cc => cc.Adjust1Total).HasColumnType("DOUBLE(9,2)");
            builder.Property(cc => cc.Adjust2Total).HasColumnType("DOUBLE(9,2)");
            builder.Property(cc => cc.GovernmentTotal).HasColumnType("DOUBLE(9,2)");

        }
    }
}
