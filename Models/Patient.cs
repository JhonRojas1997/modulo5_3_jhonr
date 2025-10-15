namespace Modulo5_3_JhonR.Models;

public class Patient
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string DocumentNumber { get; set; }
    public required int Age { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Status  { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<EmailHistory> EmailHistories { get; set; } = new List<EmailHistory>();
}