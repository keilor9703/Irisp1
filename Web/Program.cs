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

        // 🔒 Recomendado para SSO/MFA en subdominios o flujos externos:
        options.Cookie.SameSite = SameSiteMode.None;   // <-- ANTES: Lax
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // <-- ANTES: SameAsRequest


        //options.Cookie.SameSite = SameSiteMode.Lax;
        //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;


        // (Opcional, si usas subdominios)
        // options.Cookie.Domain = ".policia.gov.co";

        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.MaxAge = options.ExpireTimeSpan;
        options.SlidingExpiration = true;
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


//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\IRISP1\DataProtectionKeys"))
//    .SetApplicationName("IRIS-P1");



var dpConn = builder.Configuration.GetConnectionString("strConexionIris_Disec");

builder.Services.AddSingleton<IXmlRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<OracleXmlRepository>>();
    return new OracleXmlRepository(dpConn, logger);
});


builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(sp =>
    new ConfigureOptions<KeyManagementOptions>(options =>
    {
        options.XmlRepository = sp.GetRequiredService<IXmlRepository>();
    })
);

builder.Services.AddDataProtection()
    .SetApplicationName("IRIS-P1");


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
builder.Services.AddScoped<IMfaTotpService, MfaTotpService>();
builder.Services.AddScoped<IDbMfaIris, DbMfaIris>();




// httpClient
builder.Services.AddHttpClient<IPipWebServices, PipWebServices>();
//builder.Services.AddHttpClient<IApiWebToken, ApiWebToken>();
//builder.Services.AddHttpClient<IApiWebFuncionariosIdPIP, ApiWebFuncionariosIdPIP>();

//Variables de Sesión
builder.Services.AddMvc();


builder.Services.AddDistributedMemoryCache(); // 

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".IRISP1.Session";
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;

    // ✅ CORRECTO PARA LOCALHOST
    //options.Cookie.SameSite = SameSiteMode.Lax;
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

});





//AppSettings
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

// ✅ Session DEBE ir aquí (después de Routing y antes de Auth)
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Tu middleware de validación de sesión debe ir DESPUÉS de UseSession
app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value ?? "";
    var hayError = url.Contains("Error", StringComparison.OrdinalIgnoreCase);

    if (!hayError)
    {
        var obj = context.Session.GetObject<List<DtoMenu>>("ListaMenu");
        var ipMaquina = context.Session.GetString("IpMaquina");

        bool esRutaMfa =
            url.Contains("/Cuenta/Mfa", StringComparison.OrdinalIgnoreCase) ||
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

// ✅ Endpoints al final
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=InicioSesion}/{id?}");

app.Run();


