﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PettySQLite;

#nullable disable

namespace PettySQLite.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("PettySQLite.Models.BaseSettings", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InformationPerceptionMode")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHapticFeedback")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("BaseSettings");
                });

            modelBuilder.Entity("PettySQLite.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BaseSettingsId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VoiceSettingsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BaseSettingsId");

                    b.HasIndex("VoiceSettingsId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("PettySQLite.Models.VoiceSettings", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Pitch")
                        .HasColumnType("REAL");

                    b.Property<float>("Volume")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("VoiceSettings");
                });

            modelBuilder.Entity("PettySQLite.Models.Settings", b =>
                {
                    b.HasOne("PettySQLite.Models.BaseSettings", "BaseSettings")
                        .WithMany()
                        .HasForeignKey("BaseSettingsId");

                    b.HasOne("PettySQLite.Models.VoiceSettings", "VoiceSettings")
                        .WithMany()
                        .HasForeignKey("VoiceSettingsId");

                    b.Navigation("BaseSettings");

                    b.Navigation("VoiceSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
