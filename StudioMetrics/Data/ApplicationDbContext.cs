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

            // Restrict deletion of related project when PlayerProjects entry is removed
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
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

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

        }
    }
}
