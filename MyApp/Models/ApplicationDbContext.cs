using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity
            base.OnModelCreating(modelBuilder);

            //DENNA KAN TAS BORT SENARE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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

            //Example data
            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, HomeAddress = "Exempelgatan 10", City = "Stockholm", ZipCode = "123 45" }
            );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1, 
                    Name = "Lisa Skarf",
                    ProfileImage = "default.jpg",
                    Email = "lisa.skarf@example.com",
                    NormalizedEmail = "LISA.SKARF@EXAMPLE.COM",
                    UserName = "lisaskarf",
                    NormalizedUserName = "LISASKARF",
                    PasswordHash = "HASH_PLACEHOLDER", // PasswordHash
                    PhoneNumber = "0720204584",
                    Cv = "cv_lisa.pdf",
                    Visibility = true,
                    Deactivated = false,
                    AddressId = 1,
                    EmailConfirmed = true,
                    SecurityStamp = "78901234-5678-9012-3456-789012345678",
                    ConcurrencyStamp = "d2eb4f2f-6e57-4d1a-9c11-a93aa5825c19"
                },
                new User
                {
                    Id = 2,
                    Name = "Liam Jonsson",
                    ProfileImage = "default.jpg",
                    Email = "liam.jonsson@example.com",
                    NormalizedEmail = "LIAM.JONSSON@EXAMPLE.COM",
                    UserName = "liamjonsson",
                    NormalizedUserName = "LIAMJONSSON",
                    PasswordHash = "HASH_PLACEHOLDER",
                    PhoneNumber = "0737528105",
                    Cv = "cv_liam.pdf",
                    Visibility = true,
                    Deactivated = false,
                    AddressId = 1,
                    EmailConfirmed = true,
                    SecurityStamp = "12345678 - 1234 - 5678 - 1234 - 567812345678",
                    ConcurrencyStamp = "5e632b10-1032-48d9-b9db-b8dbada42280"
                },
                //DENNA KAN TAS BORT SENARE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                new User
                {
                    Id = 3,
                    Name = "Test Testsson",
                    ProfileImage = "default.jpg",
                    Email = "test@test.se",
                    NormalizedEmail = "TEST@TEST.SE",
                    UserName = "testanvandare",
                    NormalizedUserName = "TESTANVANDARE",
                    PasswordHash = hasher.HashPassword(null, "Lösenord123!"),
                    PhoneNumber = "0700000000",
                    Cv = "cv_test.pdf",
                    Visibility = true,
                    Deactivated = false,
                    AddressId = 1,
                    EmailConfirmed = true,
                    SecurityStamp = "8d2eb4f2f - 6e57 - 4d1a - 9c11 - a93aa5825c19",
                    ConcurrencyStamp = "b2b4f2f6 - e574 - d1a9 - c11a - 93aa5825c19"
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
                    CreatorId = 1,
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
                    Text = "Hej", 
                    Read = false, 
                    SenderName = "Lisa Skarf", 
                    SenderId = 1, 
                    ReceiverId = 2 
                },
                //DENNA KAN TAS BORT SENARE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                new Message
                {
                    MessageId = 2,
                    Text = "Funkar inloggningen?",
                    Read = false,
                    SenderName = "Lisa Skarf",
                    SenderId = 1,
                    ReceiverId = 3 
                }
            );


        }
    }
}
