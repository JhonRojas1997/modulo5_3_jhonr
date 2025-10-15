using Microsoft.AspNetCore.Mvc;
using Modulo5_3_JhonR.Data;
using Modulo5_3_JhonR.Models;
using MailKit.Net.Smtp;
using MimeKit;
namespace Modulo5_3_JhonR.Controllers;

public class PatientController: Controller
{
    private readonly PostgresDbContext _context;
    public PatientController(PostgresDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var patients = _context.Patients.ToList();
        return View(patients);
    }

    [HttpPost]
   public IActionResult Create([Bind("FullName,DocumentNumber,PhoneNumber,Email,Age,Status")] Patient patient)
{
    
    bool isValid = _context.Patients.Any(p => p.DocumentNumber == patient.DocumentNumber);
    if (isValid)
    {
        TempData["message"] = "Paciente ya existe";
        return RedirectToAction("Index");
    }

    
    if (string.IsNullOrWhiteSpace(patient.FullName))
    {
        TempData["message"] = "El nombre completo es obligatorio.";
        return RedirectToAction("Index");
    }

    if (string.IsNullOrWhiteSpace(patient.DocumentNumber))
    {
        TempData["message"] = "El número de documento es obligatorio.";
        return RedirectToAction("Index");
    }

    if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
    {
        TempData["message"] = "El número de teléfono es obligatorio.";
        return RedirectToAction("Index");
    }

    if (string.IsNullOrWhiteSpace(patient.Email))
    {
        TempData["message"] = "El correo electrónico es obligatorio.";
        return RedirectToAction("Index");
    }
    else if (!IsValidEmail(patient.Email))
    {
        TempData["message"] = "El correo electrónico no es válido.";
        return RedirectToAction("Index");
    }

    if (patient.Age <= 0)
    {
        TempData["message"] = "La edad debe ser un valor mayor a 0.";
        return RedirectToAction("Index");
    }

   
    if (!ModelState.IsValid)
    {
        
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        TempData["message"] = "Errores de validación: " + string.Join(", ", errors);
        return RedirectToAction("Index");
    }

    try
    {
        patient.Status = "Activo";  
        

        _context.Patients.Add(patient);
        _context.SaveChanges();

      
        EnviarCorreoMailKit(patient.Email, patient.FullName, patient.DocumentNumber);

       
        TempData["message"] = "Paciente creado satisfactoriamente";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
       
        TempData["message"] = $"Error al guardar el paciente: {ex.Message}";
        return RedirectToAction("Index");
    }
}


private bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}


    public IActionResult Destroy(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            return NotFound();
        }
        if (patient.Status == "Activo")
        {
            patient.Status = "Inactivo";
            TempData["message"] = "Paciente inactivado exitosamente!";
        }
        else if (patient.Status == "Inactivo")
        {
            patient.Status = "Activo";
            TempData["message"] = "Paciente activado exitosamente!";
        }

        _context.Patients.Update(patient);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            return NotFound();
        }
        return View(patient);
        
    }
    [HttpPost]
    public IActionResult Edit(int id, [Bind("FullName,PhoneNumber,Email,Age")] Patient updatePatient)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            return NotFound();
        }
        

        if (string.IsNullOrWhiteSpace(updatePatient.FullName))
        {
            TempData["message"] = "El nombre completo es obligatorio.";
            return View(updatePatient);
        }

        if (string.IsNullOrWhiteSpace(updatePatient.PhoneNumber))
        {
            TempData["message"] = "El número de teléfono es obligatorio.";
            return View(updatePatient);
        }

        if (updatePatient.Age <= 0)
        {
            TempData["message"] = "La edad debe ser un valor mayor a 0.";
            return View(updatePatient);
        }
        
        patient.FullName = updatePatient.FullName;
        patient.Age = updatePatient.Age;
        patient.Email = updatePatient.Email; // Asumiendo que el correo se mantiene
        patient.PhoneNumber = updatePatient.PhoneNumber;

        _context.SaveChanges();

        TempData["message"] = "Paciente actualizado correctamente.";
        return RedirectToAction("Index");
    }


    public IActionResult Details(int id)
    {
        var patient = _context.Patients.Find(id);
        return View(patient);
    }

    public void EnviarCorreoMailKit(string destino, string nombre, string documento)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress($"Bienvenid@ {nombre}", "jfrojas1997@gmail.com"));
        mensaje.To.Add(MailboxAddress.Parse(destino));
        mensaje.Subject = "Bienvenido";

        mensaje.Body = new TextPart("plain")
        {
            Text = $"Hola {nombre} has sido registrado con el numero de documento {documento}." +
                   $"Gracias por registrarte en nuestro sistema de salud saludcita de la buena."
        };

        var estadoEnvio = "No Enviado";  
        string? mensajeError = null; 

        try
        {
            using (var cliente = new SmtpClient())
            {
                cliente.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                cliente.Authenticate("jfrojas1997@gmail.com", "xujgbgeqmvlfzsal");
                cliente.Send(mensaje);
                cliente.Disconnect(true);
            }

            estadoEnvio = "Enviado";  // Si todo fue bien, cambiamos el estado a "Enviado"
        }
        catch (Exception ex)
        {
            mensajeError = ex.Message;  
        }
        finally
        {
            // Usar DateTime.UtcNow en lugar de DateTime.Now
            var emailHistory = new EmailHistory
            {
                EmailDestino = destino,
                Asunto = mensaje.Subject,
                Mensaje = mensaje.Body.ToString(),
                Estado = estadoEnvio,
                FechaEnvio = DateTime.UtcNow,  // Asegurándonos de que esté en UTC
                MensajeError = mensajeError
            };

            _context.EmailHistories.Add(emailHistory);
            _context.SaveChanges();
        }
    }


}