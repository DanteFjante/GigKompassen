using KulturKanalen.Models;
using KulturKanalen.Models.Accounts;
using KulturKanalen.Models.Artists;
using KulturKanalen.Models.Chat;
using KulturKanalen.Models.Media;
using KulturKanalen.Models.Scenes;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KulturKanalen.Web.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
  {
    // DbSets for your entities
    public DbSet<ManagementProfile> ManagementProfiles { get; set; }
    public DbSet<ArtistProfile> ArtistProfiles { get; set; }
    public DbSet<SceneProfile> SceneProfiles { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MediaGallery> MediaGalleries { get; set; }
    public DbSet<MediaItem> MediaItems { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatMessageContent> ChatMessageContents { get; set; }

    // Constructor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Rename Identity tables if needed
      modelBuilder.Entity<ApplicationUser>(entity => entity.ToTable("AspNetUsers"));
      modelBuilder.Entity<ApplicationRole>(entity => entity.ToTable("AspNetRoles"));
      modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("AspNetUserRoles"));
      modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("AspNetUserClaims"));
      modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("AspNetUserLogins"));
      modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("AspNetRoleClaims"));
      modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("AspNetUserTokens"));

      // Configure IDs to be generated on add
      modelBuilder.Entity<ApplicationUser>()
          .Property(u => u.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<ManagementProfile>()
          .Property(mp => mp.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<ArtistProfile>()
          .Property(ap => ap.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<GroupMember>()
          .Property(gm => gm.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<Genre>()
          .Property(g => g.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<SceneProfile>()
          .Property(sp => sp.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<MediaGallery>()
          .Property(mg => mg.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<MediaItem>()
          .Property(mi => mi.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<Chat>()
          .Property(c => c.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<ChatParticipant>()
          .Property(cp => cp.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<ChatMessage>()
          .Property(cm => cm.Id)
          .ValueGeneratedOnAdd();

      modelBuilder.Entity<ChatMessageContent>()
          .Property(cmc => cmc.Id)
          .ValueGeneratedOnAdd();

      // Configure many-to-many relationship between ArtistProfile and Genre without explicit join entity
      modelBuilder.Entity<ArtistProfile>()
          .HasMany(ap => ap.Genres)
          .WithMany()
          .UsingEntity<Dictionary<string, object>>(
              "ArtistProfileGenre",
              j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
              j => j.HasOne<ArtistProfile>().WithMany().HasForeignKey("ArtistProfileId"),
              j =>
              {
                j.HasKey("ArtistProfileId", "GenreId");
                j.Property<Guid>("ArtistProfileId");
                j.Property<Guid>("GenreId");
              }
          );

      // Configure ChatParticipant relationships
      modelBuilder.Entity<ChatParticipant>()
          .HasOne(cp => cp.Chat)
          .WithMany(c => c.Participants)
          .HasForeignKey(cp => cp.ChatId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ChatParticipant>()
          .HasOne(cp => cp.ApplicationUser)
          .WithMany(au => au.ChatParticipants)
          .HasForeignKey(cp => cp.ApplicationUserId)
          .OnDelete(DeleteBehavior.Restrict);

      // Configure relationship for LastReadMessage
      modelBuilder.Entity<ChatParticipant>()
          .HasOne(cp => cp.LastReadMessage)
          .WithMany()
          .HasForeignKey(cp => cp.LastReadMessageId)
          .OnDelete(DeleteBehavior.Restrict);

      // Enforce uniqueness on ChatId and ApplicationUserId in ChatParticipant
      modelBuilder.Entity<ChatParticipant>()
          .HasIndex(cp => new { cp.ChatId, cp.ApplicationUserId })
          .IsUnique();

      // Configure ChatMessage relationships
      modelBuilder.Entity<ChatMessage>()
          .HasOne(cm => cm.ChatParticipant)
          .WithMany()
          .HasForeignKey(cm => cm.ChatParticipantId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ChatMessage>()
          .HasOne(cm => cm.Chat)
          .WithMany(c => c.Messages)
          .HasForeignKey(cm => cm.ChatId)
          .OnDelete(DeleteBehavior.Restrict);

      // Configure one-to-many relationship between ChatMessageContent and ChatMessage
      modelBuilder.Entity<ChatMessage>()
          .HasOne(cm => cm.ChatMessageContent)
          .WithMany(cmc => cmc.ChatMessages)
          .HasForeignKey(cm => cm.ChatMessageContentId)
          .OnDelete(DeleteBehavior.Restrict);

      // Configure ChatMessageContent audit fields relationships
      modelBuilder.Entity<ChatMessageContent>()
          .HasOne(cmc => cmc.CreatedByUser)
          .WithMany()
          .HasForeignKey(cmc => cmc.CreatedByUserId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ChatMessageContent>()
          .HasOne(cmc => cmc.ModifiedByUser)
          .WithMany()
          .HasForeignKey(cmc => cmc.ModifiedByUserId)
          .OnDelete(DeleteBehavior.Restrict);

      // Self-referencing relationship for ChatMessage (ReplyTo)
      modelBuilder.Entity<ChatMessage>()
          .HasOne(cm => cm.ReplyTo)
          .WithMany()
          .HasForeignKey(cm => cm.ReplyToId)
          .OnDelete(DeleteBehavior.Restrict);

      // One-to-many relationship between ManagementProfile and ArtistProfile
      modelBuilder.Entity<ManagementProfile>()
          .HasMany(mp => mp.ArtistProfiles)
          .WithOne(ap => ap.ManagementProfile)
          .HasForeignKey(ap => ap.ManagementProfileId);

      // One-to-many relationship between ManagementProfile and SceneProfile
      modelBuilder.Entity<ManagementProfile>()
          .HasMany(mp => mp.SceneProfiles)
          .WithOne(sp => sp.ManagementProfile)
          .HasForeignKey(sp => sp.ManagementProfileId);

      // Configurations for MediaGallery relationships (adjusted to one-to-one)
      modelBuilder.Entity<ArtistProfile>()
          .HasOne(ap => ap.MediaGallery)
          .WithOne()
          .HasForeignKey<ArtistProfile>(ap => ap.MediaGalleryId)
          .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<SceneProfile>()
          .HasOne(sp => sp.MediaGallery)
          .WithOne()
          .HasForeignKey<SceneProfile>(sp => sp.MediaGalleryId)
          .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<Chat>()
          .HasOne(c => c.MediaGallery)
          .WithOne()
          .HasForeignKey<Chat>(c => c.MediaGalleryId)
          .OnDelete(DeleteBehavior.SetNull);

      // Configurations for GroupMember
      modelBuilder.Entity<GroupMember>()
          .HasOne(gm => gm.ArtistProfile)
          .WithMany(ap => ap.GroupMembers)
          .HasForeignKey(gm => gm.ArtistProfileId);
    }
  }
}
