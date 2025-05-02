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

            // Appointment -> Doctor (NO navigation property in Appointment class)
            modelBuilder.Entity<Appointment>()
                .HasOne<Doctor>() // no a.Doctor property
                .WithMany()
                .HasForeignKey(a => a.DoctorId);

            // Appointment -> Patient (optional, based on your model)
            modelBuilder.Entity<Appointment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.PatientId);

            // MedicalFile -> Patient
            modelBuilder.Entity<MedicalFile>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalFiles)
                .HasForeignKey(m => m.PatientId);

            // MedicalFile -> Doctor (no navigation needed)
            modelBuilder.Entity<MedicalFile>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId);

            // Prescription -> Patient
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId);

            // Prescription -> Doctor
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId);

            // PharmacyRecommendation -> Prescription
            modelBuilder.Entity<PharmacyRecommendation>()
                .HasOne(r => r.Prescription)
                .WithMany()
                .HasForeignKey(r => r.PrescriptionId);

            // User Role discriminator
            modelBuilder.Entity<User>()
    .HasDiscriminator<string>("Role")
    .HasValue<Doctor>("Doctor")
    .HasValue<Patient>("Patient")
    .HasValue<User>("Admin");

        }
    }
}
