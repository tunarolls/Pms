﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pms.Masterlists.Persistence;

namespace Pms.Masterlists.Migrations
{
    [DbContext(typeof(EmployeeDbContext))]
    [Migration("20220920115525_definedMaxLengthToJobCode")]
    partial class definedMaxLengthToJobCode
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("Pms.Masterlists.Entities.Company", b =>
                {
                    b.Property<string>("CompanyId")
                        .HasColumnType("VARCHAR(35)");

                    b.Property<string>("Acronym")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<byte>("BranchCode")
                        .HasColumnType("TINYINT");

                    b.Property<double>("MinimumRate")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<string>("Region")
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("RegisteredName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("Site")
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("TIN")
                        .HasColumnType("VARCHAR(20)");

                    b.HasKey("CompanyId");

                    b.ToTable("company");
                });

            modelBuilder.Entity("Pms.Masterlists.Entities.Employee", b =>
                {
                    b.Property<string>("EEId")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("VARCHAR(30)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte>("Bank")
                        .HasColumnType("TINYINT");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("DATE");

                    b.Property<string>("CardNumber")
                        .HasColumnType("VARCHAR(30)");

                    b.Property<string>("CompanyId")
                        .HasColumnType("VARCHAR(25)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("DATETIME");

                    b.Property<string>("FirstName")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("Gender")
                        .HasColumnType("VARCHAR(1)");

                    b.Property<string>("JobCode")
                        .HasColumnType("VARCHAR(25)");

                    b.Property<string>("LastName")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("Location")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("NameExtension")
                        .HasColumnType("VARCHAR(6)");

                    b.Property<string>("Pagibig")
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("PayrollCode")
                        .HasColumnType("VARCHAR(6)");

                    b.Property<string>("PhilHealth")
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("SSS")
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("Site")
                        .HasColumnType("VARCHAR(25)");

                    b.Property<string>("TIN")
                        .HasColumnType("VARCHAR(20)");

                    b.HasKey("EEId");

                    b.ToTable("masterlist");
                });

            modelBuilder.Entity("Pms.Masterlists.Entities.PayrollCode", b =>
                {
                    b.Property<string>("PayrollCodeId")
                        .HasColumnType("VARCHAR(12)");

                    b.Property<string>("CompanyId")
                        .IsRequired()
                        .HasColumnType("VARCHAR(35)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<byte>("Process")
                        .HasColumnType("TINYINT");

                    b.Property<string>("Site")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.HasKey("PayrollCodeId");

                    b.ToTable("PayrollCodes");
                });
#pragma warning restore 612, 618
        }
    }
}
