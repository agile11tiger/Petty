﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PettySQLite;

#nullable disable

namespace PettySQLite.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230901173916_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("PettySQLite.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("LanguageType")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("TryHarder")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("TryInverted")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("UseFrontCamera")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Settings", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
