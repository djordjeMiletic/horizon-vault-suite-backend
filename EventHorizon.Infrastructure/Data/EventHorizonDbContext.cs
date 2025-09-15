using EventHorizon.Domain.Entities;
using EventHorizon.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Infrastructure.Data;

public class EventHorizonDbContext : DbContext
{
    public EventHorizonDbContext(DbContextOptions<EventHorizonDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentCycle> PaymentCycles { get; set; }
    public DbSet<PaymentCycleItem> PaymentCycleItems { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<GoalHistory> GoalHistory { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketMessage> TicketMessages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<SignatureRequest> SignatureRequests { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<WebsiteInquiry> WebsiteInquiries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).HasConversion<string>();
        });

        // Policy
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductRatePct).HasPrecision(5, 2);
            entity.Property(e => e.MarginPct).HasPrecision(5, 2);
            entity.Property(e => e.ThresholdMultiplier).HasPrecision(5, 2);
            entity.Property(e => e.SplitAdvisor).HasPrecision(5, 2);
            entity.Property(e => e.SplitIntroducer).HasPrecision(5, 2);
            entity.Property(e => e.SplitManager).HasPrecision(5, 2);
            entity.Property(e => e.SplitExec).HasPrecision(5, 2);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdvisorEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(200);
            entity.Property(e => e.APE).HasPrecision(18, 2);
            entity.Property(e => e.Receipts).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Payments)
                .HasForeignKey(e => e.ProductId);
        });

        // PaymentCycle
        modelBuilder.Entity<PaymentCycle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CycleKey).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        // PaymentCycleItem
        modelBuilder.Entity<PaymentCycleItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProposedStatus).HasMaxLength(50);
            entity.Property(e => e.FinalStatus).HasMaxLength(50);
            entity.Property(e => e.ManagerNote).HasMaxLength(1000);

            entity.HasOne(e => e.Payment)
                .WithMany(p => p.CycleItems)
                .HasForeignKey(e => e.PaymentId);

            entity.HasOne(e => e.Cycle)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CycleId);
        });

        // Goal
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SubjectType).HasConversion<string>();
            entity.Property(e => e.SubjectRef).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MonthlyTarget).HasPrecision(18, 2);
        });

        // GoalHistory
        modelBuilder.Entity<GoalHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Month).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Achieved).HasPrecision(18, 2);

            entity.HasOne(e => e.Goal)
                .WithMany(g => g.History)
                .HasForeignKey(e => e.GoalId);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecipientEmail).HasMaxLength(200);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });

        // Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FromEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(20);
        });

        // TicketMessage
        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Text).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(e => e.TicketId);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Actor).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DetailsJson).IsRequired();
        });

        // Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OwnerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.OriginalName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(200);
            entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Tags).HasMaxLength(500);
        });

        // SignatureRequest
        modelBuilder.Entity<SignatureRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SignerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Document)
                .WithMany(d => d.SignatureRequests)
                .HasForeignKey(e => e.DocumentId);
        });

        // JobPosting
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
        });

        // JobApplication
        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApplicantName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApplicantEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CVPath).HasMaxLength(1000);
            entity.Property(e => e.CoverLetter).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.JobPosting)
                .WithMany(jp => jp.Applications)
                .HasForeignKey(e => e.JobPostingId);
        });

        // WebsiteInquiry
        modelBuilder.Entity<WebsiteInquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Source).HasMaxLength(100);
        });
    }
}