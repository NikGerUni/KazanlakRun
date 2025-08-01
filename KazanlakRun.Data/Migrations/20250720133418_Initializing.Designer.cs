﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KazanlakRun.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250720133418_Initializing")]
    partial class Initializing
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KazanlakRun.Data.Models.AidStation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.HasKey("Id");

                    b.ToTable("AidStations", t =>
                        {
                            t.HasCheckConstraint("CK_AidStations_Name_MinLength", "LEN([Name]) >= 6");

                            t.HasCheckConstraint("CK_AidStations_ShortName_MinLength", "LEN([ShortName]) >= 2");
                        });

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Aid stationn 1",
                            ShortName = "A1"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Aid stationn 2",
                            ShortName = "A2"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Aid stationn 3",
                            ShortName = "A3"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Aid stationn 4",
                            ShortName = "A4"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Aid stationn 5",
                            ShortName = "A5"
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.AidStationDistance", b =>
                {
                    b.Property<int>("AidStationId")
                        .HasColumnType("int");

                    b.Property<int>("DistanceId")
                        .HasColumnType("int");

                    b.HasKey("AidStationId", "DistanceId");

                    b.HasIndex("DistanceId");

                    b.ToTable("AidStationDistances");

                    b.HasData(
                        new
                        {
                            AidStationId = 1,
                            DistanceId = 1
                        },
                        new
                        {
                            AidStationId = 1,
                            DistanceId = 2
                        },
                        new
                        {
                            AidStationId = 1,
                            DistanceId = 3
                        },
                        new
                        {
                            AidStationId = 2,
                            DistanceId = 1
                        },
                        new
                        {
                            AidStationId = 2,
                            DistanceId = 2
                        },
                        new
                        {
                            AidStationId = 3,
                            DistanceId = 1
                        },
                        new
                        {
                            AidStationId = 3,
                            DistanceId = 3
                        },
                        new
                        {
                            AidStationId = 4,
                            DistanceId = 1
                        },
                        new
                        {
                            AidStationId = 5,
                            DistanceId = 1
                        },
                        new
                        {
                            AidStationId = 5,
                            DistanceId = 2
                        },
                        new
                        {
                            AidStationId = 5,
                            DistanceId = 3
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Distance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Distans")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegRunners")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Distances", t =>
                        {
                            t.HasCheckConstraint("CK_Distances_RegRunners_Range", "[RegRunners] >= 0 AND [RegRunners] <= 1000");
                        });

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Distans = "10 km",
                            RegRunners = 100
                        },
                        new
                        {
                            Id = 2,
                            Distans = "20 km",
                            RegRunners = 80
                        },
                        new
                        {
                            Id = 3,
                            Distans = "40 km",
                            RegRunners = 60
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Good", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Measure")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Quantity")
                        .HasColumnType("float");

                    b.Property<double>("QuantityOneRunner")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Goods");

                    b.HasData(
                        new
                        {
                            Id = 5,
                            Measure = "pcs",
                            Name = "Cups",
                            Quantity = 200.0,
                            QuantityOneRunner = 1.0
                        },
                        new
                        {
                            Id = 6,
                            Measure = "kg",
                            Name = "Salt",
                            Quantity = 1.0,
                            QuantityOneRunner = 0.0030000000000000001
                        },
                        new
                        {
                            Id = 7,
                            Measure = "l",
                            Name = "Drinking water",
                            Quantity = 100.0,
                            QuantityOneRunner = 0.5
                        },
                        new
                        {
                            Id = 8,
                            Measure = "l",
                            Name = "Isotonic",
                            Quantity = 50.0,
                            QuantityOneRunner = 0.20000000000000001
                        },
                        new
                        {
                            Id = 9,
                            Measure = "pcs",
                            Name = "Lemons",
                            Quantity = 100.0,
                            QuantityOneRunner = 0.5
                        },
                        new
                        {
                            Id = 10,
                            Measure = "kg",
                            Name = "Apples",
                            Quantity = 100.0,
                            QuantityOneRunner = 0.5
                        },
                        new
                        {
                            Id = 11,
                            Measure = "pcs",
                            Name = "Bananas",
                            Quantity = 100.0,
                            QuantityOneRunner = 0.5
                        },
                        new
                        {
                            Id = 12,
                            Measure = "pcs",
                            Name = "Chocolate",
                            Quantity = 50.0,
                            QuantityOneRunner = 0.01
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Roles", t =>
                        {
                            t.HasCheckConstraint("CK_Roles_Name_MinLength", "LEN([Name]) >= 3");
                        });

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "doctor"
                        },
                        new
                        {
                            Id = 2,
                            Name = "security"
                        },
                        new
                        {
                            Id = 3,
                            Name = "radio"
                        },
                        new
                        {
                            Id = 4,
                            Name = "food preparation"
                        },
                        new
                        {
                            Id = 5,
                            Name = "time recorder"
                        },
                        new
                        {
                            Id = 6,
                            Name = "general"
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Volunteer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AidStationId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Names")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AidStationId");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("IX_Volunteers_Email");

                    b.ToTable("Volunteers", null, t =>
                        {
                            t.HasCheckConstraint("CK_Volunteers_Names_MinLength", "LEN([Names]) >= 5");

                            t.HasCheckConstraint("CK_Volunteers_Phone_MinLength", "LEN([Phone]) >= 7");
                        });

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AidStationId = 1,
                            Email = "nnnnnn@nnn.bg",
                            Names = "Nikola Nikolov",
                            Phone = "0888998899"
                        },
                        new
                        {
                            Id = 2,
                            AidStationId = 1,
                            Email = "iiiiii@.iii.bg",
                            Names = "Ivan Ivanov",
                            Phone = "0888999999"
                        },
                        new
                        {
                            Id = 3,
                            AidStationId = 2,
                            Email = "gggggg@ggg.bg",
                            Names = "Georg Georgiev",
                            Phone = "0888777777"
                        },
                        new
                        {
                            Id = 4,
                            AidStationId = 2,
                            Email = "petarp@p.bg",
                            Names = "Petar Petrov",
                            Phone = "0888111111"
                        },
                        new
                        {
                            Id = 5,
                            AidStationId = 3,
                            Email = "maria@i.bg",
                            Names = "Maria Ivanova",
                            Phone = "0888222222"
                        },
                        new
                        {
                            Id = 6,
                            AidStationId = 3,
                            Email = "stoyand@d.bg",
                            Names = "Stoyan Dimitrov",
                            Phone = "0888333333"
                        },
                        new
                        {
                            Id = 7,
                            AidStationId = 4,
                            Email = "elena@p.bg",
                            Names = "Elena Petrova",
                            Phone = "0888444444"
                        },
                        new
                        {
                            Id = 8,
                            AidStationId = 4,
                            Email = "vlad@st.bg",
                            Names = "Vladimir Stoyanov",
                            Phone = "0888555555"
                        },
                        new
                        {
                            Id = 9,
                            AidStationId = 5,
                            Email = "tsveta@k.bg",
                            Names = "Tsveta Koleva",
                            Phone = "0888666666"
                        },
                        new
                        {
                            Id = 10,
                            AidStationId = 5,
                            Email = "nikolay@m.bg",
                            Names = "Nikolay Marinov",
                            Phone = "0888778778"
                        });
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.VolunteerRole", b =>
                {
                    b.Property<int>("VolunteerId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("VolunteerId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("VolunteerRoles");

                    b.HasData(
                        new
                        {
                            VolunteerId = 1,
                            RoleId = 1
                        },
                        new
                        {
                            VolunteerId = 1,
                            RoleId = 2
                        },
                        new
                        {
                            VolunteerId = 2,
                            RoleId = 2
                        },
                        new
                        {
                            VolunteerId = 2,
                            RoleId = 3
                        },
                        new
                        {
                            VolunteerId = 3,
                            RoleId = 3
                        },
                        new
                        {
                            VolunteerId = 3,
                            RoleId = 4
                        },
                        new
                        {
                            VolunteerId = 4,
                            RoleId = 4
                        },
                        new
                        {
                            VolunteerId = 4,
                            RoleId = 5
                        },
                        new
                        {
                            VolunteerId = 5,
                            RoleId = 5
                        },
                        new
                        {
                            VolunteerId = 5,
                            RoleId = 6
                        },
                        new
                        {
                            VolunteerId = 6,
                            RoleId = 6
                        },
                        new
                        {
                            VolunteerId = 6,
                            RoleId = 1
                        },
                        new
                        {
                            VolunteerId = 7,
                            RoleId = 1
                        },
                        new
                        {
                            VolunteerId = 7,
                            RoleId = 3
                        },
                        new
                        {
                            VolunteerId = 8,
                            RoleId = 2
                        },
                        new
                        {
                            VolunteerId = 8,
                            RoleId = 4
                        },
                        new
                        {
                            VolunteerId = 9,
                            RoleId = 3
                        },
                        new
                        {
                            VolunteerId = 9,
                            RoleId = 5
                        },
                        new
                        {
                            VolunteerId = 10,
                            RoleId = 4
                        },
                        new
                        {
                            VolunteerId = 10,
                            RoleId = 6
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "a1b2c3d4-e5f6-4782-8209-d76562e0feaa",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "f1e2d3c4-b5a6-4948-9309-c56782b0faab",
                            Name = "User",
                            NormalizedName = "USER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "7699db7d-964f-4782-8209-d76562e0fece",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "d11ae180-5c85-4fd3-9981-15ed578de3cd",
                            Email = "admin@KazanlakRun.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@KAZANLAKRUN.COM",
                            NormalizedUserName = "ADMIN@KAZANLAKRUN.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAEEnI9dENyxCl+AiY2hAY0y2hf7O1GEwY31AxcyyTDFQr4P31glpk2MVHhzEr+FNgRQ==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "b4facfba-8503-4bce-9ca4-59cae95fc0cf",
                            TwoFactorEnabled = false,
                            UserName = "admin@KazanlakRun.com"
                        },
                        new
                        {
                            Id = "11111111-2222-3333-4444-555555555555",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "19a94845-084e-4c0d-9525-b1aecb030b73",
                            Email = "user@abv.bg",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "USER@ABV.BG",
                            NormalizedUserName = "USER@ABV.BG",
                            PasswordHash = "AQAAAAIAAYagAAAAEPhoYogeejGn7aQiNdXn1IkhNOlKgTL8bBZVJirC+ajokBp6L3zqqBO61tsuS34m5Q==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "d126a509-9f12-4a13-b4c8-d84a58e8472b",
                            TwoFactorEnabled = false,
                            UserName = "user@abv.bg"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "7699db7d-964f-4782-8209-d76562e0fece",
                            RoleId = "a1b2c3d4-e5f6-4782-8209-d76562e0feaa"
                        },
                        new
                        {
                            UserId = "11111111-2222-3333-4444-555555555555",
                            RoleId = "f1e2d3c4-b5a6-4948-9309-c56782b0faab"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.AidStationDistance", b =>
                {
                    b.HasOne("KazanlakRun.Data.Models.AidStation", "AidStation")
                        .WithMany("AidStationDistances")
                        .HasForeignKey("AidStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KazanlakRun.Data.Models.Distance", "Distance")
                        .WithMany("AidStationDistances")
                        .HasForeignKey("DistanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AidStation");

                    b.Navigation("Distance");
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Volunteer", b =>
                {
                    b.HasOne("KazanlakRun.Data.Models.AidStation", "AidStation")
                        .WithMany("Volunteers")
                        .HasForeignKey("AidStationId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("AidStation");
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.VolunteerRole", b =>
                {
                    b.HasOne("KazanlakRun.Data.Models.Role", "Role")
                        .WithMany("VolunteerRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KazanlakRun.Data.Models.Volunteer", "Volunteer")
                        .WithMany("VolunteerRoles")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Volunteer");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.AidStation", b =>
                {
                    b.Navigation("AidStationDistances");

                    b.Navigation("Volunteers");
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Distance", b =>
                {
                    b.Navigation("AidStationDistances");
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Role", b =>
                {
                    b.Navigation("VolunteerRoles");
                });

            modelBuilder.Entity("KazanlakRun.Data.Models.Volunteer", b =>
                {
                    b.Navigation("VolunteerRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
