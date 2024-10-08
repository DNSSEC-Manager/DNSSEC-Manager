﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240919133703_AddDefaultUserAccount")]
    partial class AddDefaultUserAccount
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Backend.Models.Algorithm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Bits")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Algorithms");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Bits = 2048,
                            Description = "Required algorithm, works with all bit sizes",
                            Disabled = true,
                            IsDefault = false,
                            Name = "RSASHA1",
                            Number = 5
                        },
                        new
                        {
                            Id = 2,
                            Bits = 2048,
                            Description = "Works with all bit sizes",
                            Disabled = false,
                            IsDefault = false,
                            Name = "RSASHA256",
                            Number = 8
                        },
                        new
                        {
                            Id = 3,
                            Bits = 256,
                            Description = "(Default) Works with 256 bit size only",
                            Disabled = false,
                            IsDefault = true,
                            Name = "ECDSAP256SHA256",
                            Number = 13
                        },
                        new
                        {
                            Id = 4,
                            Bits = 384,
                            Description = "Works with 384 bit size only",
                            Disabled = false,
                            IsDefault = false,
                            Name = "ECDSAP384SHA384",
                            Number = 14
                        },
                        new
                        {
                            Id = 5,
                            Bits = 256,
                            Description = "Works with 256 bit size only",
                            Disabled = false,
                            IsDefault = false,
                            Name = "ED25519",
                            Number = 15
                        },
                        new
                        {
                            Id = 6,
                            Bits = 2048,
                            Description = "Currently not supported in PowerDNS",
                            Disabled = false,
                            IsDefault = false,
                            Name = "ED448",
                            Number = 16
                        });
                });

            modelBuilder.Entity("Backend.Models.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Configs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Key = "DomainChangesRunEvery",
                            Value = "86400000"
                        },
                        new
                        {
                            Id = 2,
                            Key = "DomainChangesDefaultRuntime",
                            Value = "16:00"
                        },
                        new
                        {
                            Id = 3,
                            Key = "CheckAllDomainsRunEvery",
                            Value = "60000"
                        },
                        new
                        {
                            Id = 4,
                            Key = "CheckAllDomainsReRunAfter",
                            Value = "86400000"
                        },
                        new
                        {
                            Id = 5,
                            Key = "CheckAlldomainsPerRun",
                            Value = "50"
                        },
                        new
                        {
                            Id = 6,
                            Key = "AutomaticSign",
                            Value = "false"
                        },
                        new
                        {
                            Id = 7,
                            Key = "AutomaticFix",
                            Value = "false"
                        },
                        new
                        {
                            Id = 8,
                            Key = "DomainSignFailRerun",
                            Value = "86400000"
                        },
                        new
                        {
                            Id = 9,
                            Key = "DefaultAlgorithm",
                            Value = "3"
                        },
                        new
                        {
                            Id = 10,
                            Key = "DomainPageSize",
                            Value = "50"
                        },
                        new
                        {
                            Id = 11,
                            Key = "AutomaticKeyRollover",
                            Value = "false"
                        },
                        new
                        {
                            Id = 12,
                            Key = "KeyRolloverTime",
                            Value = "86400"
                        },
                        new
                        {
                            Id = 13,
                            Key = "DefaultTtl",
                            Value = "86400"
                        });
                });

            modelBuilder.Entity("Backend.Models.Cryptokey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Algo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DomainId")
                        .HasColumnType("int");

                    b.Property<string>("Flag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KeyTag")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.ToTable("Cryptokeys");
                });

            modelBuilder.Entity("Backend.Models.DnsRecordSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DomainId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastCheckedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Records")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.ToTable("DnsRecordSet");
                });

            modelBuilder.Entity("Backend.Models.DnsServer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AuthToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BaseUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServerType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DnsServers");
                });

            modelBuilder.Entity("Backend.Models.Domain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CustomRegistryId")
                        .HasColumnType("int");

                    b.Property<int>("DnsServerId")
                        .HasColumnType("int");

                    b.Property<bool>("ExcludeSigning")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReservedByScheduler")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastChecked")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NameServerGroupId")
                        .HasColumnType("int");

                    b.Property<bool>("RemovedFromDnsServer")
                        .HasColumnType("bit");

                    b.Property<bool>("SignMatch")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("SignedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TopLevelDomainId")
                        .HasColumnType("int");

                    b.Property<int>("Ttl")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CustomRegistryId");

                    b.HasIndex("DnsServerId");

                    b.HasIndex("NameServerGroupId");

                    b.HasIndex("TopLevelDomainId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("Backend.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CryptokeyId")
                        .HasColumnType("int");

                    b.Property<int?>("DnsServerId")
                        .HasColumnType("int");

                    b.Property<int?>("DomainId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPermanent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RunAfter")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Step")
                        .HasColumnType("int");

                    b.Property<int>("Task")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CryptokeyId");

                    b.HasIndex("DnsServerId");

                    b.HasIndex("DomainId");

                    b.ToTable("Jobs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsCompleted = false,
                            IsPermanent = true,
                            IsSuccessful = false,
                            RunAfter = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Task = 3,
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("Backend.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DnsServerId")
                        .HasColumnType("int");

                    b.Property<int?>("DomainId")
                        .HasColumnType("int");

                    b.Property<int?>("JobId")
                        .HasColumnType("int");

                    b.Property<int>("LogType")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RawMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RegistryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DnsServerId");

                    b.HasIndex("DomainId");

                    b.HasIndex("JobId");

                    b.HasIndex("RegistryId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Backend.Models.NameServer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NameServerGroupId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NameServerGroupId");

                    b.ToTable("NameServers");
                });

            modelBuilder.Entity("Backend.Models.NameServerGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("DnsServerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DnsServerId");

                    b.ToTable("NameServerGroups");
                });

            modelBuilder.Entity("Backend.Models.Registry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegistryType")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Registries");
                });

            modelBuilder.Entity("Backend.Models.TopLevelDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("OverrideAlgorithmId")
                        .HasColumnType("int");

                    b.Property<string>("Tld")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OverrideAlgorithmId");

                    b.ToTable("TopLevelDomains");
                });

            modelBuilder.Entity("Backend.Models.TopLevelDomainRegistry", b =>
                {
                    b.Property<int>("RegistryId")
                        .HasColumnType("int");

                    b.Property<int>("TopLevelDomainId")
                        .HasColumnType("int");

                    b.HasKey("RegistryId", "TopLevelDomainId");

                    b.HasIndex("TopLevelDomainId");

                    b.ToTable("TopLevelDomainRegistries");
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
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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

            modelBuilder.Entity("Backend.Models.Cryptokey", b =>
                {
                    b.HasOne("Backend.Models.Domain", "Domain")
                        .WithMany("Cryptokeys")
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("Backend.Models.DnsRecordSet", b =>
                {
                    b.HasOne("Backend.Models.Domain", "Domain")
                        .WithMany("DnsRecordSets")
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("Backend.Models.Domain", b =>
                {
                    b.HasOne("Backend.Models.Registry", "Registry")
                        .WithMany("Domains")
                        .HasForeignKey("CustomRegistryId");

                    b.HasOne("Backend.Models.DnsServer", "DnsServer")
                        .WithMany("Domains")
                        .HasForeignKey("DnsServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.NameServerGroup", "NameServerGroup")
                        .WithMany("Domains")
                        .HasForeignKey("NameServerGroupId");

                    b.HasOne("Backend.Models.TopLevelDomain", "TopLevelDomain")
                        .WithMany("Domains")
                        .HasForeignKey("TopLevelDomainId");

                    b.Navigation("DnsServer");

                    b.Navigation("NameServerGroup");

                    b.Navigation("Registry");

                    b.Navigation("TopLevelDomain");
                });

            modelBuilder.Entity("Backend.Models.Job", b =>
                {
                    b.HasOne("Backend.Models.Cryptokey", "Cryptokey")
                        .WithMany("Jobs")
                        .HasForeignKey("CryptokeyId");

                    b.HasOne("Backend.Models.DnsServer", "DnsServer")
                        .WithMany("Jobs")
                        .HasForeignKey("DnsServerId");

                    b.HasOne("Backend.Models.Domain", "Domain")
                        .WithMany("Jobs")
                        .HasForeignKey("DomainId");

                    b.Navigation("Cryptokey");

                    b.Navigation("DnsServer");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("Backend.Models.Log", b =>
                {
                    b.HasOne("Backend.Models.DnsServer", "DnsServer")
                        .WithMany()
                        .HasForeignKey("DnsServerId");

                    b.HasOne("Backend.Models.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("Backend.Models.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId");

                    b.HasOne("Backend.Models.Registry", "Registry")
                        .WithMany()
                        .HasForeignKey("RegistryId");

                    b.Navigation("DnsServer");

                    b.Navigation("Domain");

                    b.Navigation("Job");

                    b.Navigation("Registry");
                });

            modelBuilder.Entity("Backend.Models.NameServer", b =>
                {
                    b.HasOne("Backend.Models.NameServerGroup", "NameServerGroup")
                        .WithMany("NameServers")
                        .HasForeignKey("NameServerGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NameServerGroup");
                });

            modelBuilder.Entity("Backend.Models.NameServerGroup", b =>
                {
                    b.HasOne("Backend.Models.DnsServer", "DnsServer")
                        .WithMany("NameServerGroups")
                        .HasForeignKey("DnsServerId");

                    b.Navigation("DnsServer");
                });

            modelBuilder.Entity("Backend.Models.TopLevelDomain", b =>
                {
                    b.HasOne("Backend.Models.Algorithm", "Algorithm")
                        .WithMany()
                        .HasForeignKey("OverrideAlgorithmId");

                    b.Navigation("Algorithm");
                });

            modelBuilder.Entity("Backend.Models.TopLevelDomainRegistry", b =>
                {
                    b.HasOne("Backend.Models.Registry", "Registry")
                        .WithMany("TopLevelDomainRegistries")
                        .HasForeignKey("RegistryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.TopLevelDomain", "TopLevelDomain")
                        .WithMany("TopLevelDomainRegistries")
                        .HasForeignKey("TopLevelDomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Registry");

                    b.Navigation("TopLevelDomain");
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

            modelBuilder.Entity("Backend.Models.Cryptokey", b =>
                {
                    b.Navigation("Jobs");
                });

            modelBuilder.Entity("Backend.Models.DnsServer", b =>
                {
                    b.Navigation("Domains");

                    b.Navigation("Jobs");

                    b.Navigation("NameServerGroups");
                });

            modelBuilder.Entity("Backend.Models.Domain", b =>
                {
                    b.Navigation("Cryptokeys");

                    b.Navigation("DnsRecordSets");

                    b.Navigation("Jobs");
                });

            modelBuilder.Entity("Backend.Models.NameServerGroup", b =>
                {
                    b.Navigation("Domains");

                    b.Navigation("NameServers");
                });

            modelBuilder.Entity("Backend.Models.Registry", b =>
                {
                    b.Navigation("Domains");

                    b.Navigation("TopLevelDomainRegistries");
                });

            modelBuilder.Entity("Backend.Models.TopLevelDomain", b =>
                {
                    b.Navigation("Domains");

                    b.Navigation("TopLevelDomainRegistries");
                });
#pragma warning restore 612, 618
        }
    }
}
