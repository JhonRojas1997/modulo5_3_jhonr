namespace Modulo5_3_JhonR.Models;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime Hour { get; set; }
    public string Status {get; set;} = "Confirmed";
    public int? PatientId  { get; set; }
    public Patient? Patient { get; set; }
    
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
}