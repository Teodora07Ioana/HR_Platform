using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HR_Platform.Data;
using System.Runtime.ConstrainedExecution;
using HR_Platform.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);


 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .SetIsOriginAllowed(origin => true); // Permite toate originile (doar pentru testare)
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HRManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Parolă
    options.Password.RequireDigit = true; // Parola trebuie să conțină cel puțin o cifră
    options.Password.RequiredLength = 10; // Lungimea minimă a parolei este 10 caractere
    options.Password.RequireNonAlphanumeric = true; // Parola trebuie să conțină cel puțin un caracter special (!, @, # etc.)
    options.Password.RequireUppercase = true; // Parola trebuie să conțină cel puțin o literă mare
    options.Password.RequireLowercase = true; // Parola trebuie să conțină cel puțin o literă mică
    options.Password.RequiredUniqueChars = 5; // Parola trebuie să conțină cel puțin 5 caractere unice

    // Blocare cont
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Blocare de 5 minute
    options.Lockout.MaxFailedAccessAttempts = 3; // Maxim 3 încercări nereușite
    options.Lockout.AllowedForNewUsers = true; // Se aplică și utilizatorilor noi
    Console.WriteLine($"Lockout TimeSpan: {options.Lockout.DefaultLockoutTimeSpan}, Max Attempts: {options.Lockout.MaxFailedAccessAttempts}");

    // Confirmare email
    options.SignIn.RequireConfirmedAccount = true; // Confirmare email necesară
})
.AddRoles<IdentityRole>() // Adăugăm suport pentru roluri
.AddEntityFrameworkStores<IdentityContext>();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
});





builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddHttpClient();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Expirare după 30 de minute de inactivitate
    options.SlidingExpiration = true; // Reînnoire automată a sesiunii dacă utilizatorul este activ
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;

});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapHub<NotificationHub>("/notifications");

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



/*app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub"); // Exact cum este scris în frontend
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
*/
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});
*/

app.MapRazorPages();

app.Run();
