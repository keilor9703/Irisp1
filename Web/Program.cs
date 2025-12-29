using Comun.Areas.Admin;
using Comun.Areas.Reportes;
using Comun.General;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Negocio.Gestion.Admin;

using Negocio.Gestion.Expendios;
using Negocio.Gestion.General;
using Negocio.Gestion.Integrantes;
using Negocio.Gestion.Irisp1;
using Negocio.Gestion.Reportes;
using Negocio.Interfaz.Admin;

using Negocio.Interfaz.Expendios;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Integrantes;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Reportes;
using QuestPDF.Infrastructure;
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


DefaultTypeMap.MatchNamesWithUnderscores = true;

QuestPDF.Settings.License = LicenseType.Community;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



builder.Services.AddHttpContextAccessor();

// Seguridad
builder.Services.AddScoped<IDbConsultasPIP, DbConsultasPIP>();
//builder.Services.AddScoped<IGestionOUD, GestionOUD>();
builder.Services.AddScoped<IPipWebServices, PipWebServices>();

//Áreas
builder.Services.AddScoped<IDbAdministracion, DbAdministracion>();
builder.Services.AddScoped<IDbIrisp1, DbIrisp1>();
builder.Services.AddScoped<IDbFuncionarios, DbFuncionarios>();
builder.Services.AddScoped<IDbDominios, DbDominios>();
builder.Services.AddScoped<IDbVerificacionIris, DbVerificacionIris>();
builder.Services.AddScoped<IDbSeguimientoIris, DbSeguimientoIris>();
builder.Services.AddScoped<IDbRegistroExpendio, DbRegistroExpendio>();
builder.Services.AddScoped<IDbRegistroInteg, DbRegistroInteg>();
builder.Services.AddScoped<IDbBuscarIntegrantes, DbBuscarIntegrantes>();
builder.Services.AddScoped<IDbReportesGeneral, DbReportesGeneral>();
builder.Services.AddScoped<IDbReporteVerificacion, DbReporteVerificacion>();


// httpClient
builder.Services.AddHttpClient<IPipWebServices, PipWebServices>();
//builder.Services.AddHttpClient<IApiWebToken, ApiWebToken>();
//builder.Services.AddHttpClient<IApiWebFuncionariosIdPIP, ApiWebFuncionariosIdPIP>();

//Variables de Sesión
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//AppSettings
builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<CredencialesPipOptions>(builder.Configuration.GetSection("CredencialesPip"));




var RutaVisualizador = builder.Configuration.GetValue<string>("Visualizador");

var app = builder.Build();

app.UseCors("AngularPolicy");


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


//app.Use(async (context, next) =>
//{
//    var url = context.Request.Path.Value;
//    var HayError = url.Contains("Error");

//    if (!HayError)
//    {
//        var obj = context.Session.GetObject<List<DtoMenu>>("ListaMenu");
//        var ipMaquina = context.Session.GetString("IpMaquina");

//        if ((!url.Contains("Cuenta")) && url.Length > 5)
//        {
//            if (obj == null || string.IsNullOrEmpty(ipMaquina))
//            {
//                context.Response.Redirect($"{context.Request.Scheme}://{context.Request.Host.Value}/Cuenta/CerrarSesion");
//                return;
//            }
//        }
//    }
//    await next();
//});


app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Ignorar estáticos / cuenta / error
    if (path.StartsWith("/Cuenta", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/img") ||
        path.Contains("Error", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    // Solo aplicar si el usuario está autenticado (si no, que cookie auth lo mande al LoginPath)
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var menu = context.Session.GetObject<List<DtoMenu>>("ListaMenu");
        var ipMaquina = context.Session.GetString("IpMaquina");

        // Si se perdió session, NO cierres sesión: redirige a una ruta de "SesionExpirada"
        if (menu == null || string.IsNullOrEmpty(ipMaquina))
        {
            context.Response.Redirect("/Cuenta/CerrarSesion");
            return;
        }
    }

    await next();
});


app.Run();

