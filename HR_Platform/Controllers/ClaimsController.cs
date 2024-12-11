using HR_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HR_Platform.Controllers
{
    //[Authorize(Policy = "AdminPolicy")] // Numai Admin poate atribui roluri
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // RoleManager necesar pentru verificarea și crearea rolurilor
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ClaimsController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // View pentru atribuire roluri
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string email, string role)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                // Verificăm dacă rolul există
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    ModelState.AddModelError("", "Role does not exist.");
                    return View();
                }

                // Adăugăm rolul utilizatorului
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role assigned successfully.";
                    _logger.LogInformation($"Role '{role}' assigned to user '{email}'.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignRole");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> UserRoles()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userRoles);
        }
    }

    public class UserRoleViewModel
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
