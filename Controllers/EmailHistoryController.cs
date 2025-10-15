// EmailHistoryController.cs
using Microsoft.AspNetCore.Mvc;
using Modulo5_3_JhonR.Data;
using Modulo5_3_JhonR.Models;

public class EmailHistoryController : Controller
{
    private readonly PostgresDbContext _context;

    public EmailHistoryController(PostgresDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var emailHistory = _context.EmailHistories.ToList();
        return View(emailHistory);
    }
}