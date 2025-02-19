﻿using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
  {
    public DbSet<ProfileAccess> ProfileAccesses { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<MessageContent> MessageContents { get; set; }
    public DbSet<MessageTextContent> MessageTextContents { get; set; }
    public DbSet<MessageMediaContent> MessageMediaContents { get; set; }
    public DbSet<MediaGalleryOwner> MediaGalleryOwners { get; set; }
    public DbSet<MediaGallery> MediaGalleries { get; set; }
    public DbSet<MediaItem> MediaItems { get; set; }
    public DbSet<MediaLink> MediaLinks { get; set; }
    public DbSet<BaseProfile> Profiles { get; set; }
    public DbSet<ArtistProfile> ArtistProfiles { get; set; }
    public DbSet<SceneProfile> SceneProfiles { get; set; }
    public DbSet<ManagerProfile> ManagerProfiles { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<ArtistMember> ArtistMembers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configure Profile and Inheritance
      modelBuilder.Entity<BaseProfile>(entity =>
      {
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Name).IsRequired();
        entity.Property(p => p.Public).IsRequired();
        entity.Property(p => p.ProfileType).IsRequired();

        entity.HasOne(p => p.Owner)
              .WithMany()
              .HasForeignKey(p => p.OwnerId)
              .OnDelete(DeleteBehavior.Cascade); // When ApplicationUser is deleted, delete Profile

        entity.HasOne(p => p.MediaGalleryOwner)
              .WithOne()
              .HasForeignKey<BaseProfile>(p => p.MediaGalleryOwnerId)
              .OnDelete(DeleteBehavior.Cascade); // When Profile is deleted, delete MediaGalleryOwner
      });

      modelBuilder.Entity<ArtistProfile>().HasBaseType<BaseProfile>();
      modelBuilder.Entity<SceneProfile>().HasBaseType<BaseProfile>();
      modelBuilder.Entity<ManagerProfile>().HasBaseType<BaseProfile>();

      // Configure Profile inheritance discriminator
      modelBuilder.Entity<BaseProfile>()
          .HasDiscriminator<ProfileTypes>("ProfileType")
          .HasValue<ArtistProfile>(ProfileTypes.Artist)
          .HasValue<SceneProfile>(ProfileTypes.Scene)
          .HasValue<ManagerProfile>(ProfileTypes.Manager);

      // Configure ProfileAccess
      modelBuilder.Entity<ProfileAccess>(entity =>
      {
        entity.HasKey(pa => pa.Id);

        entity.HasOne(pa => pa.User)
              .WithMany()
              .HasForeignKey(pa => pa.UserId)
              .OnDelete(DeleteBehavior.Cascade); // When ApplicationUser is deleted, delete ProfileAccess

        entity.HasOne(pa => pa.Profile)
              .WithMany()
              .HasForeignKey(pa => pa.ProfileId)
              .OnDelete(DeleteBehavior.ClientCascade); // When Profile is deleted, delete ProfileAccess
      });

      //Cofigure Chat
      modelBuilder.Entity<Chat>(entity =>
      {
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Name).IsRequired();
        entity.HasOne(c => c.MediaGalleryOwner)
              .WithOne()
              .HasForeignKey<Chat>(c => c.MediaGalleryOwnerId)
              .OnDelete(DeleteBehavior.Cascade); // When Chat is deleted, delete MediaGalleryOwner
      });

      // Configure ChatParticipant
      modelBuilder.Entity<ChatParticipant>(entity =>
      {
        entity.HasKey(cp => cp.Id);

        entity.HasOne(cp => cp.User)
              .WithMany()
              .HasForeignKey(cp => cp.UserId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when ApplicationUser is deleted

        entity.HasOne(cp => cp.Chat)
              .WithMany(c => c.Participants)
              .HasForeignKey(cp => cp.ChatId)
              .OnDelete(DeleteBehavior.Cascade); // When Chat is deleted, delete ChatParticipant

        // Messages relationship is configured in ChatMessage
      });

      // Configure ChatMessage
      modelBuilder.Entity<ChatMessage>(entity =>
      {
        entity.HasKey(cm => cm.Id);

        entity.HasOne(cm => cm.Chat)
              .WithMany(c => c.Messages)
              .HasForeignKey(cm => cm.ChatId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when Chat is deleted

        entity.HasOne(cm => cm.Sender)
              .WithMany(cp => cp.Messages)
              .HasForeignKey(cm => cm.SenderId)
              .OnDelete(DeleteBehavior.ClientNoAction); // Ignore deletion of Sender

        entity.HasOne(cm => cm.Content)
              .WithOne()
              .HasForeignKey<ChatMessage>(mc => mc.ContentId)
              .OnDelete(DeleteBehavior.SetNull); // Cascade when ChatMessage is deleted

        entity.HasOne(cm => cm.ReplyTo)
              .WithMany()
              .HasForeignKey(cm => cm.ReplyToId)
              .OnDelete(DeleteBehavior.ClientNoAction); // Ignore deletion of ReplyTo
      });

      // Configure MessageContent and Inheritance
      modelBuilder.Entity<MessageContent>(entity =>
      {
        entity.HasKey(mc => mc.Id);
        entity.Property(mc => mc.Created).IsRequired();
        entity.Property(mc => mc.DataType).IsRequired();

        // Inheritance
        entity.HasDiscriminator<ChatMessageType>("DataType")
              .HasValue<MessageTextContent>(ChatMessageType.Text)
              .HasValue<MessageMediaContent>(ChatMessageType.Media);
      });

      // Configure MessageMediaContent
      modelBuilder.Entity<MessageMediaContent>(entity =>
      {
        entity.HasOne(mmc => mmc.MediaLink)
              .WithOne()
              .HasForeignKey<MessageMediaContent>(mmc => mmc.MediaLinkId)
              .OnDelete(DeleteBehavior.SetNull); // Cascade when MediaLink is deleted
      });

      // Configure MediaGalleryOwner
      modelBuilder.Entity<MediaGalleryOwner>(entity =>
      {
        entity.HasKey(mgo => mgo.Id);

        entity.HasMany(mgo => mgo.Galleries)
              .WithOne(g => g.Owner)
              .HasForeignKey(g => g.OwnerId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when MediaGalleryOwner is deleted
      });

      // Configure MediaGallery
      modelBuilder.Entity<MediaGallery>(entity =>
      {
        entity.HasKey(g => g.Id);

        entity.HasMany(g => g.Items)
              .WithOne(mi => mi.Gallery)
              .HasForeignKey(mi => mi.GalleryId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when MediaGallery is deleted
      });

      // Configure MediaItem
      modelBuilder.Entity<MediaItem>(entity =>
      {
        entity.HasKey(mi => mi.Id);

        entity.HasOne(mi => mi.MediaLink)
              .WithOne()
              .HasForeignKey<MediaItem>(mi => mi.MediaLinkId)
              .OnDelete(DeleteBehavior.SetNull); // Cascade when MediaLink is deleted
      });

      // Configure MediaLink
      modelBuilder.Entity<MediaLink>(entity =>
      {
        entity.HasKey(ml => ml.Id);

        entity.HasOne(ml => ml.Uploader)
              .WithMany()
              .HasForeignKey(ml => ml.UploaderId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when ApplicationUser is deleted
      });

      // Configure ArtistProfile
      modelBuilder.Entity<ArtistProfile>(entity =>
      {
        entity.HasMany(ap => ap.Members)
              .WithOne(am => am.ArtistProfile)
              .HasForeignKey(am => am.ArtistProfileId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade when ArtistProfile is deleted

        entity.HasMany(ap => ap.Genres)
              .WithMany(); // Ignore deletion
      });

      // Configure SceneProfile
      modelBuilder.Entity<SceneProfile>(entity =>
      {
        entity.HasMany(sp => sp.Genres)
              .WithMany(); // Ignore deletion
      });


    }
  }
}
