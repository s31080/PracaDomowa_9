using Microsoft.EntityFrameworkCore;
using PracaDomowa_9.Controllers;
using PracaDomowa_9.Data;
using PracaDomowa_9.DTOs;
using PracaDomowa_9.Exceptions;
using PracaDomowa_9.Models;

namespace PracaDomowa_9.DbService;

public interface IDbService
{
    Task<PatientGetDto> GetPatientDetailsAsync(int idPatient);
    Task AddPrescriptionAsync(PrescriptionCreateDto dto);
}

public class DbService(AppDbContext data) : IDbService
{
    


    public async Task AddPrescriptionAsync(PrescriptionCreateDto dto)
{
    if (dto.Medicaments.Count > 10)
        throw new BadRequestException("Prescription cannot contain more than 10 medicaments.");

    if (dto.DueDate < dto.Date)
        throw new BadRequestException("DueDate must be greater than or equal to Date.");

    //czy leki istnieja
    var medicamentIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
    var existingMedicaments = await data.Medicaments.Where(m => medicamentIds.Contains(m.IdMedicament)).Select(m => m.IdMedicament).ToListAsync();
    var notFound = medicamentIds.Except(existingMedicaments).ToList();
    if (notFound.Any())
        throw new PrescriptionsController.NotFoundException($"Medicament(s) with id(s): {string.Join(",", notFound)} not found.");

    //pacjent
    var patient = await data.Patients.FirstOrDefaultAsync(p => p.IdPatient == dto.Patient.IdPatient);
    if (patient == null)
    {
        patient = new Patient
        {
            FirstName = dto.Patient.FirstName,
            LastName = dto.Patient.LastName,
            Birthdate = dto.Patient.Birthdate,
        };
        data.Patients.Add(patient);
        await data.SaveChangesAsync();
    }

    //recepta
    var prescription = new Prescription
    {
        Date = dto.Date,
        DueDate = dto.DueDate,
        IdPatient = patient.IdPatient,
        IdDoctor = dto.IdDoctor
    };
    data.Prescriptions.Add(prescription);
    await data.SaveChangesAsync();

    //leki na recepcie
    foreach (var med in dto.Medicaments)
    {
        data.PrescriptionMedicaments.Add(new Prescription_Medicament
        {
            IdPrescription = prescription.IdPrescription,
            IdMedicament = med.IdMedicament,
            Dose = med.Dose,
            Details = med.Description
        });
    }
    await data.SaveChangesAsync();
}

public async Task<PatientGetDto> GetPatientDetailsAsync(int idPatient)
{
    var patient = await data.Patients
        .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
        .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
        .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

    if (patient == null)
        throw new PrescriptionsController.NotFoundException($"Patient with id {idPatient} not found");

    return new PatientGetDto
    {
        IdPatient = patient.IdPatient,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        Birthdate = patient.Birthdate,
        Prescriptions = patient.Prescriptions
            .OrderBy(pr => pr.DueDate)
            .Select(pr => new PrescriptionGetDto
            {
                IdPrescription = pr.IdPrescription,
                Date = pr.Date,
                DueDate = pr.DueDate,
                Medicaments = pr.PrescriptionMedicaments.Select(pm => new MedicamentGetDto
                {
                    IdMedicament = pm.IdMedicament,
                    Name = pm.Medicament.Name,
                    Dose = pm.Dose,
                    Description = pm.Details
                }).ToList(),
                Doctor = new DoctorGetDto
                {
                    IdDoctor = pr.Doctor.IdDoctor,
                    FirstName = pr.Doctor.FirstName,
                    LastName = pr.Doctor.LastName
                }
            }).ToList()
    };
}

}