using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }
        public DbSet<Job> Job
            { get; set; }
        public DbSet<JobApplication> JobApplication
        { get; set; }
        public DbSet<JobApplicationStatusHistory> JobApplicationStatusHistory
        { get; set; }
        public DbSet<CandidateProfile> CandidateProfile
        { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Required: IdentityDbContext configures the Identity tables here.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobApplication>(entity =>
            {
                // Must match AspNetUsers.Id (nvarchar(450)) so it can be a foreign key and part of an index.
                entity.Property(a => a.JobSeekerId).HasMaxLength(450);

                // Required relationships; deleting a Job or user also removes their applications.
                entity.HasOne<Job>().WithMany().HasForeignKey(a => a.JobId);
                entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(a => a.JobSeekerId);

                // One application per JobSeeker per Job, guaranteed by the database itself.
                entity.HasIndex(a => new { a.JobId, a.JobSeekerId }).IsUnique();
            });

            modelBuilder.Entity<JobApplicationStatusHistory>(entity =>
            {
                // Same size as the user ids it stores. Not a foreign key on purpose: see ChangedByUserId.
                entity.Property(h => h.ChangedByUserId).HasMaxLength(450);

                // History belongs to its application: if an application is ever removed, its history goes with it.
                entity.HasOne<JobApplication>().WithMany().HasForeignKey(h => h.JobApplicationId);
            });

            modelBuilder.Entity<CandidateProfile>(entity =>
            {
                // Must match AspNetUsers.Id (nvarchar(450)) so it can be a foreign key and part of an index.
                entity.Property(p => p.JobSeekerId).HasMaxLength(450);

                // Required relationship; deleting a user also removes their profile.
                entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(p => p.JobSeekerId);

                // One profile per JobSeeker, guaranteed by the database itself.
                entity.HasIndex(p => p.JobSeekerId).IsUnique();
            });
        }
    }
}
