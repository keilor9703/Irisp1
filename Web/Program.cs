using Comun.Areas.Admin;
using Comun.Areas.Reportes;
using Comun.General;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Options;
using Negocio.Gestion.Admin;
using Negocio.Gestion.Control;
using Negocio.Gestion.Expendios;
using Negocio.Gestion.General;
using Negocio.Gestion.Integrantes;
using Negocio.Gestion.Irisp1;
using Negocio.Gestion.Reportes;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Control;
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

// Librería MFA
using Ponal.Seguridad.MfaCliente.Extensions;
using Ponal.Seguridad.MfaCliente.Interfaces; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

// Agregamos controladores de la librería
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(Ponal.Seguridad.MfaCliente.Controllers.MfaOrquestadorController).Assembly);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/InicioSesion";
        options.AccessDeniedPath = "/Cuenta/CerrarSesion";
        options.LogoutPath = "/Cuenta/CerrarSesion";
        options.Cookie.Name = "Web";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.MaxAge = options.ExpireTimeSpan;
        options.SlidingExpiration = true;
    });

DefaultTypeMap.MatchNamesWithUnderscores = true;
QuestPDF.Settings.License = LicenseType.Community;

// Anti-CSRF: el token puede llegar por header (lo adjunta el _Layout vía ajaxSend)
// además del campo de formulario. Los controladores propios lo exigen con
// [AutoValidateAntiforgeryToken]; la librería MFA externa no se ve afectada.
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.AddHttpClient();

// Modelo de URLs del PIP (Comun.General.ApiGatewayUrl) usado por PipWebServices
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();

    var pipBase = cfg.GetValue<string>("ApiGatewayUrl")
                 ?? throw new InvalidOperationException("Falta ApiGatewayUrl en configuración.");

    var mfaBase = cfg.GetValue<string>("MfaCentral:BaseUrl") ?? "";

    // Retornamos el modelo de Comun.General que usa PipWebServices
    return new ApiGatewayUrl(pipBase, mfaBase);
});

// Configuracion log de mensajes

// ...




// Configuración log de mensajes
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var dpConn = builder.Configuration.GetConnectionString("strConexionIris_Disec");

builder.Services.AddDataProtection()
    .SetApplicationName("IRIS-P1");

builder.Services.AddHttpContextAccessor();

// Seguridad
builder.Services.AddScoped<IDbConsultasPIP, DbConsultasPIP>();
builder.Services.AddScoped<IPipWebServices, PipWebServices>();

// Áreas
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
builder.Services.AddScoped<IDbControlGestion, DbControlGestion>();

// Inyección de la librería MFA (registra sus propias URLs). El MFA real se atiende por
// IDbMfaCentralWs (proyecto Ponal.Seguridad.MfaCliente); el antiguo IMfaTotpService quedó
// huérfano (registrado pero sin inyectarse en ningún lado) y se retiró.
builder.Services.AddMemoryCache();
builder.Services.AddPonalMfa(builder.Configuration);

// Salvaguarda de configuración: si MFA está habilitado pero su BaseUrl apunta a un entorno
// local, todos los inicios de sesión fallarían. Se advierte fuerte en el log de arranque.
{
    var mfaEnabled = builder.Configuration.GetValue<bool>("MfaCentral:Enabled");
    var mfaUrl = builder.Configuration.GetValue<string>("MfaCentral:BaseUrl") ?? "";
    if (mfaEnabled && (mfaUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                       || mfaUrl.Contains("127.0.0.1")))
    {
        logger.Warning("MFA está habilitado (MfaCentral:Enabled=true) pero MfaCentral:BaseUrl apunta a un entorno local ({MfaUrl}). Los inicios de sesión con MFA fallarán hasta corregir la URL.", mfaUrl);
    }
}

// httpClient
builder.Services.AddHttpClient<IPipWebServices, PipWebServices>();

// Variables de Sesión
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".IRISP1.Session";
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// AppSettings
builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<CredencialesPipOptions>(builder.Configuration.GetSection("CredencialesPip"));

var RutaVisualizador = builder.Configuration.GetValue<string>("Visualizador");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value ?? "";
    var hayError = url.Contains("Error", StringComparison.OrdinalIgnoreCase);

    if (!hayError)
    {
        var obj = context.Session.GetObject<List<DtoMenu>>("ListaMenu");
        var ipMaquina = context.Session.GetString("IpMaquina");

        bool esRutaMfa = url.Contains("/Cuenta/Mfa", StringComparison.OrdinalIgnoreCase) ||
                         url.Contains("/Mfa", StringComparison.OrdinalIgnoreCase);

        bool esRutaCuenta = url.Contains("/Cuenta", StringComparison.OrdinalIgnoreCase);

        if (!esRutaCuenta && !esRutaMfa && url.Length > 1)
        {
            if (obj == null || string.IsNullOrEmpty(ipMaquina))
            {
                context.Response.Redirect($"{context.Request.Scheme}://{context.Request.Host}/Cuenta/CerrarSesion");
                return;
            }
        }
    }
    await next();
});

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=InicioSesion}/{id?}");

app.Run();