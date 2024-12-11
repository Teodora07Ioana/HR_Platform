using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using HR_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HR_Platform.Hubs;


namespace HR_Platform.Controllers
{


    public class LeaveRequestsController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7109/api/LeaveRequests";
        private readonly string _employeeApiUrl = "https://localhost:7109/api/Employees";

        public LeaveRequestsController(HttpClient httpClient, IHubContext<NotificationHub> hubContext)
        {
            _httpClient = httpClient;
            _hubContext = hubContext;
        }

        private async Task<List<Employee>> GetEmployeesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_employeeApiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(json);

                return employees ?? new List<Employee>();
            }
            catch
            {
                return new List<Employee>();
            }
        }

        private async Task PopulateEmployeesDropdown(int? selectedEmployeeId = null)
        {
            var employees = await GetEmployeesAsync();
            ViewBag.EmployeeID = new SelectList(employees, "EmployeeID", "FullName", selectedEmployeeId);
        }

        // GET: LeaveRequests

        public async Task<IActionResult> Index()
        {

            Console.WriteLine($"User: {User.Identity.Name}, IsManager: {User.IsInRole("Manager")}");
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var leaveRequests = JsonConvert.DeserializeObject<List<LeaveRequest>>(json);




                // Filtrare după utilizatorul logat (dacă este angajat)
                if (User.IsInRole("User"))
                {
                    var userEmail = User.Identity?.Name;
                    var employees = await GetEmployeesAsync();
                    var currentEmployee = employees.FirstOrDefault(e => e.Email == userEmail);
                    leaveRequests = leaveRequests.Where(lr => lr.EmployeeID == currentEmployee?.EmployeeID).ToList();
                }

                return View(leaveRequests);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(new List<LeaveRequest>());
            }
        }





        // GET: LeaveRequests/Details/5
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
                var leaveRequest = JsonConvert.DeserializeObject<LeaveRequest>(json);

                return View(leaveRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LeaveRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateEmployeesDropdown();
            return View();
        }

        // POST: LeaveRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveRequestID,StartDate,EndDate,Reason")] LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obține email-ul utilizatorului autentificat
                    var userEmail = User.Identity?.Name;

                    // Căutare angajat pe baza email-ului utilizatorului
                    var employees = await GetEmployeesAsync(); // Apelează API-ul pentru a obține lista angajaților
                    var currentEmployee = employees.FirstOrDefault(e => e.Email == userEmail);

                    if (currentEmployee == null)
                    {
                        ModelState.AddModelError("", "Nu s-a găsit angajatul pentru utilizatorul logat.");
                        return View(leaveRequest);
                    }

                    // Asociere EmployeeID și setare status implicit
                    leaveRequest.EmployeeID = currentEmployee.EmployeeID;
                    leaveRequest.Status = "Neaprobat";

                    // Trimitere cerere către API
                    var json = JsonConvert.SerializeObject(leaveRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(_baseUrl, content);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"A apărut o eroare: {ex.Message}");
                }
            }

            return View(leaveRequest);
        }



        /// GET: LeaveRequests/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var leaveRequest = JsonConvert.DeserializeObject<LeaveRequest>(await response.Content.ReadAsStringAsync());
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }




        // POST: LeaveRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StartDate,EndDate,Reason")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveRequestID)
            {
                return NotFound();
            }

            try
            {
                // Obține cererea existentă din API
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var existingRequestJson = await response.Content.ReadAsStringAsync();
                var existingRequest = JsonConvert.DeserializeObject<LeaveRequest>(existingRequestJson);

                if (existingRequest == null)
                {
                    return NotFound();
                }

                // Actualizează doar câmpurile permise
                existingRequest.StartDate = leaveRequest.StartDate;
                existingRequest.EndDate = leaveRequest.EndDate;
                existingRequest.Reason = leaveRequest.Reason;

                // Trimite cererea modificată către API
                var json = JsonConvert.SerializeObject(existingRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var putResponse = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                putResponse.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(leaveRequest);
            }
        }



        // GET: LeaveRequests/Delete/5
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
                var leaveRequest = JsonConvert.DeserializeObject<LeaveRequest>(json);

                return View(leaveRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: LeaveRequests/Delete/5
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

         [Authorize(Policy = "ManagerPolicy")]
             public async Task<IActionResult> Approve(int id)
             {
                 try
                 {
                     var response = await _httpClient.PutAsync($"{_baseUrl}/Approve/{id}", null);
                     if (!response.IsSuccessStatusCode) return NotFound();

                     TempData["Message"] = "Leave request approved successfully.";
                     return RedirectToAction(nameof(Index));
                 }
                 catch (Exception ex)
                 {
                     ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                     return RedirectToAction(nameof(Index));
                 }
             }
      /*  [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> Approve(int id)
        {
            var response = await _httpClient.PutAsync($"{_baseUrl}/Approve/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                // Obține detaliile cererii de concediu aprobate
                var leaveRequestResponse = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (leaveRequestResponse.IsSuccessStatusCode)
                {
                    var request = JsonConvert.DeserializeObject<LeaveRequest>(await leaveRequestResponse.Content.ReadAsStringAsync());

                    // Obține detalii despre angajat
                    var employees = await GetEmployeesAsync();
                    var currentEmployee = employees.FirstOrDefault(e => e.EmployeeID == request.EmployeeID);

                    // Trimite notificare angajatului prin SignalR
                    if (currentEmployee != null)
                    {
                        await _hubContext.Clients.User(currentEmployee.Email)
                            .SendAsync("ReceiveNotification", "Cererea ta de concediu a fost aprobată.");
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }*/


        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{_baseUrl}/Reject/{id}", null);
                if (!response.IsSuccessStatusCode) return NotFound();

                TempData["Message"] = "Leave request rejected successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }




    }
}