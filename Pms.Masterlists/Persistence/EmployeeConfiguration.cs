using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pms.Masterlists.Entities;

namespace Pms.Masterlists.Persistence
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {

        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("masterlist").HasKey(ts => ts.EEId);

            builder.Property(cc => cc.EEId).HasColumnType("VARCHAR(8)").IsRequired();
            builder.Property(cc => cc.FirstName).HasColumnType("VARCHAR(45)");
            builder.Property(cc => cc.LastName).HasColumnType("VARCHAR(45)");
            builder.Property(cc => cc.MiddleName).HasColumnType("VARCHAR(45)");
            builder.Property(cc => cc.NameExtension).HasColumnType("VARCHAR(6)");
            builder.Property(cc => cc.Gender).HasColumnType("VARCHAR(1)");
            builder.Property(cc => cc.BirthDate).HasColumnType("DATE");
            builder.Property(cc => cc.DateResigned).HasColumnType("DATE");
            builder.Property(cc => cc.DateHired).HasColumnType("DATE");

            builder.Property(cc => cc.Location).HasColumnType("VARCHAR(45)");
            builder.Property(cc => cc.Site).HasColumnType("VARCHAR(25)");
            builder.Property(cc => cc.CompanyId).HasColumnType("VARCHAR(25)");
            builder.Property(cc => cc.JobCode).HasColumnType("VARCHAR(25)");
            builder.Ignore(cc => cc.JobRemarks);

            builder.Property(cc => cc.PayrollCode).HasColumnType("VARCHAR(6)");
            builder.Property(cc => cc.CardNumber).HasColumnType("VARCHAR(30)");
            builder.Property(cc => cc.AccountNumber).HasColumnType("VARCHAR(30)");
            builder.Property(cc => cc.Bank).HasColumnType("TINYINT");
            
            builder.Property(cc => cc.Pagibig).HasColumnType("VARCHAR(20)");
            builder.Property(cc => cc.PhilHealth).HasColumnType("VARCHAR(20)");
            builder.Property(cc => cc.SSS).HasColumnType("VARCHAR(20)");
            builder.Property(cc => cc.TIN).HasColumnType("VARCHAR(20)");

            builder.Property(cc => cc.DateModified).HasColumnType("DATETIME");
        }
    }
}