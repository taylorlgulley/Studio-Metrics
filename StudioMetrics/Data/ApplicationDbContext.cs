using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudioMetrics.Models;

namespace StudioMetrics.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Set up each DbSet for each model of what will be in the database
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Artist> Artist { get; set; }
        public DbSet<ArtistProject> ArtistProject { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientArtist> ClientArtist { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<PlayerProject> PlayerProject { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectType> ProjectType { get; set; }
        public DbSet<StatusType> StatusType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restrict deletion of related project when PlayerProjects entry is removed, does the same for ArtistProjects, ClientArtists
            // It will restrict deleting a Project until you delete the joiner tables assocaited with it
            modelBuilder.Entity<Project>()
                .HasMany(p => p.PlayerProjects)
                .WithOne(l => l.Project)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ArtistProjects)
                .WithOne(l => l.Project)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.PlayerProjects)
                .WithOne(l => l.Player)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Artist>()
                .HasMany(p => p.ArtistProjects)
                .WithOne(l => l.Artist)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Artist>()
                .HasMany(p => p.ClientArtists)
                .WithOne(l => l.Artist)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasMany(p => p.ClientArtists)
                .WithOne(l => l.Client)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Projects)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Restrict);

            // Creation of the admin user
            ApplicationUser user = new ApplicationUser
            {
                CompanyName = "admin",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            // This is the creation of the passward and hashing it
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            // The following sections are seeding the database with base data for the Admin user to test the application
            // Additionally data to be present for every used was created like Status Types
            modelBuilder.Entity<ProjectType>().HasData(
                new ProjectType()
                {
                    ProjectTypeId = 1,
                    Type = "Single Song Release"
                },
                new ProjectType()
                {
                    ProjectTypeId = 2,
                    Type = "Extended Play CD"
                },
                new ProjectType()
                {
                    ProjectTypeId = 3,
                    Type = "Full Length CD"
                },
                new ProjectType()
                {
                    ProjectTypeId = 4,
                    Type = "Single Song Demo"
                },
                new ProjectType()
                {
                    ProjectTypeId = 5,
                    Type = "30 Second Audio Commercial"
                },
                new ProjectType()
                {
                    ProjectTypeId = 6,
                    Type = "60 Second Audio Commercial"
                },
                new ProjectType()
                {
                    ProjectTypeId = 7,
                    Type = "Audio Industrial"
                },
                new ProjectType()
                {
                    ProjectTypeId = 8,
                    Type = "Live Concert Recording"
                }
            );

            modelBuilder.Entity<StatusType>().HasData(
                new StatusType()
                {
                    StatusTypeId = 1,
                    Type = "Upcoming"
                },
                new StatusType()
                {
                    StatusTypeId = 2,
                    Type = "Completed"
                },
                new StatusType()
                {
                    StatusTypeId = 3,
                    Type = "Current"
                },
                new StatusType()
                {
                    StatusTypeId = 4,
                    Type = "Tentative"
                }
            );

            modelBuilder.Entity<Artist>().HasData(
                new Artist()
                {
                    ArtistId = 1,
                    UserId = user.Id,
                    Name = "Smashing Pumpkins"
                },
                new Artist()
                {
                    ArtistId = 2,
                    UserId = user.Id,
                    Name = "Lake Street Dive"
                },
                new Artist()
                {
                    ArtistId = 3,
                    UserId = user.Id,
                    Name = "Barns Courtney"
                },
                new Artist()
                {
                    ArtistId = 4,
                    UserId = user.Id,
                    Name = "The Pink Spiders"
                }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client()
                {
                    ClientId = 1,
                    UserId = user.Id,
                    Name = "Mark Hale",
                    Phone = "615-111-1111",
                    Email = "mhaleindustries@mhale.com"
                },
                new Client()
                {
                    ClientId = 2,
                    UserId = user.Id,
                    Name = "David Cunningham",
                    Phone = "615-222-2222",
                    Email = "dcunningham@dcunningham.com"
                },
                new Client()
                {
                    ClientId = 3,
                    UserId = user.Id,
                    Name = "Michelle An",
                    Phone = "615-333-3333",
                    Email = "mansound@sound.com"
                }
            );

            modelBuilder.Entity<Player>().HasData(
                new Player()
                {
                    PlayerId = 1,
                    UserId = user.Id,
                    FirstName = "Abe",
                    LastName = "Laboriel Jr.",
                    Instrument = "Drums",
                    Phone = "999-999-9999",
                    Email = "abelaboriel@paulmcaartney.com"
                },
                new Player()
                {
                    PlayerId = 2,
                    UserId = user.Id,
                    FirstName = "Paul",
                    LastName = "McCartney",
                    Instrument = "Vocals",
                    Phone = "999-999-9999",
                    Email = "paul@paulmccartney.com"
                },
                new Player()
                {
                    PlayerId = 3,
                    UserId = user.Id,
                    FirstName = "Jimmy",
                    LastName = "Hendrix",
                    Instrument = "Guitar",
                    Phone = "999-999-9999",
                    Email = "jhendrix@jhendrix.com"
                },
                new Player()
                {
                    PlayerId = 4,
                    UserId = user.Id,
                    FirstName = "Stevie",
                    LastName = "Wonder",
                    Instrument = "Keyboards",
                    Phone = "999-999-9999",
                    Email = "swonder@swonder.com"
                }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project()
                {
                    ProjectId = 1,
                    Title = "Hey Jude",
                    ProjectTypeId = 1,
                    Description = "An hit in the making",
                    Payrate = 2500,
                    TimeTable = 5,
                    StartDate = new DateTime(2018, 12, 28),
                    StatusTypeId = 1,
                    UserId = user.Id,
                    ClientId = 1
                },
                new Project()
                {
                    ProjectId = 2,
                    Title = "Free Yourself Up",
                    ProjectTypeId = 3,
                    Description = "A great album",
                    Payrate = 1500,
                    TimeTable = 25,
                    StartDate = new DateTime(2018, 11, 04),
                    StatusTypeId = 2,
                    UserId = user.Id,
                    ClientId = 3
                }
            );

            modelBuilder.Entity<PlayerProject>().HasData(
                new PlayerProject()
                {
                    PlayerProjectId = 1,
                    PlayerId = 2,
                    ProjectId = 1
                },
                new PlayerProject()
                {
                    PlayerProjectId = 2,
                    PlayerId = 1,
                    ProjectId = 1
                },
                new PlayerProject()
                {
                    PlayerProjectId = 3,
                    PlayerId = 4,
                    ProjectId = 2
                }
            );

            modelBuilder.Entity<ClientArtist>().HasData(
                new ClientArtist()
                {
                    ClientArtistId = 1,
                    ClientId = 1,
                    ArtistId = 1
                },
                new ClientArtist()
                {
                    ClientArtistId = 2,
                    ClientId = 3,
                    ArtistId = 2
                }
            );

            modelBuilder.Entity<ArtistProject>().HasData(
                new ArtistProject()
                {
                    ArtistProjectId = 1,
                    ArtistId = 1,
                    ProjectId = 1
                },
                new ArtistProject()
                {
                    ArtistProjectId = 2,
                    ArtistId = 2,
                    ProjectId = 2
                }
            );
        }
    }
}
