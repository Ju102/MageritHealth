using MageritHealth.Data;
using MageritHealth.Repositories;
using MageritHealth.Repositories.Interfaces;
using MageritHealth.Services;
using MageritHealth.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios (Dependency Injection Configuration / Configuración de la inyección de dependencias) //

builder.Services.AddAntiforgery();

string connectionString = builder.Configuration.GetConnectionString("MageritHealthConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'MageritHealthConnection'.");

builder.Services.AddDbContext<MageritHealthDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IUsuariosRepository, UsuariosRepository>(); // usuarios, especialidades y credenciales
builder.Services.AddTransient<ICitasRepository, CitasRepository>(); // citas

builder.Services.AddTransient<IPrescripcionesRepository, PrescripcionesRepository>(); // prescripciones y medicamentos

builder.Services.AddTransient<IAnaliticasRepository, AnaliticasRepository>(); // analiticas, mediciones y tipos_mediciones

builder.Services.AddTransient<IInfoClinicaRepository, InfoClinicaRepository>(); // info_clinica y antecedentes

builder.Services.AddTransient<IEmailingService, EmailingService>();
builder.Services.AddScoped<IExportService, ExportService>();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DoctorOnly", policy => policy.RequireRole("doctor"));
    options.AddPolicy("PacienteOnly", policy => policy.RequireRole("paciente"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
{
    config.AccessDeniedPath = "/Account/AccesoDenegado";
});

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();

// Middleware (HTTP Request Pipeline Configuration / Configuración de la tubería de solicitudes HTTP) //

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
// app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// app.MapStaticAssets();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Account}/{action=Login}/{id?}");
});

//app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}")
//    .WithStaticAssets();

app.Run();
