using Microsoft.EntityFrameworkCore;

namespace MyApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // Tabellerna i databasen
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Behaviour
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

            //Example data
            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, HomeAddress = "Exempelgatan 10", City = "Stockholm", ZipCode = "123 45" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Lisa Skarf",
                    ProfileImage = "default.jpg",
                    Email = "lisa.skarf@example.com",
                    UserName = "lisaskarf",
                    Password = "passwordlisa",
                    PhoneNumber = "0720204584",
                    Cv = "cv_lisa.pdf",
                    Visibility = true,
                    Deactivated = false,
                    AddressId = 1
                },
                new User
                {
                    UserId = 2,
                    Name = "Liam Jonsson",
                    ProfileImage = "default.jpg",
                    Email = "liam.jonsson@example.com",
                    UserName = "liamjonsson",
                    Password = "passwordliam",
                    PhoneNumber = "0737528105",
                    Cv = "cv_liam.pdf",
                    Visibility = true,
                    Deactivated = false,
                    AddressId = 1
                }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    ProjectId = 1,
                    Title = "Mitt Första C# Projekt",
                    Description = "En enkel konsolapplikation.",
                    CodeLanguage = "C#",
                    StartDate = new DateOnly(2025, 10, 1),
                    EndDate = new DateOnly(2025, 12, 15),
                    ZipFile = "console.zip",
                    CreatorId = 1
                },
                new Project
                {
                    ProjectId = 2,
                    Title = "React Frontend",
                    Description = "En snygg frontend-app.",
                    CodeLanguage = "JavaScript",
                    StartDate = new DateOnly(2025, 9, 12),
                    EndDate = new DateOnly(2025, 12, 23),
                    ZipFile = "react.zip",
                    CreatorId = 2
                }
            );

            modelBuilder.Entity<ProjectUser>().HasData(
                new ProjectUser { ProjectId = 1, UserId = 1 },

                new ProjectUser { ProjectId = 1, UserId = 2 },

                new ProjectUser { ProjectId = 2, UserId = 2 }
            );

            modelBuilder.Entity<Message>().HasData(
                new Message { MessageId = 1, Text = "Hej", Read = false, SenderName = "Lisa Skarf", SenderId = 1, ReceiverId = 2 }
            );


        }
    }
}
