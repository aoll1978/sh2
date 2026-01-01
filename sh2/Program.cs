using Microsoft.EntityFrameworkCore;
using sh2.Data;
using Microsoft.AspNetCore.Identity;
using sh2.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Register the application's database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Configure Identity with roles and custom settings
builder.Services.AddIdentity<sh2.Models.User, sh2.Models.Role>(options =>
{
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
    // Password settings (minimum requirements, no strict validation)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Add built-in Identity UI

// Add Razor Pages and related services
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- START OF INITIALIZATION BLOCK ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Вызываем наш метод наполнения
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при наполнении базы данных (Seeding).");
    }
}
// --- END OF BLOCK ---


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
