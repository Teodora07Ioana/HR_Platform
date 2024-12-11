using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HR_Platform.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using HR_Platform.Hubs;

namespace HR_Platform.Controllers
{
    public class EmployeeTrainingsController : Controller
    {
        private readonly HRManagementContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmployeeTrainingsController(HRManagementContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: EmployeeTrainings
        public async Task<IActionResult> Index()
        {
            try
            {
                var employeeTrainings = await _context.EmployeeTrainings
                    .Include(e => e.Employee)
                    .Include(e => e.TrainingProgram)
                    .ToListAsync();

                Console.WriteLine($"Found {employeeTrainings.Count} employee trainings.");

                return View(employeeTrainings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return View(new List<EmployeeTraining>());
            }
        }


        // GET: EmployeeTrainings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmployeeTrainings == null)
            {
                return NotFound();
            }

            var employeeTraining = await _context.EmployeeTrainings
                .Include(e => e.Employee)
                .Include(e => e.TrainingProgram)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employeeTraining == null)
            {
                return NotFound();
            }

            return View(employeeTraining);
        }

        // GET: EmployeeTrainings/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName");
            ViewData["TrainingProgramID"] = new SelectList(_context.TrainingPrograms, "TrainingProgramID", "Description");
            return View();
        }

        // POST: EmployeeTrainings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,TrainingProgramID")] EmployeeTraining employeeTraining)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeTraining);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", employeeTraining.EmployeeID);
            ViewData["TrainingProgramID"] = new SelectList(_context.TrainingPrograms, "TrainingProgramID", "Description", employeeTraining.TrainingProgramID);
            return View(employeeTraining);
        }

        // GET: EmployeeTrainings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmployeeTrainings == null)
            {
                return NotFound();
            }

            var employeeTraining = await _context.EmployeeTrainings.FindAsync(id);
            if (employeeTraining == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", employeeTraining.EmployeeID);
            ViewData["TrainingProgramID"] = new SelectList(_context.TrainingPrograms, "TrainingProgramID", "Description", employeeTraining.TrainingProgramID);
            return View(employeeTraining);
        }

        // POST: EmployeeTrainings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,TrainingProgramID")] EmployeeTraining employeeTraining)
        {
            if (id != employeeTraining.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeTraining);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeTrainingExists(employeeTraining.EmployeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", employeeTraining.EmployeeID);
            ViewData["TrainingProgramID"] = new SelectList(_context.TrainingPrograms, "TrainingProgramID", "Description", employeeTraining.TrainingProgramID);
            return View(employeeTraining);
        }

        // GET: EmployeeTrainings/Delete/5
        public async Task<IActionResult> Delete(int employeeId, int trainingProgramId)
        {
            var employeeTraining = await _context.EmployeeTrainings
                .FirstOrDefaultAsync(et => et.EmployeeID == employeeId && et.TrainingProgramID == trainingProgramId);

            if (employeeTraining == null)
            {
                return NotFound();
            }

            return View(employeeTraining);
        }


        // POST: EmployeeTrainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int employeeId, int trainingProgramId)
        {
            // Găsește entitatea folosind cheia compusă
            var employeeTraining = await _context.EmployeeTrainings
                .FirstOrDefaultAsync(et => et.EmployeeID == employeeId && et.TrainingProgramID == trainingProgramId);

            if (employeeTraining == null)
            {
                return NotFound();
            }

            _context.EmployeeTrainings.Remove(employeeTraining);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool EmployeeTrainingExists(int id)
        {
            return (_context.EmployeeTrainings?.Any(e => e.EmployeeID == id)).GetValueOrDefault();
        }
        

        [HttpPost]
        public async Task<IActionResult> Enroll(int trainingProgramId)
        {
            // Obține email-ul utilizatorului autentificat
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                await _hubContext.Clients.User(email)
                    .SendAsync("ReceiveNotification", "Email-ul utilizatorului nu a fost găsit.");
                return RedirectToAction("Index", "TrainingPrograms");
            }

            // Găsește angajatul asociat email-ului
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                await _hubContext.Clients.User(email)
                    .SendAsync("ReceiveNotification", "Angajatul nu a fost găsit.");
                return RedirectToAction("Index", "TrainingPrograms");
            }

            // Verifică dacă programul de training există
            var program = await _context.TrainingPrograms.FindAsync(trainingProgramId);
            if (program == null)
            {
                await _hubContext.Clients.User(email)
                    .SendAsync("ReceiveNotification", "Programul de training nu a fost găsit.");
                return RedirectToAction("Index", "TrainingPrograms");
            }

            // Verifică dacă înscrierea există deja
            var existingEnrollment = await _context.EmployeeTrainings
                .FirstOrDefaultAsync(et => et.EmployeeID == employee.EmployeeID && et.TrainingProgramID == trainingProgramId);

            if (existingEnrollment != null)
            {
                await _hubContext.Clients.User(email)
                    .SendAsync("ReceiveNotification", "Ești deja înscris la acest program.");
                return RedirectToAction("Index", "TrainingPrograms");
            }

            // Creează o nouă înregistrare
            var employeeTraining = new EmployeeTraining
            {
                EmployeeID = employee.EmployeeID,
                TrainingProgramID = trainingProgramId
            };

            _context.EmployeeTrainings.Add(employeeTraining);
            await _context.SaveChangesAsync(); // Salvează înregistrarea înainte de notificare

            // Trimite notificarea prin SignalR
            await _hubContext.Clients.User(email)
                .SendAsync("ReceiveNotification", $"Te-ai înscris cu succes la programul {program.ProgramName}!");

            return RedirectToAction("Index", "TrainingPrograms");
        }




    }
}
