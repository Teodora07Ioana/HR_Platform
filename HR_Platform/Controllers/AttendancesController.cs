using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using HR_Platform.Models;
using System.Security.Claims;

namespace HR_Platform.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7109/api/Attendances";
        private readonly string _employeeApiUrl = "https://localhost:7109/api/Employees"; 

        public AttendancesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<List<Employee>> GetEmployeesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_employeeApiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<Employee>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(json);

                if (employees == null || !employees.Any())
                {
                    Console.WriteLine("No employees found.");
                }

                return employees ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching employees: {ex.Message}");
                return new List<Employee>();
            }
        }


        /* private async Task PopulateEmployeesDropdown(int? selectedEmployeeId = null)
         {
             var employees = await GetEmployeesAsync();

             if (employees == null || !employees.Any())
             {
                 ModelState.AddModelError("", "No employees available to select.");
             }

             ViewBag.Employees = new SelectList(employees, "EmployeeID", "FirstName", selectedEmployeeId);
         }
 */
        private async Task PopulateEmployeesDropdown(int? selectedEmployeeId = null)
        {
            var employees = await GetEmployeesAsync();

            if (employees == null || !employees.Any())
            {
                Console.WriteLine("No employees found for dropdown.");
            }
            else
            {
                Console.WriteLine($"Loaded {employees.Count} employees for dropdown.");
            }

            ViewBag.EmployeeID = new SelectList(employees, "EmployeeID", "FullName", selectedEmployeeId);
        }


        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            try
            {
                // Verifică rolul utilizatorului
                if (User.IsInRole("Manager"))
                {
                    // Managerul vede toate cererile
                    var response = await _httpClient.GetAsync(_baseUrl);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var attendances = JsonConvert.DeserializeObject<List<Attendance>>(json);

                    return View(attendances);
                }
                else
                {
                    // Utilizatorul obișnuit vede doar cererile sale
                    var email = User.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                    {
                        ModelState.AddModelError("", "Nu s-a găsit email-ul utilizatorului autentificat.");
                        return View(new List<Attendance>());
                    }

                    // Obține cererile utilizatorului curent
                    var response = await _httpClient.GetAsync($"{_baseUrl}/user/{email}");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var attendances = JsonConvert.DeserializeObject<List<Attendance>>(json);

                    return View(attendances);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"A apărut o eroare: {ex.Message}");
                return View(new List<Attendance>());
            }
        }


        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var json = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<Attendance>(json);

                return View(attendance);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Attendances/Create
        /* public async Task<IActionResult> Create()
         {
             await PopulateEmployeesDropdown();
             return View();
         }
 */
        public async Task<IActionResult> Create()
        {
            await PopulateEmployeesDropdown();
            Console.WriteLine($"Employees in dropdown: {ViewBag.Employees}");
            return View();
        }


        // POST: Attendances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceID,EmployeeID,Date,CheckInTime,CheckOutTime,HoursWorked")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(attendance);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(_baseUrl, content);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            await PopulateEmployeesDropdown(attendance.EmployeeID);
            return View(attendance);
        }

        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var json = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<Attendance>(json);

                await PopulateEmployeesDropdown(attendance.EmployeeID);
                return View(attendance);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Attendances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceID,EmployeeID,Date,CheckInTime,CheckOutTime,HoursWorked")] Attendance attendance)
        {
            if (id != attendance.AttendanceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(attendance);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            await PopulateEmployeesDropdown(attendance.EmployeeID);
            return View(attendance);
        }

        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var json = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<Attendance>(json);

                return View(attendance);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction("Delete", new { id });
            }
        }

        public async Task<IActionResult> Manage()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var attendances = JsonConvert.DeserializeObject<List<Attendance>>(json);

                return View(attendances);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(new List<Attendance>());
            }
        }


    }
}
