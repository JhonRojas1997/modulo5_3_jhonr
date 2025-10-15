using Microsoft.AspNetCore.Mvc;
using Modulo5_3_JhonR.Data;
using Microsoft.EntityFrameworkCore;
using Modulo5_3_JhonR.Models;

namespace Modulo5_3_JhonR.Controllers
{
    public class AppointmentController : Controller
    {
        private PostgresDbContext _context;
        public AppointmentController(PostgresDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient) // Relación con Patient
                .Include(a => a.Doctor)
                .ToList();

            ViewBag.Doctors = _context.Doctors.Where(d => d.Status == "Activo").ToList();
            return View(appointments);
        }

        public IActionResult Details(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                TempData["message"] = "Cita no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        [HttpGet]
        public IActionResult GetPatientByNuip(string nuip)
        {
            
            var patient = _context.Patients.FirstOrDefault(p => p.DocumentNumber == nuip);

            if (patient == null)
                return Json(new { found = false });

            return Json(new
            {
                found = true,
                id = patient.Id,
                name = patient.FullName
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
    
            ViewBag.Patients = _context.Patients.ToList();
            ViewBag.Doctors = _context.Doctors.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Date,Hour,Status,PatientId,DoctorId")] Appointment appointment)
        {
            Console.WriteLine($"Date: {appointment.Date}, Hour: {appointment.Hour}, PatientId: {appointment.PatientId}, DoctorId: {appointment.DoctorId}, Status: {appointment.Status}");

  
            var patient = _context.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
            if (patient == null)
            {
                TempData["error"] = "Paciente no encontrado";
                return RedirectToAction(nameof(Index));
            }

            if (patient.Status != "Activo")
            {
                TempData["error"] = "El paciente no se encuentra activo";
                return RedirectToAction(nameof(Index));
            }

      
            if (TimeSpan.TryParse(appointment.Hour.ToString(), out var horaSeleccionada))
            {
                appointment.Hour = appointment.Date.Date.Add(horaSeleccionada);
            }

     
            appointment.Date = DateTime.SpecifyKind(appointment.Date, DateTimeKind.Utc);
            appointment.Hour = DateTime.SpecifyKind(appointment.Hour, DateTimeKind.Utc);

      
            var startOfDay = appointment.Date.Date;
            var endOfDay = startOfDay.AddDays(1);

            bool citaExistente = _context.Appointments.Any(a =>
                a.DoctorId == appointment.DoctorId &&
                a.Date >= startOfDay && a.Date < endOfDay &&
                a.Hour == appointment.Hour
            );

            if (citaExistente)
            {
                TempData["error"] = "El doctor ya tiene una cita programada en esa fecha y hora.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["message"] = "Cita creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = _context.Patients.ToList();
            ViewBag.Doctors = _context.Doctors.ToList();
            return View(appointment);
        }

        public IActionResult Edit(int id, [Bind("Date,Hour")] Appointment updateAppointment)
        {
            var appointment = _context.Appointments.Find(id);

            if (appointment == null)
            {
                TempData["error"] = "Cita no encontrada";
                return RedirectToAction(nameof(Index));
            }

            appointment.Date = DateTime.SpecifyKind(updateAppointment.Date, DateTimeKind.Utc);
            appointment.Hour = DateTime.SpecifyKind(updateAppointment.Hour, DateTimeKind.Utc);
            _context.Appointments.Update(appointment);
            _context.SaveChanges();

            TempData["message"] = "Cita editada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                TempData["error"] = "Cita no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Doctors = _context.Doctors.ToList();
            return View(appointment);
        }
        

        [HttpGet]
        public IActionResult ValidatePatientAvailability(int patientId, DateTime date, string hour)
        {
        
            if (TimeSpan.TryParse(hour, out var horaSeleccionada))
            {
                var citaHora = DateTime.SpecifyKind(date.Date.Add(horaSeleccionada), DateTimeKind.Utc);

                var citaExistente = _context.Appointments.Any(a =>
                    a.PatientId == patientId &&
                    a.Date.Date == citaHora.Date && // Asegúrate de comparar en UTC
                    a.Hour == citaHora
                );

                return Json(new { available = !citaExistente });
            }

            return Json(new { available = true });
        }



        [HttpGet]
        public IActionResult ValidateAvailability(int doctorId, DateTime date, string hour)
        {
     
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            // Rango del día completo
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            var appointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date >= startOfDay && a.Date < endOfDay)
                .AsEnumerable();

            bool available = !appointments.Any(a => a.Hour.ToString("HH:mm") == hour);

            return Json(new { available });
        }

        public IActionResult Cancel(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                TempData["error"] = "El appointment no se encontrado.";
            }
            appointment.Status = "Cancelado";
            _context.Appointments.Update(appointment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Acept(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                TempData["error"] = "El appointment no se encontrado.";
            }
            appointment.Status = "Atendido";
            _context.Appointments.Update(appointment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
       
    }
    
}
