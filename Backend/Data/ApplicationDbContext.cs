using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Config>().HasData(
                new Config
                {
                    Id = 1,
                    Key = "DomainChangesRunEvery",
                    Value = "86400000"
                },
                new Config
                {
                    Id = 2,
                    Key = "DomainChangesDefaultRuntime",
                    Value = "16:00"
                },
                new Config
                {
                    Id = 3,
                    Key = "CheckAllDomainsRunEvery",
                    Value = "60000"
                },
                new Config
                {
                    Id = 4,
                    Key = "CheckAllDomainsReRunAfter",
                    Value = "86400000"
                },
                new Config
                {
                    Id = 5,
                    Key = "CheckAlldomainsPerRun",
                    Value = "50"
                },
                new Config
                {
                    Id = 6,
                    Key = "AutomaticSign",
                    Value = "false"
                },
                new Config
                {
                    Id = 7,
                    Key = "AutomaticFix",
                    Value = "false"
                },
                new Config
                {
                    Id = 8,
                    Key = "DomainSignFailRerun",
                    Value = "86400000"
                },
                new Config
                {
                    Id = 9,
                    Key = "DefaultAlgorithm",
                    Value = "3"
                },
                new Config
                {
                    Id = 10,
                    Key = "DomainPageSize",
                    Value = "50"
                },
                new Config
                {
                    Id = 11,
                    Key = "AutomaticKeyRollover",
                    Value = "false"
                },
                new Config
                {
                    Id = 12,
                    Key = "KeyRolloverTime",
                    Value = "86400"
                },
                new Config
                {
                    Id = 13,
                    Key = "DefaultTtl",
                    Value = "86400"
                }
            );

            modelBuilder.Entity<Job>().HasData(
                new Job
                {
                    Id = 1,
                    IsPermanent = true,
                    Task = JobName.CheckDomain
                }
            );
            modelBuilder.Entity<TopLevelDomainRegistry>()
                .HasKey(t => new {t.RegistryId, t.TopLevelDomainId});

            modelBuilder.Entity<Algorithm>().HasData(
                new Algorithm
                {
                    Id = 1,
                    Number = 5,
                    Name = "RSASHA1",
                    Description = "Required algorithm, works with all bit sizes",
                    Bits = 2048,
                    IsDefault = false,
                    Disabled = true
                },
                new Algorithm
                {
                    Id = 2,
                    Number = 8,
                    Name = "RSASHA256",
                    Description = "Works with all bit sizes",
                    Bits = 2048,
                    IsDefault = false,
                    Disabled = false
                },
                new Algorithm
                {
                    Id = 3,
                    Number = 13,
                    Name = "ECDSAP256SHA256",
                    Description = "(Default) Works with 256 bit size only",
                    Bits = 256,
                    IsDefault = true,
                    Disabled = false
                },
                new Algorithm
                {
                    Id = 4,
                    Number = 14,
                    Name = "ECDSAP384SHA384",
                    Description = "Works with 384 bit size only",
                    Bits = 384,
                    IsDefault = false,
                    Disabled = false
                },
                new Algorithm
                {
                    Id = 5,
                    Number = 15,
                    Name = "ED25519",
                    Description = "Works with 256 bit size only",
                    Bits = 256,
                    IsDefault = false,
                    Disabled = false
                },
                new Algorithm
                {
                    Id = 6,
                    Number = 16,
                    Name = "ED448",
                    Description = "Currently not supported in PowerDNS",
                    Bits = 2048,
                    IsDefault = false,
                    Disabled = false
                });
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain> Domains { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<DnsServer> DnsServers { get; set; }
        public DbSet<NameServerGroup> NameServerGroups { get; set; }
        public DbSet<NameServer> NameServers { get; set; }
        public DbSet<Registry> Registries { get; set; }
        public DbSet<TopLevelDomain> TopLevelDomains { get; set; }
        public DbSet<Cryptokey> Cryptokeys { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<TopLevelDomainRegistry> TopLevelDomainRegistries { get; set; }
        public DbSet<Algorithm> Algorithms { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
