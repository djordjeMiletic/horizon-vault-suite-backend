using EventHorizon.Domain.Entities;
using EventHorizon.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(EventHorizonDbContext context)
    {
        if (await context.Users.AnyAsync()) return; // Already seeded

        // Users
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Email = "sarah.johnson@event-horizon.test", DisplayName = "Sarah Johnson", Role = UserRole.Advisor, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new User { Id = Guid.NewGuid(), Email = "michael.carter@event-horizon.test", DisplayName = "Michael Carter", Role = UserRole.Advisor, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new User { Id = Guid.NewGuid(), Email = "manager@event-horizon.test", DisplayName = "Team Manager", Role = UserRole.Manager, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new User { Id = Guid.NewGuid(), Email = "admin@event-horizon.test", DisplayName = "System Admin", Role = UserRole.Administrator, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new User { Id = Guid.NewGuid(), Email = "client@event-horizon.test", DisplayName = "Test Client", Role = UserRole.Client, CreatedAt = DateTime.UtcNow.AddMonths(-3) }
        };
        context.Users.AddRange(users);

        // Policies
        var policies = new[]
        {
            new Policy { Id = Guid.NewGuid(), ProductCode = "PRD-TERM", ProductName = "Term Life", ProductRatePct = 7.5m, MarginPct = 10m, ThresholdMultiplier = 2.0m, SplitAdvisor = 50m, SplitIntroducer = 10m, SplitManager = 30m, SplitExec = 10m },
            new Policy { Id = Guid.NewGuid(), ProductCode = "PRD-CI", ProductName = "Critical Illness", ProductRatePct = 8.0m, MarginPct = 12m, ThresholdMultiplier = 2.0m, SplitAdvisor = 50m, SplitIntroducer = 10m, SplitManager = 30m, SplitExec = 10m },
            new Policy { Id = Guid.NewGuid(), ProductCode = "PRD-WOL", ProductName = "Whole of Life", ProductRatePct = 6.5m, MarginPct = 8m, ThresholdMultiplier = 2.0m, SplitAdvisor = 55m, SplitIntroducer = 10m, SplitManager = 25m, SplitExec = 10m },
            new Policy { Id = Guid.NewGuid(), ProductCode = "PRD-IP", ProductName = "Income Protection", ProductRatePct = 7.0m, MarginPct = 10m, ThresholdMultiplier = 2.0m, SplitAdvisor = 50m, SplitIntroducer = 10m, SplitManager = 30m, SplitExec = 10m }
        };
        context.Policies.AddRange(policies);

        await context.SaveChangesAsync();

        // Payments
        var random = new Random();
        var providers = new[] { "Royal London", "MetLife", "Guardian", "Aviva", "Aegon" };
        var statuses = new[] { "Pending", "Approved", "Rejected" };
        var advisorEmails = new[] { "sarah.johnson@event-horizon.test", "michael.carter@event-horizon.test" };

        var payments = new List<Payment>();
        for (int i = 0; i < 50; i++)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(-random.Next(1, 540))); // 18 months back
            var policy = policies[random.Next(policies.Length)];
            var ape = random.Next(1000, 50000);
            var receipts = ape + random.Next(-500, 2000);

            payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                Date = date,
                AdvisorEmail = advisorEmails[random.Next(advisorEmails.Length)],
                ProductId = policy.Id,
                Provider = providers[random.Next(providers.Length)],
                APE = ape,
                Receipts = Math.Max(0, receipts),
                Status = statuses[random.Next(statuses.Length)],
                Notes = i % 5 == 0 ? "Special case requiring review" : null
            });
        }
        context.Payments.AddRange(payments);

        // Goals
        var goals = new[]
        {
            new Goal { Id = Guid.NewGuid(), SubjectType = SubjectType.Advisor, SubjectRef = "sarah.johnson@event-horizon.test", MonthlyTarget = 25000m },
            new Goal { Id = Guid.NewGuid(), SubjectType = SubjectType.Advisor, SubjectRef = "michael.carter@event-horizon.test", MonthlyTarget = 30000m },
            new Goal { Id = Guid.NewGuid(), SubjectType = SubjectType.Manager, SubjectRef = "team", MonthlyTarget = 55000m }
        };
        context.Goals.AddRange(goals);

        // Goal History
        var goalHistory = new List<GoalHistory>();
        foreach (var goal in goals)
        {
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i).ToString("yyyy-MM");
                var achieved = goal.MonthlyTarget * (0.7m + (decimal)random.NextDouble() * 0.6m); // 70-130% of target
                
                goalHistory.Add(new GoalHistory
                {
                    Id = Guid.NewGuid(),
                    GoalId = goal.Id,
                    Month = month,
                    Achieved = Math.Round(achieved, 2)
                });
            }
        }
        context.GoalHistory.AddRange(goalHistory);

        // Notifications
        var notifications = new[]
        {
            new Notification { Id = Guid.NewGuid(), RecipientEmail = null, Title = "Payment cycle 2024-12 closed", Type = "System", CreatedAt = DateTime.UtcNow.AddDays(-2), Read = false },
            new Notification { Id = Guid.NewGuid(), RecipientEmail = null, Title = "New advisor user registered", Type = "User", CreatedAt = DateTime.UtcNow.AddDays(-5), Read = false },
            new Notification { Id = Guid.NewGuid(), RecipientEmail = "client@event-horizon.test", Title = "Your case has been updated", Type = "Case", CreatedAt = DateTime.UtcNow.AddDays(-1), Read = false },
            new Notification { Id = Guid.NewGuid(), RecipientEmail = "client@event-horizon.test", Title = "Document ready for signature", Type = "Document", CreatedAt = DateTime.UtcNow.AddDays(-3), Read = false }
        };
        context.Notifications.AddRange(notifications);

        // Tickets
        var tickets = new[]
        {
            new Ticket { Id = Guid.NewGuid(), Subject = "Policy clarification needed", FromEmail = "client@event-horizon.test", Status = "Open", Priority = "Medium", UpdatedAt = DateTime.UtcNow.AddDays(-2) },
            new Ticket { Id = Guid.NewGuid(), Subject = "Commission calculation query", FromEmail = "sarah.johnson@event-horizon.test", Status = "Pending", Priority = "High", UpdatedAt = DateTime.UtcNow.AddDays(-1) },
            new Ticket { Id = Guid.NewGuid(), Subject = "System access issue", FromEmail = "client@event-horizon.test", Status = "Resolved", Priority = "Low", UpdatedAt = DateTime.UtcNow.AddDays(-5) }
        };
        context.Tickets.AddRange(tickets);

        await context.SaveChangesAsync();

        // Ticket Messages
        var ticketMessages = new List<TicketMessage>();
        foreach (var ticket in tickets)
        {
            ticketMessages.Add(new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                At = ticket.UpdatedAt.AddMinutes(-30),
                FromEmail = ticket.FromEmail,
                Text = "Initial message for this ticket. Please help me with this issue."
            });

            if (ticket.Status != "Open")
            {
                ticketMessages.Add(new TicketMessage
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    At = ticket.UpdatedAt,
                    FromEmail = "admin@event-horizon.test",
                    Text = "Thank you for your inquiry. We're looking into this matter."
                });
            }
        }
        context.TicketMessages.AddRange(ticketMessages);

        // Documents
        var documentsPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "documents");
        Directory.CreateDirectory(documentsPath);

        var documents = new[]
        {
            new Document { Id = Guid.NewGuid(), OwnerEmail = "client@event-horizon.test", FileName = "sample1.pdf", OriginalName = "Policy Document.pdf", ContentType = "application/pdf", SizeBytes = 1024, StoragePath = "storage/documents/sample1.pdf", CreatedAt = DateTime.UtcNow.AddDays(-10), Tags = "policy,important" },
            new Document { Id = Guid.NewGuid(), OwnerEmail = "sarah.johnson@event-horizon.test", FileName = "sample2.pdf", OriginalName = "Client Agreement.pdf", ContentType = "application/pdf", SizeBytes = 2048, StoragePath = "storage/documents/sample2.pdf", CreatedAt = DateTime.UtcNow.AddDays(-5), Tags = "agreement" }
        };
        context.Documents.AddRange(documents);

        // Create sample files
        foreach (var doc in documents)
        {
            var filePath = Path.Combine(documentsPath, doc.FileName);
            await File.WriteAllTextAsync(filePath, $"Sample document content for {doc.OriginalName}");
        }

        // Signature Requests
        var signatureRequests = new[]
        {
            new SignatureRequest { Id = Guid.NewGuid(), DocumentId = documents[0].Id, SignerEmail = "client@event-horizon.test", Status = "Pending", CreatedAt = DateTime.UtcNow.AddDays(-3), Token = Guid.NewGuid().ToString("N") },
            new SignatureRequest { Id = Guid.NewGuid(), DocumentId = documents[1].Id, SignerEmail = "client@event-horizon.test", Status = "Signed", CreatedAt = DateTime.UtcNow.AddDays(-7), CompletedAt = DateTime.UtcNow.AddDays(-6), Token = Guid.NewGuid().ToString("N") }
        };
        context.SignatureRequests.AddRange(signatureRequests);

        // Job Postings
        var jobPostings = new[]
        {
            new JobPosting { Id = Guid.NewGuid(), Title = "Senior Financial Advisor", Department = "Sales", Location = "London", Description = "We are seeking an experienced financial advisor to join our growing team.", IsOpen = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
            new JobPosting { Id = Guid.NewGuid(), Title = "Insurance Specialist", Department = "Operations", Location = "Manchester", Description = "Looking for an insurance specialist with 3+ years experience.", IsOpen = true, CreatedAt = DateTime.UtcNow.AddDays(-7) }
        };
        context.JobPostings.AddRange(jobPostings);

        // Website Inquiries
        var inquiries = new[]
        {
            new WebsiteInquiry { Id = Guid.NewGuid(), Name = "John Smith", Email = "john.smith@example.com", Subject = "Investment advice", Message = "I'm interested in learning more about your investment services.", CreatedAt = DateTime.UtcNow.AddDays(-3), Source = "website" },
            new WebsiteInquiry { Id = Guid.NewGuid(), Name = "Jane Doe", Email = "jane.doe@example.com", Subject = "Life insurance quote", Message = "Could you provide a quote for life insurance coverage?", CreatedAt = DateTime.UtcNow.AddDays(-1), Source = "referral" }
        };
        context.WebsiteInquiries.AddRange(inquiries);

        // Audit Logs
        var auditLogs = new[]
        {
            new AuditLog { Id = Guid.NewGuid(), Ts = DateTime.UtcNow.AddDays(-2), Actor = "admin@event-horizon.test", Action = "CREATE", EntityType = "PaymentCycle", EntityId = Guid.NewGuid().ToString(), DetailsJson = "{\"cycleKey\":\"2024-12\"}" },
            new AuditLog { Id = Guid.NewGuid(), Ts = DateTime.UtcNow.AddDays(-1), Actor = "manager@event-horizon.test", Action = "UPDATE", EntityType = "Payment", EntityId = payments[0].Id.ToString(), DetailsJson = "{\"status\":\"Approved\"}" }
        };
        context.AuditLogs.AddRange(auditLogs);

        // HR Data
        var interviews = new[]
        {
            new Interview { Id = Guid.NewGuid(), JobApplicationId = Guid.NewGuid(), ScheduledAt = DateTime.UtcNow.AddDays(3), Mode = "Video", Status = "Scheduled", Notes = "Initial screening interview", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Interview { Id = Guid.NewGuid(), JobApplicationId = Guid.NewGuid(), ScheduledAt = DateTime.UtcNow.AddDays(-1), Mode = "In-Person", Status = "Completed", Notes = "Technical interview completed successfully", CreatedAt = DateTime.UtcNow.AddDays(-5) }
        };
        context.Interviews.AddRange(interviews);

        var onboardingTasks = new[]
        {
            new OnboardingTask { Id = Guid.NewGuid(), Title = "Complete IT setup", AssigneeEmail = "sarah.johnson@event-horizon.test", Status = "Completed", CreatedAt = DateTime.UtcNow.AddDays(-10), CompletedAt = DateTime.UtcNow.AddDays(-8) },
            new OnboardingTask { Id = Guid.NewGuid(), Title = "Compliance training", AssigneeEmail = "michael.carter@event-horizon.test", Status = "In Progress", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new OnboardingTask { Id = Guid.NewGuid(), Title = "Meet team members", AssigneeEmail = "sarah.johnson@event-horizon.test", Status = "Pending", CreatedAt = DateTime.UtcNow.AddDays(-3) }
        };
        context.OnboardingTasks.AddRange(onboardingTasks);

        // Client Management Data
        var appointments = new[]
        {
            new Appointment { Id = Guid.NewGuid(), ClientEmail = "client@event-horizon.test", AdvisorEmail = "sarah.johnson@event-horizon.test", StartAt = DateTime.UtcNow.AddDays(2), EndAt = DateTime.UtcNow.AddDays(2).AddHours(1), Location = "Office Meeting Room A", Status = "Scheduled", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Appointment { Id = Guid.NewGuid(), ClientEmail = "client@event-horizon.test", AdvisorEmail = "michael.carter@event-horizon.test", StartAt = DateTime.UtcNow.AddDays(-2), EndAt = DateTime.UtcNow.AddDays(-2).AddHours(1), Location = "Video Call", Status = "Completed", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Appointment { Id = Guid.NewGuid(), ClientEmail = "john.smith@example.com", AdvisorEmail = "sarah.johnson@event-horizon.test", StartAt = DateTime.UtcNow.AddDays(5), EndAt = DateTime.UtcNow.AddDays(5).AddHours(1), Location = "Client Office", Status = "Scheduled", CreatedAt = DateTime.UtcNow.AddDays(-1) }
        };
        context.Appointments.AddRange(appointments);

        // CRM Data
        var leads = new[]
        {
            new Lead { Id = Guid.NewGuid(), Name = "Emma Wilson", Email = "emma.wilson@example.com", Phone = "+44 20 7123 4567", Status = "New", OwnerEmail = "sarah.johnson@event-horizon.test", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Lead { Id = Guid.NewGuid(), Name = "James Brown", Email = "james.brown@example.com", Phone = "+44 20 7123 4568", Status = "Contacted", OwnerEmail = "michael.carter@event-horizon.test", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Lead { Id = Guid.NewGuid(), Name = "Sophie Davis", Email = "sophie.davis@example.com", Phone = "+44 20 7123 4569", Status = "Qualified", OwnerEmail = "sarah.johnson@event-horizon.test", CreatedAt = DateTime.UtcNow.AddDays(-7) }
        };
        context.Leads.AddRange(leads);

        var pipelineDeals = new[]
        {
            new PipelineDeal { Id = Guid.NewGuid(), Title = "Life Insurance - Wilson Family", AdvisorEmail = "sarah.johnson@event-horizon.test", Stage = "Proposal", Value = 25000m, CreatedAt = DateTime.UtcNow.AddDays(-10), UpdatedAt = DateTime.UtcNow.AddDays(-2) },
            new PipelineDeal { Id = Guid.NewGuid(), Title = "Critical Illness - Brown", AdvisorEmail = "michael.carter@event-horizon.test", Stage = "Negotiation", Value = 15000m, CreatedAt = DateTime.UtcNow.AddDays(-15), UpdatedAt = DateTime.UtcNow.AddDays(-3) },
            new PipelineDeal { Id = Guid.NewGuid(), Title = "Income Protection - Davis", AdvisorEmail = "sarah.johnson@event-horizon.test", Stage = "Qualified", Value = 18000m, CreatedAt = DateTime.UtcNow.AddDays(-8), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };
        context.PipelineDeals.AddRange(pipelineDeals);

        var referralPartners = new[]
        {
            new ReferralPartner { Id = Guid.NewGuid(), Name = "ABC Financial Services", Email = "contact@abcfinancial.com", Phone = "+44 20 7123 5000", Active = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new ReferralPartner { Id = Guid.NewGuid(), Name = "XYZ Insurance Brokers", Email = "info@xyzbrokers.com", Phone = "+44 20 7123 5001", Active = true, CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new ReferralPartner { Id = Guid.NewGuid(), Name = "Premier Wealth Management", Email = "hello@premierwealth.com", Active = false, CreatedAt = DateTime.UtcNow.AddDays(-60) }
        };
        context.ReferralPartners.AddRange(referralPartners);

        // Compliance Data
        var complianceDocs = new[]
        {
            new ComplianceDoc { Id = Guid.NewGuid(), OwnerEmail = "admin@event-horizon.test", Title = "FCA Compliance Review Q4 2024", Status = "Under Review", CreatedAt = DateTime.UtcNow.AddDays(-10), UpdatedAt = DateTime.UtcNow.AddDays(-2) },
            new ComplianceDoc { Id = Guid.NewGuid(), OwnerEmail = "manager@event-horizon.test", Title = "Data Protection Impact Assessment", Status = "Approved", CreatedAt = DateTime.UtcNow.AddDays(-20), UpdatedAt = DateTime.UtcNow.AddDays(-5) },
            new ComplianceDoc { Id = Guid.NewGuid(), OwnerEmail = "sarah.johnson@event-horizon.test", Title = "Client Suitability Report Template", Status = "Draft", CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };
        context.ComplianceDocs.AddRange(complianceDocs);

        await context.SaveChangesAsync();
    }
}