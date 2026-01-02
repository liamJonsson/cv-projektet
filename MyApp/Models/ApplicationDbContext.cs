using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyApp.Models;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // Tabellerna i databasen
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Säger åt EF Core att ignorera att hash-koderna ändras
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            var hasher = new PasswordHasher<User>();

            // Behaviour
            modelBuilder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.ProjectId, pu.UserId });

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ParticipatingProjects)
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.Participants)
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //Exempel data
            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, HomeAddress = "Exempelgatan 10", City = "Stockholm", ZipCode = "123 45" }
            );

            var lisa = new User
            {
                Id = 1,
                Name = "Lisa Skarf",
                ProfileImage = "default.jpg",
                Email = "lisa.skarf@example.com",
                NormalizedEmail = "LISA.SKARF@EXAMPLE.COM",
                UserName = "lisaskarf",
                NormalizedUserName = "LISASKARF",
                PhoneNumber = "0720204584",
                Cv = "cv_lisa.pdf",
                Visibility = true,
                Deactivated = false,
                AddressId = 1,
                EmailConfirmed = true,
                SecurityStamp = "11111111-1111-1111-1111-111111111111",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
            };
            lisa.PasswordHash = hasher.HashPassword(lisa, "Lösenord123!");

            var liam = new User
            {
                Id = 2,
                Name = "Liam Jonsson",
                ProfileImage = "default.jpg",
                Email = "liam.jonsson@example.com",
                NormalizedEmail = "LIAM.JONSSON@EXAMPLE.COM",
                UserName = "liamjonsson",
                NormalizedUserName = "LIAMJONSSON",
                PhoneNumber = "0737528105",
                Cv = "cv_liam.pdf",
                Visibility = true,
                Deactivated = false,
                AddressId = 1,
                EmailConfirmed = true,
                SecurityStamp = "33333333-3333-3333-3333-333333333333",
                ConcurrencyStamp = "44444444-4444-4444-4444-444444444444"
            };
            
            liam.PasswordHash = hasher.HashPassword(liam, "Lösenord123!");

            // Lägger till dem i databasen
            modelBuilder.Entity<User>().HasData(lisa, liam);

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    ProjectId = 1,
                    Title = "Mitt Första C# Projekt",
                    Description = "En enkel konsolapplikation.",
                    CodeLanguage = "C#",
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2025, 12, 15),
                    ZipFile = "console.zip",
                    CreatorId = 1,
                },
                new Project
                {
                    ProjectId = 2,
                    Title = "React Frontend",
                    Description = "En snygg frontend-app.",
                    CodeLanguage = "JavaScript",
                    StartDate = new DateTime(2025, 9, 12),
                    EndDate = new DateTime(2025, 12, 23),
                    ZipFile = "react.zip",
                    CreatorId = 2,
                }
            );

            modelBuilder.Entity<ProjectUser>().HasData(
                new ProjectUser { ProjectId = 1, UserId = 1 },

                new ProjectUser { ProjectId = 1, UserId = 2 },

                new ProjectUser { ProjectId = 2, UserId = 2 }
            );

            modelBuilder.Entity<Message>().HasData(
                new Message {
                    MessageId = 1,
                    Text = "Hej hej. Vilket bra projekt. Hur har du tänkt när du gjorde Add-metoden? Vill gärna lära mig av dig. Hör av dig ifall du är intresserad att vara min handledare!!", 
                    SentAt = new DateTime(2025, 9, 12, 14, 30, 0),
                    Read = false, 
                    SenderName = "Lisa Skarf", 
                    SenderId = 1, 
                    ReceiverId = 2 
                },
                 new Message
                 {
                     MessageId = 2,
                     Text = "Gott nytt år!",
                     SentAt = new DateTime(2025, 12, 31, 23, 59, 0),
                     Read = false,
                     SenderName = "Meja Ammer",
                     SenderId = 1,
                     ReceiverId = 2
                 },
                 new Message
                 {
                     MessageId = 3,
                     Text = "Hej Liam! Hur har du det på lovet? Har du haft en bra jul? Vi ses snart. Hör gärna av dig. När vi ses ska vi programmera klart systemet. Ha det gott! /Lisa",
                     SentAt = new DateTime(2025, 12, 29, 8, 0, 0),
                     Read = false,
                     SenderName = "Lisa Skarf",
                     SenderId = 1,
                     ReceiverId = 2
                 }
            );
        }
    }
}
