﻿// <auto-generated />
using System;
using GigKompassen.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GigKompassen.Blazor.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241112093607_mssql.local_migration_495")]
    partial class mssqllocal_migration_495
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArtistProfileGenre", b =>
                {
                    b.Property<Guid>("ArtistProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenresId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ArtistProfileId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("ArtistProfileGenre");
                });

            modelBuilder.Entity("GenreSceneProfile", b =>
                {
                    b.Property<Guid>("GenresId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SceneProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GenresId", "SceneProfileId");

                    b.HasIndex("SceneProfileId");

                    b.ToTable("GenreSceneProfile");
                });

            modelBuilder.Entity("GigKompassen.Models.Accounts.ApplicationRole", b =>
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

            modelBuilder.Entity("GigKompassen.Models.Accounts.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
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

                    b.Property<DateTime?>("LastLogin")
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

                    b.Property<bool>("ProfileCompleted")
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

            modelBuilder.Entity("GigKompassen.Models.Accounts.ProfileAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessType")
                        .HasColumnType("int");

                    b.Property<Guid>("ProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.HasIndex("UserId");

                    b.ToTable("ProfileAccesses");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MediaGalleryOwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MediaGalleryOwnerId")
                        .IsUnique();

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ContentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ReplyToId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Sent")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ContentId")
                        .IsUnique()
                        .HasFilter("[ContentId] IS NOT NULL");

                    b.HasIndex("ReplyToId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.ChatParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastReadTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatParticipants");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.MessageContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<int>("DataType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MessageContents");

                    b.HasDiscriminator<int>("DataType");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaGallery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("MediaGalleries");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaGalleryOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("MediaGalleryOwners");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GalleryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MediaLinkId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GalleryId");

                    b.HasIndex("MediaLinkId")
                        .IsUnique()
                        .HasFilter("[MediaLinkId] IS NOT NULL");

                    b.ToTable("MediaItems");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MediaType")
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Uploaded")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UploaderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UploaderId");

                    b.ToTable("MediaLinks");
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.ArtistMember", b =>
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

                    b.ToTable("ArtistMembers");
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.BaseProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MediaGalleryOwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ProfileType")
                        .HasColumnType("int");

                    b.Property<bool>("Public")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("MediaGalleryOwnerId")
                        .IsUnique();

                    b.HasIndex("OwnerId");

                    b.ToTable("Profiles");

                    b.HasDiscriminator<int>("ProfileType");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.Genre", b =>
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

            modelBuilder.Entity("GigKompassen.Models.Chats.MessageMediaContent", b =>
                {
                    b.HasBaseType("GigKompassen.Models.Chats.MessageContent");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MediaLinkId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("MediaLinkId")
                        .IsUnique()
                        .HasFilter("[MediaLinkId] IS NOT NULL");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.MessageTextContent", b =>
                {
                    b.HasBaseType("GigKompassen.Models.Chats.MessageContent");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.ArtistProfile", b =>
                {
                    b.HasBaseType("GigKompassen.Models.Profiles.BaseProfile");

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

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.ManagerProfile", b =>
                {
                    b.HasBaseType("GigKompassen.Models.Profiles.BaseProfile");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Profiles", t =>
                        {
                            t.Property("Description")
                                .HasColumnName("ManagerProfile_Description");

                            t.Property("Location")
                                .HasColumnName("ManagerProfile_Location");
                        });

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.SceneProfile", b =>
                {
                    b.HasBaseType("GigKompassen.Models.Profiles.BaseProfile");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Amenities")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bio")
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

                    b.Property<string>("OpeningHours")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VenueType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Profiles", t =>
                        {
                            t.Property("Bio")
                                .HasColumnName("SceneProfile_Bio");

                            t.Property("Description")
                                .HasColumnName("SceneProfile_Description");
                        });

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("ArtistProfileGenre", b =>
                {
                    b.HasOne("GigKompassen.Models.Profiles.ArtistProfile", null)
                        .WithMany()
                        .HasForeignKey("ArtistProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Profiles.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenreSceneProfile", b =>
                {
                    b.HasOne("GigKompassen.Models.Profiles.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Profiles.SceneProfile", null)
                        .WithMany()
                        .HasForeignKey("SceneProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GigKompassen.Models.Accounts.ProfileAccess", b =>
                {
                    b.HasOne("GigKompassen.Models.Profiles.BaseProfile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Profile");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.Chat", b =>
                {
                    b.HasOne("GigKompassen.Models.Media.MediaGalleryOwner", "MediaGalleryOwner")
                        .WithOne()
                        .HasForeignKey("GigKompassen.Models.Chats.Chat", "MediaGalleryOwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("MediaGalleryOwner");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.ChatMessage", b =>
                {
                    b.HasOne("GigKompassen.Models.Chats.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Chats.MessageContent", "Content")
                        .WithOne()
                        .HasForeignKey("GigKompassen.Models.Chats.ChatMessage", "ContentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("GigKompassen.Models.Chats.ChatMessage", "ReplyTo")
                        .WithMany()
                        .HasForeignKey("ReplyToId");

                    b.HasOne("GigKompassen.Models.Chats.ChatParticipant", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Content");

                    b.Navigation("ReplyTo");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.ChatParticipant", b =>
                {
                    b.HasOne("GigKompassen.Models.Chats.Chat", "Chat")
                        .WithMany("Participants")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaGallery", b =>
                {
                    b.HasOne("GigKompassen.Models.Media.MediaGalleryOwner", "Owner")
                        .WithMany("Galleries")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaItem", b =>
                {
                    b.HasOne("GigKompassen.Models.Media.MediaGallery", "Gallery")
                        .WithMany("Items")
                        .HasForeignKey("GalleryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Media.MediaLink", "MediaLink")
                        .WithOne()
                        .HasForeignKey("GigKompassen.Models.Media.MediaItem", "MediaLinkId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Gallery");

                    b.Navigation("MediaLink");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaLink", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.ArtistMember", b =>
                {
                    b.HasOne("GigKompassen.Models.Profiles.ArtistProfile", "ArtistProfile")
                        .WithMany("Members")
                        .HasForeignKey("ArtistProfileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ArtistProfile");
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.BaseProfile", b =>
                {
                    b.HasOne("GigKompassen.Models.Media.MediaGalleryOwner", "MediaGalleryOwner")
                        .WithOne()
                        .HasForeignKey("GigKompassen.Models.Profiles.BaseProfile", "MediaGalleryOwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("MediaGalleryOwner");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("GigKompassen.Models.Accounts.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.MessageMediaContent", b =>
                {
                    b.HasOne("GigKompassen.Models.Media.MediaLink", "MediaLink")
                        .WithOne()
                        .HasForeignKey("GigKompassen.Models.Chats.MessageMediaContent", "MediaLinkId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("MediaLink");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.Chat", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("GigKompassen.Models.Chats.ChatParticipant", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaGallery", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("GigKompassen.Models.Media.MediaGalleryOwner", b =>
                {
                    b.Navigation("Galleries");
                });

            modelBuilder.Entity("GigKompassen.Models.Profiles.ArtistProfile", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
