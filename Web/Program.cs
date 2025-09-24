using Comun.Areas.Admin;
using Comun.General;
using Microsoft.AspNetCore.Authentication.Cookies;
using Negocio.Gestion.Admin;
using Negocio.Gestion.Clientes;
using Negocio.Gestion.General;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Modulo1;
using Serilog;
using Servicios.Api;
using Servicios.ApiInterfaz;
using Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/InicioSesion";
        options.AccessDeniedPath = "/Cuenta/CerrarSesion";
        options.LogoutPath = "/Cuenta/CerrarSesion";
        options.Cookie.Name = "Web";
        options.Cookie.HttpOnly = true;
        //vencimiento
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.MaxAge = options.ExpireTimeSpan;
        options.SlidingExpiration = true;
        // ReturnUrlParameter requires 
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    });

//cadena conexión
builder.Services.AddHttpClient();

// Proxies api gateway 
builder.Services.AddSingleton(new ApiGatewayUrl(builder.Configuration.GetValue<string>("ApiGatewayUrl")));

//configuracion log de mensajes
var logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.Enrich.FromLogContext()
.CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddHttpContextAccessor();

// Seguridad
builder.Services.AddScoped<IGestionToken, GestionToken>();
builder.Services.AddScoped<IGestionOUD, GestionOUD>();

//Áreas
builder.Services.AddScoped<IDbAdministracion, DbAdministracion>();
builder.Services.AddScoped<IDbIrisp1, DbIrisp1>();
builder.Services.AddScoped<IDbFuncionarios, DbFuncionarios>();
builder.Services.AddScoped<IDbDominios, DbDominios>();
builder.Services.AddScoped<IDbClientes, DbClientes>();
builder.Services.AddScoped<IUnidades, Unidades>();
builder.Services.AddScoped<IDbSeguimientoIris, DbSeguimientoIris>();

// httpClient
builder.Services.AddHttpClient<IApiWebOud, ApiWebOud>();
builder.Services.AddHttpClient<IApiWebToken, ApiWebToken>();

//Variables de Sesión
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//AppSettings
builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


var RutaVisualizador = builder.Configuration.GetValue<string>("Visualizador");

var app = builder.Build();

//Variables de Sesion
app.UseSession();

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
app.UseAuthentication();
app.UseAuthorization();

//configuración de area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=InicioSesion}/{id?}");


app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value;
    var HayError = url.Contains("Error");

    if (!HayError)
    {
        var obj = context.Session.GetObject<List<DtoMenu>>("ListaMenu");
        var ipMaquina = context.Session.GetString("IpMaquina");

        if ((!url.Contains("Cuenta")) && url.Length > 5)
        {
            if (obj == null || string.IsNullOrEmpty(ipMaquina))
            {
                context.Response.Redirect($"{context.Request.Scheme}://{context.Request.Host.Value}/Cuenta/CerrarSesion");
                return;
            }
        }
    }
    await next();
});

app.Run();

