using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bits = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DnsServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServerType = table.Column<int>(type: "int", nullable: false),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    RegistryType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TopLevelDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tld = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverrideAlgorithmId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopLevelDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopLevelDomains_Algorithms_OverrideAlgorithmId",
                        column: x => x.OverrideAlgorithmId,
                        principalTable: "Algorithms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NameServerGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DnsServerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameServerGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NameServerGroups_DnsServers_DnsServerId",
                        column: x => x.DnsServerId,
                        principalTable: "DnsServers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TopLevelDomainRegistries",
                columns: table => new
                {
                    TopLevelDomainId = table.Column<int>(type: "int", nullable: false),
                    RegistryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopLevelDomainRegistries", x => new { x.RegistryId, x.TopLevelDomainId });
                    table.ForeignKey(
                        name: "FK_TopLevelDomainRegistries_Registries_RegistryId",
                        column: x => x.RegistryId,
                        principalTable: "Registries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopLevelDomainRegistries_TopLevelDomains_TopLevelDomainId",
                        column: x => x.TopLevelDomainId,
                        principalTable: "TopLevelDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DnsServerId = table.Column<int>(type: "int", nullable: false),
                    CustomRegistryId = table.Column<int>(type: "int", nullable: true),
                    NameServerGroupId = table.Column<int>(type: "int", nullable: true),
                    RemovedFromDnsServer = table.Column<bool>(type: "bit", nullable: false),
                    SignMatch = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeSigning = table.Column<bool>(type: "bit", nullable: false),
                    IsReservedByScheduler = table.Column<bool>(type: "bit", nullable: false),
                    Ttl = table.Column<int>(type: "int", nullable: false),
                    TopLevelDomainId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChecked = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_DnsServers_DnsServerId",
                        column: x => x.DnsServerId,
                        principalTable: "DnsServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Domains_NameServerGroups_NameServerGroupId",
                        column: x => x.NameServerGroupId,
                        principalTable: "NameServerGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Domains_Registries_CustomRegistryId",
                        column: x => x.CustomRegistryId,
                        principalTable: "Registries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Domains_TopLevelDomains_TopLevelDomainId",
                        column: x => x.TopLevelDomainId,
                        principalTable: "TopLevelDomains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NameServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameServerGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NameServers_NameServerGroups_NameServerGroupId",
                        column: x => x.NameServerGroupId,
                        principalTable: "NameServerGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cryptokeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Algo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cryptokeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cryptokeys_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnsRecordSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    Records = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsRecordSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnsRecordSet_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DnsServerId = table.Column<int>(type: "int", nullable: true),
                    DomainId = table.Column<int>(type: "int", nullable: true),
                    CryptokeyId = table.Column<int>(type: "int", nullable: true),
                    Task = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: true),
                    IsPermanent = table.Column<bool>(type: "bit", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RunAfter = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Cryptokeys_CryptokeyId",
                        column: x => x.CryptokeyId,
                        principalTable: "Cryptokeys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jobs_DnsServers_DnsServerId",
                        column: x => x.DnsServerId,
                        principalTable: "DnsServers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jobs_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: true),
                    DnsServerId = table.Column<int>(type: "int", nullable: true),
                    RegistryId = table.Column<int>(type: "int", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_DnsServers_DnsServerId",
                        column: x => x.DnsServerId,
                        principalTable: "DnsServers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Logs_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Logs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Logs_Registries_RegistryId",
                        column: x => x.RegistryId,
                        principalTable: "Registries",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Algorithms",
                columns: new[] { "Id", "Bits", "Description", "Disabled", "IsDefault", "Name", "Number" },
                values: new object[,]
                {
                    { 1, 2048, "Required algorithm, works with all bit sizes", true, false, "RSASHA1", 5 },
                    { 2, 2048, "Works with all bit sizes", false, false, "RSASHA256", 8 },
                    { 3, 256, "(Default) Works with 256 bit size only", false, true, "ECDSAP256SHA256", 13 },
                    { 4, 384, "Works with 384 bit size only", false, false, "ECDSAP384SHA384", 14 },
                    { 5, 256, "Works with 256 bit size only", false, false, "ED25519", 15 },
                    { 6, 2048, "Currently not supported in PowerDNS", false, false, "ED448", 16 }
                });

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Id", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "DomainChangesRunEvery", "86400000" },
                    { 2, "DomainChangesDefaultRuntime", "16:00" },
                    { 3, "CheckAllDomainsRunEvery", "60000" },
                    { 4, "CheckAllDomainsReRunAfter", "86400000" },
                    { 5, "CheckAlldomainsPerRun", "50" },
                    { 6, "AutomaticSign", "false" },
                    { 7, "AutomaticFix", "false" },
                    { 8, "DomainSignFailRerun", "86400000" },
                    { 9, "DefaultAlgorithm", "3" },
                    { 10, "DomainPageSize", "50" },
                    { 11, "AutomaticKeyRollover", "false" },
                    { 12, "KeyRolloverTime", "86400" },
                    { 13, "DefaultTtl", "86400" }
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CreatedAt", "CryptokeyId", "DnsServerId", "DomainId", "IsCompleted", "IsPermanent", "IsSuccessful", "RunAfter", "Step", "Task", "UpdatedAt" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cryptokeys_DomainId",
                table: "Cryptokeys",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecordSet_DomainId",
                table: "DnsRecordSet",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_CustomRegistryId",
                table: "Domains",
                column: "CustomRegistryId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_DnsServerId",
                table: "Domains",
                column: "DnsServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_NameServerGroupId",
                table: "Domains",
                column: "NameServerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_TopLevelDomainId",
                table: "Domains",
                column: "TopLevelDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CryptokeyId",
                table: "Jobs",
                column: "CryptokeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DnsServerId",
                table: "Jobs",
                column: "DnsServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DomainId",
                table: "Jobs",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_DnsServerId",
                table: "Logs",
                column: "DnsServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_DomainId",
                table: "Logs",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_JobId",
                table: "Logs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RegistryId",
                table: "Logs",
                column: "RegistryId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServerGroups_DnsServerId",
                table: "NameServerGroups",
                column: "DnsServerId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_NameServerGroupId",
                table: "NameServers",
                column: "NameServerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TopLevelDomainRegistries_TopLevelDomainId",
                table: "TopLevelDomainRegistries",
                column: "TopLevelDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_TopLevelDomains_OverrideAlgorithmId",
                table: "TopLevelDomains",
                column: "OverrideAlgorithmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "DnsRecordSet");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "NameServers");

            migrationBuilder.DropTable(
                name: "TopLevelDomainRegistries");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Cryptokeys");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "NameServerGroups");

            migrationBuilder.DropTable(
                name: "Registries");

            migrationBuilder.DropTable(
                name: "TopLevelDomains");

            migrationBuilder.DropTable(
                name: "DnsServers");

            migrationBuilder.DropTable(
                name: "Algorithms");
        }
    }
}
