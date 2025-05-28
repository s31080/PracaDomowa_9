namespace PracaDomowa_9.DTOs;

public class PrescriptionCreateDto
{
    public PatientGetDto Patient { get; set; } = null!;
    public List<PrescriptionMedicamentCreateDto> Medicaments { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdDoctor { get; set; }
}

public class PrescriptionMedicamentCreateDto
{
    public int IdMedicament { get; set; }
    public int? Dose { get; set; }
    public string? Description { get; set; }
}
