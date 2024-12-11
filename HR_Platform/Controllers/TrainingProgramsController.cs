using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HR_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using HR_Platform.Hubs;


namespace HR_Platform.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class TrainingProgramsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly string _baseUrl = "https://localhost:7109/api/TrainingPrograms";
        private readonly string _employeeApiUrl = "https://localhost:7109/api/Employees";

        public TrainingProgramsController(HttpClient httpClient, IHubContext<NotificationHub> hubContext)
        {
            _httpClient = httpClient;
            _hubContext = hubContext;
        }

        // GET: TrainingPrograms
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["StartDateSortParm"] = sortOrder == "start_date" ? "start_date_desc" : "start_date";
            ViewData["CurrentFilter"] = searchString;

            try
            {
                Console.WriteLine("Fetching training programs...");
                // Build API request URL with query parameters
                var requestUrl = _baseUrl;

                if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(sortOrder))
                {
                    requestUrl += $"?searchString={searchString}&sortOrder={sortOrder}";
                }

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(json);

                return View(trainingPrograms);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(new List<TrainingProgram>());
            }
        }


        // GET: TrainingPrograms/Details/5
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
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(json);

                return View(trainingProgram);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TrainingPrograms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingProgramID,ProgramName,Description,StartDate,EndDate")] TrainingProgram trainingProgram)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(trainingProgram);
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

            return View(trainingProgram);
        }

        // GET: TrainingPrograms/Edit/5
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
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(json);

                return View(trainingProgram);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingProgramID,ProgramName,Description,StartDate,EndDate")] TrainingProgram trainingProgram)
        {
            if (id != trainingProgram.TrainingProgramID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(trainingProgram);
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

            return View(trainingProgram);
        }

        // GET: TrainingPrograms/Delete/5
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
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(json);

                return View(trainingProgram);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TrainingPrograms/Delete/5
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
        /*[HttpPost]
        public async Task<IActionResult> Enroll(int trainingId)
        {
            var userEmail = User.Identity?.Name;

            // Obține angajatul curent
            var response = await _httpClient.GetAsync(_employeeApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Nu s-a putut obține lista angajaților.");
                return RedirectToAction("Index");
            }

            var employeesJson = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesJson);
            var currentEmployee = employees.FirstOrDefault(e => e.Email == userEmail);

            if (currentEmployee == null)
            {
                ModelState.AddModelError("", "Angajatul nu a fost găsit.");
                return RedirectToAction("Index");
            }

            // Creează asocierea între angajat și training
            var enrollment = new EmployeeTraining
            {
                EmployeeID = currentEmployee.EmployeeID,
                TrainingProgramID = trainingId
            };

            var json = JsonConvert.SerializeObject(enrollment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var enrollResponse = await _httpClient.PostAsync($"{_baseUrl}/Enroll", content);
            if (enrollResponse.IsSuccessStatusCode)
            {
                // Trimite notificare prin SignalR
                await _hubContext.Clients.User(userEmail).SendAsync("ReceiveNotification", "Te-ai înscris cu succes la curs!");
            }
            else
            {
                ModelState.AddModelError("", "Înscrierea nu a reușit.");
            }

            return RedirectToAction("Index");
        }*/

    }
}
