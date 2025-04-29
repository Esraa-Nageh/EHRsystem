using Microsoft.EntityFrameworkCore;
using EHRsystem.Models.Base;
using EHRsystem.Models.Entities;

namespace EHRsystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalFile> MedicalFiles { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PharmacyRecommendation> PharmacyRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<MedicalFile>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalFiles)
                .HasForeignKey(m => m.PatientId);

            modelBuilder.Entity<MedicalFile>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId);

            modelBuilder.Entity<PharmacyRecommendation>()
                .HasOne(r => r.Prescription)
                .WithMany()
                .HasForeignKey(r => r.PrescriptionId);
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Role")
                .HasValue<Patient>("Patient")
                .HasValue<Doctor>("Doctor");

        }
    }
}
