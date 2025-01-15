using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvclaptop.Repositories;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
using MvcLaptop.Utils.ConfigOptions.VNPay;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

int optionDatabases = 1;
switch (optionDatabases)
{
    case 1:
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<MvcLaptopContext>(options =>
                options.UseSqlite(connectionString));
        }
        break;
    case 2:
        {
            var connectionString = builder.Configuration.GetConnectionString("SQLServerConnection") ?? throw new InvalidOperationException("Connection string 'SQLServerConnection' not found.");
            builder.Services.AddDbContext<MvcLaptopContext>(options =>
                options.UseSqlServer(connectionString));
        }
        break;
}
RouteRazerPage();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();  // Sử dụng bộ nhớ để lưu trữ session
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;  // Cookie chỉ có thể truy cập từ server
    options.Cookie.IsEssential = true;  // Làm cho cookie session là cần thiết
});
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<Role>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<MvcLaptopContext>();
AddScoped();
// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MvcLaptopContext>();
    db.Database.Migrate();
    var serviceProvider = scope.ServiceProvider;
    try
    {
        Log.Information("Seeding data...");
        var dbInitializer = serviceProvider.GetService<DbInitializer>();
        if (dbInitializer != null)
            dbInitializer.Seed()
                         .Wait();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

void AddScoped()
{
    builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();

    builder.Services.AddTransient<DbInitializer>();
    builder.Services.AddTransient<UnitOfWork>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
    builder.Services.AddScoped<IFunctionRepository, FunctionRepository>();
    builder.Services.AddScoped<ILaptopService, LaptopService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddTransient<IVNPayService, VNPayService>();
    builder.Services.Configure<VNPayConfigOptions>(builder.Configuration.GetSection("VnPay"));
}
void RouteRazerPage()
{
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });
    builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "login");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "logout");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/AccessDenied", "access-denied");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "register");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "forgot-password");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/ResetPassword", "reset-password");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/ConfirmEmail", "confirm-email");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/ResetPasswordConfirmation", "reset-password-confirmation");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/LogoutConfirmation", "logout-confirmation");

        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/ChangePassword", "manager/change-password");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/DeletePersonalData", "manager/delete-personal-data");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Disable2fa", "manager/disable2fa");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/DownloadPersonalData", "manager/download-personal-data");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Email", "manager/email");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/EnableAuthenticator", "manager/enable-authenticator");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/ExternalLogins", "manager/external-logins");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/GenerateRecoveryCodes", "manager/generate-recovery-codes");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Index", "manager");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/PersonalData", "manager/personal-data");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/ResetAuthenticator", "manager/reset-authenticator");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/SetPassword", "manager/set-password");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/ShowRecoveryCodes", "manager/show-recovery-codes");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/TwoFactorAuthentication", "manager/two-factor-authentication");
    });
}
