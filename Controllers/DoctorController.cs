using Microsoft.AspNetCore.Mvc;
using Modulo5_3_JhonR.Data;
using Modulo5_3_JhonR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Modulo5_3_JhonR.Controllers;

public class DoctorController : Controller
{
    private readonly PostgresDbContext _context;

    public DoctorController(PostgresDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Index(string specialtyFilter)
    {
        // Lista de especialidades
        var specialties = new List<SelectListItem>
        {
            new SelectListItem { Text = "Medicina General", Value = "General" },
            new SelectListItem { Text = "Cardiología", Value = "Cardiologia" },
            new SelectListItem { Text = "Dermatología", Value = "Dermatologia" }
        };

        var doctorsQuery = _context.Doctors.AsQueryable();

        if (!string.IsNullOrEmpty(specialtyFilter))
        {
            doctorsQuery = doctorsQuery.Where(d => d.Speciality == specialtyFilter);
        }

        var doctors = doctorsQuery.ToList();

        // Pasamos las especialidades y el filtro a la vista
        ViewBag.Specialties = specialties;
        ViewBag.SpecialtyFilter = specialtyFilter;

        return View(doctors);
    }






    [HttpPost]
    public IActionResult Create([Bind("FullName,DocumentNumber,PhoneNumber,Email,Speciality,Status")] Doctor doctor)
    {

        if (_context.Doctors.Any(d => d.DocumentNumber == doctor.DocumentNumber))
        {
            TempData["message"] = "Ya existe un doctor con el mismo número de documento.";
            return View(doctor);
        }


        if (_context.Doctors.Any(d => d.FullName == doctor.FullName && d.Speciality == doctor.Speciality))
        {
            TempData["message"] = "Ya existe un doctor con el mismo nombre y especialidad.";
            return View(doctor);
        }

        if (string.IsNullOrWhiteSpace(doctor.FullName))
        {
            TempData["message"] = "El nombre completo es obligatorio.";
            return View(doctor);
        }

        if (string.IsNullOrWhiteSpace(doctor.DocumentNumber))
        {
            TempData["message"] = "El número de documento es obligatorio.";
            return View(doctor);
        }

        if (string.IsNullOrWhiteSpace(doctor.PhoneNumber))
        {
            TempData["message"] = "El número de teléfono es obligatorio.";
            return View(doctor);
        }

        if (string.IsNullOrWhiteSpace(doctor.Email))
        {
            TempData["message"] = "El correo electrónico es obligatorio.";
            return View(doctor);
        }

        if (ModelState.IsValid)
        {
            doctor.Status = "Activo";  
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            TempData["message"] = "Doctor creado satisfactoriamente";
            return RedirectToAction("Index");
        }

        return View(doctor);
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }
        return View(doctor);
    }


    [HttpPost]
    public IActionResult Edit(int id, [Bind("FullName,PhoneNumber,Email,Speciality")] Doctor updateDoctor)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }


        if (string.IsNullOrWhiteSpace(updateDoctor.FullName))
        {
            TempData["message"] = "El nombre completo es obligatorio.";
            return View(updateDoctor);
        }

        if (string.IsNullOrWhiteSpace(updateDoctor.PhoneNumber))
        {
            TempData["message"] = "El número de teléfono es obligatorio.";
            return View(updateDoctor);
        }

        if (_context.Doctors.Any(d => d.FullName == updateDoctor.FullName && d.Speciality == updateDoctor.Speciality && d.Id != id))
        {
            TempData["message"] = "Ya existe un doctor con el mismo nombre y especialidad.";
            return View(updateDoctor);
        }


        doctor.FullName = updateDoctor.FullName;
        doctor.Speciality = updateDoctor.Speciality;
        doctor.Email = updateDoctor.Email;
        doctor.PhoneNumber = updateDoctor.PhoneNumber;

        _context.SaveChanges();

        TempData["message"] = "Doctor actualizado correctamente.";
        return RedirectToAction("Index");
    }


    public IActionResult Destroy(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }

        if (doctor.Status == "Activo")
        {
            doctor.Status = "Inactivo";
            TempData["message"] = "Doctor inactivado exitosamente!";
        }
        else if (doctor.Status == "Inactivo")
        {
            doctor.Status = "Activo";
            TempData["message"] = "Doctor activado exitosamente!";
        }

        _context.Doctors.Update(doctor);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    public IActionResult Details(int id)
    {
        var doctor = _context.Doctors.Find(id);
        return View(doctor);
    }
}
