using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "882f08a3-81dd-44dc-91cb-a7c67147494a" },
                new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "ebb9274b-af35-45a2-abf9-8075a4bfc997" },
                new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "797905bc-96fa-4d21-939a-edfb598dd95c" }
            );

        modelBuilder.Entity<MemberLike>()
            .HasKey(m => new { m.SourceMemberId, m.TargetMemberId });

        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.SourceMember)
            .WithMany(t => t.LikedMembers)
            .HasForeignKey(s => s.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.TargetMember)
            .WithMany(t => t.LikedByMembers)
            .HasForeignKey(s => s.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(mr => mr.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(mr => mr.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .Property(m => m.RecipientDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Message>()
            .Property(m => m.SenderDeleted)
            .HasDefaultValue(false);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
