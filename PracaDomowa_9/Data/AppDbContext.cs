using Microsoft.EntityFrameworkCore;
using PracaDomowa_9.Models;

namespace PracaDomowa_9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription_Medicament> PrescriptionMedicaments { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var doctor = new Doctor
        {
            IdDoctor = 1,
            FirstName = "Doctor",
            LastName = "Queen",
            Email = "queen@doctor.com"
        };

        var patient = new Patient
        {
            IdPatient = 1,
            FirstName = "Peter",
            LastName = "Jones",
            Birthdate = new DateTime(1980, 1, 1),
        };

        var prescription = new Prescription
        {
            IdPrescription = 1,
            Date = new DateTime(2018, 1, 1),
            DueDate = new DateTime(2018, 1, 1),
            IdPatient = 1,
            IdDoctor = 1,

        };

        var medicament = new Medicament
        {
            IdMedicament = 1,
            Name = "Medicament",
            Description = "Medicament",
            Type = "Normal"
        };

        var prescriptionmedicament = new Prescription_Medicament
        {
            IdMedicament = 1,
            IdPrescription = 1,
            Dose = 3,
            Details = "Medicament",

        };
        
        
        modelBuilder.Entity<Doctor>().HasData(doctor);
        modelBuilder.Entity<Patient>().HasData(patient);
        modelBuilder.Entity<Prescription>().HasData(prescription);
        modelBuilder.Entity<Medicament>().HasData(medicament);
        modelBuilder.Entity<Prescription_Medicament>().HasData(prescriptionmedicament);
    }
}