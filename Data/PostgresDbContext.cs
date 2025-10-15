using Microsoft.EntityFrameworkCore;
namespace Modulo5_3_JhonR.Data;
using Modulo5_3_JhonR.Models;
public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<EmailHistory> EmailHistories { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>()
            .HasMany(p=> p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<EmailHistory>()
            .HasOne(eh => eh.Patient) // Establece la relación entre EmailHistory y Patient
            .WithMany(p => p.EmailHistories) // Especifica la colección inversa en Patient (muchos EmailHistories)
            .HasForeignKey(eh => eh.PatientId) // Define la clave foránea en EmailHistory
            .OnDelete(DeleteBehavior.SetNull); // Define el comportamiento de eliminación (puedes cambiarlo según necesidad)


    }
}