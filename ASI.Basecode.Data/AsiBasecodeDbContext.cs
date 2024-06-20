﻿using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDBContext : DbContext
    {
        public AsiBasecodeDBContext()
        {
        }

        public AsiBasecodeDBContext(DbContextOptions<AsiBasecodeDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketPriority> Priorities { get; set; }
        public DbSet<TicketStatus> Statuses { get; set; }
        public DbSet<TicketActivity> TicketActivities { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add values to tables

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { RoleId = 1, Name = "Admin" },
                new UserRole { RoleId = 2, Name = "User" },
                new UserRole { RoleId = 3, Name = "Agent" }
            );

            modelBuilder.Entity<TicketStatus>().HasData(
                new TicketStatus { StatusId = 1, Name = "Open" },
                new TicketStatus { StatusId = 2, Name = "Assigned" },
                new TicketStatus { StatusId = 3, Name = "In Progress" },
                new TicketStatus { StatusId = 4, Name = "Resolved" },
                new TicketStatus { StatusId = 5, Name = "Closed" }
            );

            modelBuilder.Entity<TicketPriority>().HasData(
                new TicketPriority { PriorityId = 1, Name = "High" },
                new TicketPriority { PriorityId = 2, Name = "Mid" },
                new TicketPriority { PriorityId = 3, Name = "Low" }
            );

            modelBuilder.Entity<TicketOperation>().HasData(
                new TicketOperation { OperationId = 1, Name = "Created" },
                new TicketOperation { OperationId = 2, Name = "Updated" },
                new TicketOperation { OperationId = 3, Name = "Assigned" },
                new TicketOperation { OperationId = 4, Name = "Reassigned" },
                new TicketOperation { OperationId = 5, Name = "Closed"}
            );

            modelBuilder.Entity<ArticleCategory>().HasData(
                new ArticleCategory { CategoryId = 1, Name = "Technical" },
                new ArticleCategory { CategoryId = 2, Name = "Billing" },
                new ArticleCategory { CategoryId = 3, Name = "General Questions" },
                new ArticleCategory { CategoryId = 4, Name = "Account Management" },
                new ArticleCategory { CategoryId = 5, Name = "Customer Support" }
            );


            ConfigureUserEntity(modelBuilder);
            ConfigureTicketEntity(modelBuilder);
            ConfigureOtherEntities(modelBuilder);
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            // User To Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();

            modelBuilder.Entity<UserRole>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Created Ticket
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedTickets)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // User to Assigned Ticket
            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedTickets)
                .WithOne(t => t.AssignedAgent)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedAgent)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);


            // User and TicketActivity
            modelBuilder.Entity<User>()
                .HasMany(u => u.TicketActivities)
                .WithOne(th => th.User)
                .HasForeignKey(th => th.PerformedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketActivity>()
                .HasOne(th => th.User)
                .WithMany(u => u.TicketActivities)
                .HasForeignKey(th => th.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User and Feedbacks
            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User and Articles
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedArticles)
                .WithOne(a => a.Author)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany(u => u.CreatedArticles)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // User and UserPreference
            modelBuilder.Entity<User>()
                .HasOne(u => u.Preference)
                .WithOne(up => up.User)
                .HasForeignKey<UserPreference>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithOne(u => u.Preference)
                .HasForeignKey<UserPreference>(up => up.UserId)
                .IsRequired();
       
        }

        private void ConfigureTicketEntity(ModelBuilder modelBuilder)
        {
            // Ticket and Ticket Status
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.StatusId)
                .IsRequired();

            modelBuilder.Entity<TicketStatus>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Status)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);


            // Ticket and Ticket Priority
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.PriorityId)
                .IsRequired();

            modelBuilder.Entity<TicketPriority>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Priority)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);


            // Ticket and Ticket History
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.TicketActivities)
                .WithOne(th => th.Ticket)
                .HasForeignKey(th => th.TicketId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting Ticket will cascade to delete TicketActivities

            modelBuilder.Entity<TicketActivity>()
                .HasOne(th => th.Ticket)
                .WithMany(t => t.TicketActivities)
                .HasForeignKey(th => th.TicketId)
                .OnDelete(DeleteBehavior.Restrict); // Deleting TicketActivity will not delete its Ticket


            // Ticket and Ticket Feedback
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Feedback)
                .WithOne(f => f.Ticket)
                .HasForeignKey<Feedback>(f => f.TicketId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting Ticket will delete Feedback

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Ticket)
                .WithOne(t => t.Feedback)
                .HasForeignKey<Ticket>(t => t.TicketId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureOtherEntities(ModelBuilder modelBuilder)
        {
            // Article to Category

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .IsRequired();

            modelBuilder.Entity<ArticleCategory>()
                .HasMany(ac  => ac.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //TicketActivity to TicketOperation

            modelBuilder.Entity<TicketActivity>()
                .HasOne(ta => ta.Operation)
                .WithMany(o => o.TicketActivities)
                .HasForeignKey(ta => ta.OperationId)
                .IsRequired();

            modelBuilder.Entity<TicketOperation>()
                .HasMany(to => to.TicketActivities)
                .WithOne(ta => ta.Operation)
                .HasForeignKey (ta => ta.OperationId)
                .OnDelete (DeleteBehavior.Restrict);
        }
    }
}
