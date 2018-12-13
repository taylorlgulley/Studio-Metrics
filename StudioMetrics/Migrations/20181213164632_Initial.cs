using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudioMetrics.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectType",
                columns: table => new
                {
                    ProjectTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectType", x => x.ProjectTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StatusType",
                columns: table => new
                {
                    StatusTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusType", x => x.StatusTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                name: "Artist",
                columns: table => new
                {
                    ArtistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.ArtistId);
                    table.ForeignKey(
                        name: "FK_Artist_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Instrument = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_Player_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientArtist",
                columns: table => new
                {
                    ClientArtistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    ArtistId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientArtist", x => x.ClientArtistId);
                    table.ForeignKey(
                        name: "FK_ClientArtist_Artist_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artist",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientArtist_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 55, nullable: false),
                    ProjectTypeId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    Payrate = table.Column<double>(nullable: false),
                    TimeTable = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StatusTypeId = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Project_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_ProjectType_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "ProjectType",
                        principalColumn: "ProjectTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArtistProject",
                columns: table => new
                {
                    ArtistProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArtistId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistProject", x => x.ArtistProjectId);
                    table.ForeignKey(
                        name: "FK_ArtistProject_Artist_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artist",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArtistProject_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProject",
                columns: table => new
                {
                    PlayerProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProject", x => x.PlayerProjectId);
                    table.ForeignKey(
                        name: "FK_PlayerProject_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerProject_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "CompanyName" },
                values: new object[] { "6f6975c7-6999-42bc-90c1-9cb207231270", 0, "96c746a0-1b55-444b-84ae-3c43b6d4855b", "ApplicationUser", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEIo7TvX+oNRATXWXGqfLIfmVIAzvCleO2pIa6RbYdi9fRLtkOnK47eCSTHVXYZHIIQ==", null, false, "4229f397-1454-4b61-9e08-128384e7d664", false, "admin@admin.com", "admin" });

            migrationBuilder.InsertData(
                table: "ProjectType",
                columns: new[] { "ProjectTypeId", "Type" },
                values: new object[,]
                {
                    { 1, "Single Song Release" },
                    { 2, "Extended Play CD" },
                    { 3, "Full Length CD" },
                    { 4, "Single Song Demo" },
                    { 5, "30 Second Audio Commercial" },
                    { 6, "60 Second Audio Commercial" },
                    { 7, "Audio Industrial" },
                    { 8, "Live Concert Recording" }
                });

            migrationBuilder.InsertData(
                table: "StatusType",
                columns: new[] { "StatusTypeId", "Type" },
                values: new object[,]
                {
                    { 1, "Upcoming" },
                    { 2, "Completed" },
                    { 3, "Current" },
                    { 4, "Tentative" }
                });

            migrationBuilder.InsertData(
                table: "Artist",
                columns: new[] { "ArtistId", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, "Smashing Pumpkins", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 2, "Lake Street Dive", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 3, "Barns Courtney", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 4, "The Pink Spiders", "6f6975c7-6999-42bc-90c1-9cb207231270" }
                });

            migrationBuilder.InsertData(
                table: "Client",
                columns: new[] { "ClientId", "Email", "Name", "Phone", "UserId" },
                values: new object[,]
                {
                    { 1, "mhaleindustries@mhale.com", "Mark Hale", "615-111-1111", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 2, "dcunningham@dcunningham.com", "David Cunningham", "615-222-2222", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 3, "mansound@sound.com", "Michelle An", "615-333-3333", "6f6975c7-6999-42bc-90c1-9cb207231270" }
                });

            migrationBuilder.InsertData(
                table: "Player",
                columns: new[] { "PlayerId", "Email", "FirstName", "Instrument", "LastName", "Phone", "UserId" },
                values: new object[,]
                {
                    { 1, "abelaboriel@paulmcaartney.com", "Abe", "Drums", "Laboriel Jr.", "999-999-9999", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 2, "paul@paulmccartney.com", "Paul", "Vocals", "McCartney", "999-999-9999", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 3, "jhendrix@jhendrix.com", "Jimmy", "Guitar", "Hendrix", "999-999-9999", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 4, "swonder@swonder.com", "Stevie", "Keyboards", "Wonder", "999-999-9999", "6f6975c7-6999-42bc-90c1-9cb207231270" }
                });

            migrationBuilder.InsertData(
                table: "ClientArtist",
                columns: new[] { "ClientArtistId", "ArtistId", "ClientId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "ProjectId", "ClientId", "Description", "Payrate", "ProjectTypeId", "StartDate", "StatusTypeId", "TimeTable", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "An hit in the making", 2500.0, 1, new DateTime(2018, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, "Hey Jude", "6f6975c7-6999-42bc-90c1-9cb207231270" },
                    { 2, 3, "A great album", 1500.0, 3, new DateTime(2018, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 25, "Free Yourself Up", "6f6975c7-6999-42bc-90c1-9cb207231270" }
                });

            migrationBuilder.InsertData(
                table: "ArtistProject",
                columns: new[] { "ArtistProjectId", "ArtistId", "ProjectId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "PlayerProject",
                columns: new[] { "PlayerProjectId", "PlayerId", "ProjectId" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 1, 1 },
                    { 3, 4, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artist_UserId",
                table: "Artist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProject_ArtistId",
                table: "ArtistProject",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProject_ProjectId",
                table: "ArtistProject",
                column: "ProjectId");

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
                name: "IX_Client_UserId",
                table: "Client",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientArtist_ArtistId",
                table: "ClientArtist",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientArtist_ClientId",
                table: "ClientArtist",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_UserId",
                table: "Player",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProject_PlayerId",
                table: "PlayerProject",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProject_ProjectId",
                table: "PlayerProject",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ClientId",
                table: "Project",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectTypeId",
                table: "Project",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_StatusTypeId",
                table: "Project",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId",
                table: "Project",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistProject");

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
                name: "ClientArtist");

            migrationBuilder.DropTable(
                name: "PlayerProject");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Artist");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "ProjectType");

            migrationBuilder.DropTable(
                name: "StatusType");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
