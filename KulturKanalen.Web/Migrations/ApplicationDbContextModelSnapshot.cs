﻿// <auto-generated />
using System;
using KulturKanalen.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KulturKanalen.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArtistProfileGenre", b =>
                {
                    b.Property<Guid>("ArtistProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenreId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ArtistProfileId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("ArtistProfileGenre");
                });

            modelBuilder.Entity("KulturKanalen.Models.Accounts.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

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

            modelBuilder.Entity("KulturKanalen.Models.Accounts.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int>("ApplicationRole")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastLoggedIn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("KulturKanalen.Models.Artists.ArtistProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Availability")
                        .HasColumnType("int");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ManagementProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MediaGalleryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ManagementProfileId");

                    b.HasIndex("MediaGalleryId")
                        .IsUnique()
                        .HasFilter("[MediaGalleryId] IS NOT NULL");

                    b.ToTable("ArtistProfiles");
                });

            modelBuilder.Entity("KulturKanalen.Models.Artists.GroupMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ArtistProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArtistProfileId");

                    b.ToTable("GroupMembers");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChatName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("MediaGalleryId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("MediaGalleryId")
                        .IsUnique()
                        .HasFilter("[MediaGalleryId] IS NOT NULL");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatMessageContentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeSent")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ReplyToId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ChatMessageContentId");

                    b.HasIndex("ChatParticipantId");

                    b.HasIndex("ReplyToId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatMessageContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChatMessageType")
                        .HasColumnType("int");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("FlaggedForDeletion")
                        .HasColumnType("bit");

                    b.Property<Guid?>("MediaItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ModifiedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("MediaItemId");

                    b.HasIndex("ModifiedByUserId");

                    b.ToTable("ChatMessageContents");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LastReadMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("LastReadMessageId");

                    b.HasIndex("ChatId", "ApplicationUserId")
                        .IsUnique();

                    b.ToTable("ChatParticipants");
                });

            modelBuilder.Entity("KulturKanalen.Models.Genre", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("KulturKanalen.Models.ManagementProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("ManagementProfiles");
                });

            modelBuilder.Entity("KulturKanalen.Models.Media.MediaGallery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("MediaGalleries");
                });

            modelBuilder.Entity("KulturKanalen.Models.Media.MediaItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MediaGalleryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MediaType")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("MediaGalleryId");

                    b.ToTable("MediaItems");
                });

            modelBuilder.Entity("KulturKanalen.Models.Scenes.SceneProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Amenities")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("ContactInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ManagementProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MediaGalleryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OpeningHours")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VenueType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ManagementProfileId");

                    b.HasIndex("MediaGalleryId")
                        .IsUnique()
                        .HasFilter("[MediaGalleryId] IS NOT NULL");

                    b.ToTable("SceneProfiles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("ArtistProfileGenre", b =>
                {
                    b.HasOne("KulturKanalen.Models.Artists.ArtistProfile", null)
                        .WithMany()
                        .HasForeignKey("ArtistProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KulturKanalen.Models.Artists.ArtistProfile", b =>
                {
                    b.HasOne("KulturKanalen.Models.ManagementProfile", "ManagementProfile")
                        .WithMany("ArtistProfiles")
                        .HasForeignKey("ManagementProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Media.MediaGallery", "MediaGallery")
                        .WithOne()
                        .HasForeignKey("KulturKanalen.Models.Artists.ArtistProfile", "MediaGalleryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ManagementProfile");

                    b.Navigation("MediaGallery");
                });

            modelBuilder.Entity("KulturKanalen.Models.Artists.GroupMember", b =>
                {
                    b.HasOne("KulturKanalen.Models.Artists.ArtistProfile", "ArtistProfile")
                        .WithMany("GroupMembers")
                        .HasForeignKey("ArtistProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ArtistProfile");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.Chat", b =>
                {
                    b.HasOne("KulturKanalen.Models.Media.MediaGallery", "MediaGallery")
                        .WithOne()
                        .HasForeignKey("KulturKanalen.Models.Chat.Chat", "MediaGalleryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("MediaGallery");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatMessage", b =>
                {
                    b.HasOne("KulturKanalen.Models.Chat.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Chat.ChatMessageContent", "ChatMessageContent")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChatMessageContentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Chat.ChatParticipant", "ChatParticipant")
                        .WithMany()
                        .HasForeignKey("ChatParticipantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Chat.ChatMessage", "ReplyTo")
                        .WithMany()
                        .HasForeignKey("ReplyToId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Chat");

                    b.Navigation("ChatMessageContent");

                    b.Navigation("ChatParticipant");

                    b.Navigation("ReplyTo");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatMessageContent", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Media.MediaItem", "MediaItem")
                        .WithMany()
                        .HasForeignKey("MediaItemId");

                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("CreatedByUser");

                    b.Navigation("MediaItem");

                    b.Navigation("ModifiedByUser");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatParticipant", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", "ApplicationUser")
                        .WithMany("ChatParticipants")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Chat.Chat", "Chat")
                        .WithMany("Participants")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Chat.ChatMessage", "LastReadMessage")
                        .WithMany()
                        .HasForeignKey("LastReadMessageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ApplicationUser");

                    b.Navigation("Chat");

                    b.Navigation("LastReadMessage");
                });

            modelBuilder.Entity("KulturKanalen.Models.ManagementProfile", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("KulturKanalen.Models.Media.MediaItem", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("KulturKanalen.Models.Media.MediaGallery", "MediaGallery")
                        .WithMany("MediaItems")
                        .HasForeignKey("MediaGalleryId");

                    b.Navigation("ApplicationUser");

                    b.Navigation("MediaGallery");
                });

            modelBuilder.Entity("KulturKanalen.Models.Scenes.SceneProfile", b =>
                {
                    b.HasOne("KulturKanalen.Models.ManagementProfile", "ManagementProfile")
                        .WithMany("SceneProfiles")
                        .HasForeignKey("ManagementProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Media.MediaGallery", "MediaGallery")
                        .WithOne()
                        .HasForeignKey("KulturKanalen.Models.Scenes.SceneProfile", "MediaGalleryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ManagementProfile");

                    b.Navigation("MediaGallery");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("KulturKanalen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KulturKanalen.Models.Accounts.ApplicationUser", b =>
                {
                    b.Navigation("ChatParticipants");
                });

            modelBuilder.Entity("KulturKanalen.Models.Artists.ArtistProfile", b =>
                {
                    b.Navigation("GroupMembers");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.Chat", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("KulturKanalen.Models.Chat.ChatMessageContent", b =>
                {
                    b.Navigation("ChatMessages");
                });

            modelBuilder.Entity("KulturKanalen.Models.ManagementProfile", b =>
                {
                    b.Navigation("ArtistProfiles");

                    b.Navigation("SceneProfiles");
                });

            modelBuilder.Entity("KulturKanalen.Models.Media.MediaGallery", b =>
                {
                    b.Navigation("MediaItems");
                });
#pragma warning restore 612, 618
        }
    }
}
