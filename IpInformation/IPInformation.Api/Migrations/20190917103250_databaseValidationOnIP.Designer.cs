﻿// <auto-generated />
using System;
using IPInformation.Api.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IPInformation.Api.Migrations
{
    [DbContext(typeof(SqlDbContext))]
    [Migration("20190917103250_databaseValidationOnIP")]
    partial class databaseValidationOnIP
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IPInformation.Api.Models.IPDetailsExtended", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Continent");

                    b.Property<string>("Country");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Ip")
                        .IsRequired();

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.HasKey("Id");

                    b.HasIndex("Ip")
                        .IsUnique();

                    b.ToTable("IPDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
