namespace Modulo5_3_JhonR.Models;

public class EmailHistory
{
    public int Id { get; set; }
    public string? EmailDestino { get; set; }
    public string? Asunto { get; set; }
    public string? Mensaje { get; set; }
    public string? Estado { get; set; }
    public DateTime FechaEnvio { get; set; }
    public string? MensajeError { get; set; }


    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
}



