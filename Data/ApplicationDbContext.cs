using IELTSWritingAI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IELTSWritingAI.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<WritingTopic> WritingTopics => Set<WritingTopic>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WritingTopic>()
            .Property(x => x.TaskType)
            .HasConversion<string>();

        builder.Entity<WritingTopic>()
            .Property(x => x.TopicText)
            .HasMaxLength(2000);

        builder.Entity<Submission>()
            .Property(x => x.EssayText)
            .HasMaxLength(12000);

        builder.Entity<Submission>()
            .Property(x => x.Feedback)
            .HasMaxLength(8000);

        builder.Entity<Submission>()
            .Property(x => x.OverallScore)
            .HasPrecision(3, 1);

        builder.Entity<Submission>()
            .Property(x => x.TaskAchievement)
            .HasPrecision(3, 1);

        builder.Entity<Submission>()
            .Property(x => x.CoherenceCohesion)
            .HasPrecision(3, 1);

        builder.Entity<Submission>()
            .Property(x => x.LexicalResource)
            .HasPrecision(3, 1);

        builder.Entity<Submission>()
            .Property(x => x.GrammarRangeAccuracy)
            .HasPrecision(3, 1);

        builder.Entity<ChatMessage>()
            .Property(x => x.Message)
            .HasMaxLength(8000);
    }
}
