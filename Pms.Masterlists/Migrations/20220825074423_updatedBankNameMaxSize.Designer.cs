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
    [Migration("20220825074423_updatedBankNameMaxSize")]
    partial class updatedBankNameMaxSize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("Pms.Masterlists.Entities.Employee", b =>
                {
                    b.Property<string>("EEId")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("VARCHAR(30)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("BankCategory")
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("BankName")
                        .HasColumnType("VARCHAR(30)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("DATE");

                    b.Property<string>("CardNumber")
                        .HasColumnType("VARCHAR(30)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("DATETIME");

                    b.Property<string>("FirstName")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("LastName")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("Location")
                        .HasColumnType("VARCHAR(45)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("VARCHAR(45)");

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
#pragma warning restore 612, 618
        }
    }
}
