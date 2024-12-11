using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HR_Platform.Areas.Identity.Pages.Account
{
    public class UserRoleService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(UserManager<IdentityUser> userManager, ILogger<UserRoleService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task AssignDefaultRoleAsync(IdentityUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                var result = await _userManager.AddToRoleAsync(user, "User");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role 'User' assigned to {user.Email}");
                }
                else
                {
                    _logger.LogError($"Failed to assign role 'User' to {user.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
    }
