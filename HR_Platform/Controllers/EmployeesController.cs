using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HR_Platform.Models;
using Microsoft.AspNetCore.Authorization;

namespace HR_Platform.Controllers
{
    //[Authorize(Policy = "ManagerPolicy")]
    [Authorize(Policy = "AdminPolicy")]
    public class EmployeesController : Controller
    {
        private readonly HRManagementContext _context;
        private string _baseUrl = "https://localhost:7109/api/Employees";

        public EmployeesController(HRManagementContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SalarySortParm"] = sortOrder == "Salary" ? "salary_desc" : "Salary";
            ViewData["CurrentFilter"] = searchString;

            var employees = _context.Employees.Include(e => e.Department).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.FirstName.Contains(searchString) || e.LastName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.FirstName);
                    break;
                case "Salary":
                    employees = employees.OrderBy(e => e.Salary);
                    break;
                case "salary_desc":
                    employees = employees.OrderByDescending(e => e.Salary);
                    break;
                default:
                    employees = employees.OrderBy(e => e.FirstName);
                    break;
            }

            return View(await employees.AsNoTracking().ToListAsync());
        }



        // GET: Employees/Details/5
        /* public async Task<IActionResult> Details(int? id)
         {
             if (id == null || _context.Employees == null)
             {
                 return NotFound();
             }

             var employee = await _context.Employees
                 .Include(e => e.Department)
                 .FirstOrDefaultAsync(m => m.EmployeeID == id);
             if (employee == null)
             {
                 return NotFound();
             }

             return View(employee);
         }*/
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Details()
        {
            var userEmail = User.Identity.Name; // Obține email-ul utilizatorului conectat
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }


        // GET: Employees/Create
        /* public IActionResult Create()
         {
             //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
             ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");

             return View();
         }*/


        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }


        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,FirstName,LastName,Position,Salary,Email,DepartmentID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            return View(employee);
        }


    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Employees == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
        return View(employee);
    }


        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,FirstName,LastName,Position,Salary,Email,DepartmentID")] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while updating the record.");
                    }
                }
            }
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            return View(employee);
        }


        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'HRManagementContext.Employees' is null.");
            }

            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction("Delete", new { id, error = true });
            }
        }


       private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeID == id)).GetValueOrDefault();
        }
    }
}
