using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HR_Platform.Models;
using Newtonsoft.Json;
using System.Text;

namespace HR_Platform.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7109/api/Departments";

        public DepartmentsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(json);

                return View(departments);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(new List<Department>());
            }
        }

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
                var department = JsonConvert.DeserializeObject<Department>(json);

                return View(department);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,DepartmentName")] Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(department);
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

            return View(department);
        }

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
                var department = JsonConvert.DeserializeObject<Department>(json);

                return View(department);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,DepartmentName")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(department);
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

            return View(department);
        }

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
                var department = JsonConvert.DeserializeObject<Department>(json);

                return View(department);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

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


    }

}
