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
    [Migration("20230126095304_payroll_code_table_name")]
    partial class payroll_code_table_name
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.13");

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
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("RegisteredName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("Site")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("TIN")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.HasKey("CompanyId");

                    b.ToTable("company");
                });

            modelBuilder.Entity("Pms.Masterlists.Entities.Employee", b =>
                {
                    b.Property<string>("EEId")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("VARCHAR(30)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte>("Bank")
                        .HasColumnType("TINYINT");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("DATE");

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasColumnType("VARCHAR(30)");

                    b.Property<string>("CompanyId")
                        .IsRequired()
                        .HasColumnType("VARCHAR(25)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateHired")
                        .HasColumnType("DATE");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("DATETIME");

                    b.Property<DateTime>("DateResigned")
                        .HasColumnType("DATE");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("VARCHAR(1)");

                    b.Property<string>("JobCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR(25)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("NameExtension")
                        .IsRequired()
                        .HasColumnType("VARCHAR(6)");

                    b.Property<string>("Pagibig")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("PayrollCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR(6)");

                    b.Property<string>("PhilHealth")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("SSS")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("Site")
                        .IsRequired()
                        .HasColumnType("VARCHAR(25)");

                    b.Property<string>("TIN")
                        .IsRequired()
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

                    b.ToTable("payrollcodes");
                });
#pragma warning restore 612, 618
        }
    }
}
