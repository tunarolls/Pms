﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pms.Timesheets.Persistence;

namespace Pms.Timesheets.Migrations
{
    [DbContext(typeof(TimesheetDbContext))]
    partial class TimesheetDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.13");

            modelBuilder.Entity("Pms.Timesheets.EmployeeView", b =>
                {
                    b.Property<string>("EEId")
                        .HasColumnType("varchar(767)");

                    b.Property<int>("Bank")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NameExtension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PayrollCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EEId");

                    b.ToView("masterlist");
                });

            modelBuilder.Entity("Pms.Timesheets.Timesheet", b =>
                {
                    b.Property<string>("TimesheetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(35)")
                        .HasColumnName("id");

                    b.Property<double>("Adjust1")
                        .HasColumnType("DOUBLE(8,2)");

                    b.Property<double>("Adjust2")
                        .HasColumnType("DOUBLE(8,2)");

                    b.Property<double>("Allowance")
                        .HasColumnType("DOUBLE(8,2)");

                    b.Property<string>("CutoffId")
                        .IsRequired()
                        .HasColumnType("VARCHAR(6)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("EEId")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<byte>("IsConfirmed")
                        .HasColumnType("TINYINT");

                    b.Property<short>("Page")
                        .HasColumnType("SMALLINT")
                        .HasComment("Time System API Page");

                    b.Property<string>("RawPCV")
                        .IsRequired()
                        .HasColumnType("VARCHAR(255)");

                    b.Property<double>("TotalHOT")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<double>("TotalHours")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<double>("TotalND")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<double>("TotalOT")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<double>("TotalRDOT")
                        .HasColumnType("DOUBLE(6,2)");

                    b.Property<double>("TotalTardy")
                        .HasColumnType("DOUBLE(6,2)");

                    b.HasKey("TimesheetId");

                    b.HasIndex("EEId");

                    b.ToTable("timesheet");
                });

            modelBuilder.Entity("Pms.Timesheets.Timesheet", b =>
                {
                    b.HasOne("Pms.Timesheets.EmployeeView", "EE")
                        .WithMany()
                        .HasForeignKey("EEId");

                    b.Navigation("EE");
                });
#pragma warning restore 612, 618
        }
    }
}
