using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigKompassen.Web.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_870 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaGalleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaGalleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                name: "MediaLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaLinks_AspNetUsers_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    ProfileType = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistProfile_Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArtistProfile_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VenueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    SceneProfile_Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SceneProfile_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amenities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningHours = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MediaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GalleryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaItems_MediaGalleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "MediaGalleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaItems_MediaLinks_LinkId",
                        column: x => x.LinkId,
                        principalTable: "MediaLinks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaLinkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageContents_MediaLinks_MediaLinkId",
                        column: x => x.MediaLinkId,
                        principalTable: "MediaLinks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArtistMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistMembers_Profiles_ArtistProfileId",
                        column: x => x.ArtistProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArtistProfileGenre",
                columns: table => new
                {
                    ArtistProfilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistProfileGenre", x => new { x.ArtistProfilesId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_ArtistProfileGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistProfileGenre_Profiles_ArtistProfilesId",
                        column: x => x.ArtistProfilesId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreSceneProfile",
                columns: table => new
                {
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SceneProfilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreSceneProfile", x => new { x.GenresId, x.SceneProfilesId });
                    table.ForeignKey(
                        name: "FK_GenreSceneProfile_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreSceneProfile_Profiles_SceneProfilesId",
                        column: x => x.SceneProfilesId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaGalleryOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SceneProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GalleryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaGalleryOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaGalleryOwners_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaGalleryOwners_MediaGalleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "MediaGalleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaGalleryOwners_Profiles_ArtistProfileId",
                        column: x => x.ArtistProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaGalleryOwners_Profiles_SceneProfileId",
                        column: x => x.SceneProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfileAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfileAccesses_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReplyToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatMessages_ReplyToId",
                        column: x => x.ReplyToId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_MessageContents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "MessageContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastReadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatParticipants_ChatMessages_LastReadId",
                        column: x => x.LastReadId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatParticipants_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMembers_ArtistProfileId",
                table: "ArtistMembers",
                column: "ArtistProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProfileGenre_GenresId",
                table: "ArtistProfileGenre",
                column: "GenresId");

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
                name: "IX_ChatMessages_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ContentId",
                table: "ChatMessages",
                column: "ContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReplyToId",
                table: "ChatMessages",
                column: "ReplyToId",
                unique: true,
                filter: "[ReplyToId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_ChatId",
                table: "ChatParticipants",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_LastReadId",
                table: "ChatParticipants",
                column: "LastReadId",
                unique: true,
                filter: "[LastReadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_UserId",
                table: "ChatParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreSceneProfile_SceneProfilesId",
                table: "GenreSceneProfile",
                column: "SceneProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGalleryOwners_ArtistProfileId",
                table: "MediaGalleryOwners",
                column: "ArtistProfileId",
                unique: true,
                filter: "[ArtistProfileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGalleryOwners_ChatId",
                table: "MediaGalleryOwners",
                column: "ChatId",
                unique: true,
                filter: "[ChatId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGalleryOwners_GalleryId",
                table: "MediaGalleryOwners",
                column: "GalleryId",
                unique: true,
                filter: "[GalleryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGalleryOwners_SceneProfileId",
                table: "MediaGalleryOwners",
                column: "SceneProfileId",
                unique: true,
                filter: "[SceneProfileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_GalleryId",
                table: "MediaItems",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_LinkId",
                table: "MediaItems",
                column: "LinkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaLinks_UploaderId",
                table: "MediaLinks",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageContents_MediaLinkId",
                table: "MessageContents",
                column: "MediaLinkId",
                unique: true,
                filter: "[MediaLinkId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAccesses_ProfileId",
                table: "ProfileAccesses",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAccesses_UserId",
                table: "ProfileAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_OwnerId",
                table: "Profiles",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatParticipants_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "ChatParticipants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipants_AspNetUsers_UserId",
                table: "ChatParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaLinks_AspNetUsers_UploaderId",
                table: "MediaLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatParticipants_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ArtistMembers");

            migrationBuilder.DropTable(
                name: "ArtistProfileGenre");

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
                name: "GenreSceneProfile");

            migrationBuilder.DropTable(
                name: "MediaGalleryOwners");

            migrationBuilder.DropTable(
                name: "MediaItems");

            migrationBuilder.DropTable(
                name: "ProfileAccesses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MediaGalleries");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ChatParticipants");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "MessageContents");

            migrationBuilder.DropTable(
                name: "MediaLinks");
        }
    }
}
