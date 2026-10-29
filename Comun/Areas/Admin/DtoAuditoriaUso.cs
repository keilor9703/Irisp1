using System;
using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    // DTOs del Dashboard de Uso y Auditoría. Dapper mapea las columnas del cursor por nombre de
    // propiedad (MatchNamesWithUnderscores=true, p.ej. TOTAL_EVENTOS -> TotalEventos); el
    // [JsonPropertyName] en camelCase define cómo lo consume el JavaScript.

    public class DtoAudResumen
    {
        [JsonPropertyName("totalEventos")] public long TotalEventos { get; set; }
        [JsonPropertyName("usuariosActivos")] public long UsuariosActivos { get; set; }
        [JsonPropertyName("totalIngresos")] public long TotalIngresos { get; set; }
        [JsonPropertyName("modulosDistintos")] public long ModulosDistintos { get; set; }
        [JsonPropertyName("exportaciones")] public long Exportaciones { get; set; }
        [JsonPropertyName("descargas")] public long Descargas { get; set; }
    }

    public class DtoAudPorModulo
    {
        [JsonPropertyName("modulo")] public string? Modulo { get; set; }
        [JsonPropertyName("cantidad")] public long Cantidad { get; set; }
        [JsonPropertyName("usuarios")] public long Usuarios { get; set; }
    }

    public class DtoAudPorFecha
    {
        [JsonPropertyName("dia")] public DateTime? Dia { get; set; }
        [JsonPropertyName("totalEventos")] public long TotalEventos { get; set; }
        [JsonPropertyName("totalIngresos")] public long TotalIngresos { get; set; }
    }

    public class DtoAudPorUnidad
    {
        [JsonPropertyName("sigla")] public string? Sigla { get; set; }
        [JsonPropertyName("unidad")] public string? Unidad { get; set; }
        [JsonPropertyName("cantidad")] public long Cantidad { get; set; }
        [JsonPropertyName("usuarios")] public long Usuarios { get; set; }
    }

    public class DtoAudPorUsuario
    {
        [JsonPropertyName("identificacion")] public long Identificacion { get; set; }
        [JsonPropertyName("sigla")] public string? Sigla { get; set; }
        [JsonPropertyName("unidad")] public string? Unidad { get; set; }
        [JsonPropertyName("totalEventos")] public long TotalEventos { get; set; }
        [JsonPropertyName("totalIngresos")] public long TotalIngresos { get; set; }
        [JsonPropertyName("ultimoIngreso")] public DateTime? UltimoIngreso { get; set; }
        [JsonPropertyName("primerIngreso")] public DateTime? PrimerIngreso { get; set; }
        [JsonPropertyName("modulosDistintos")] public long ModulosDistintos { get; set; }
        [JsonPropertyName("ultimaActividad")] public DateTime? UltimaActividad { get; set; }
    }

    // Fila de la tabla de depuración de usuarios: se arma en el controlador cruzando F_GetUsuarios
    // (usuarios creados) con la actividad (F_GetPorUsuario). Incluye a quienes NUNCA ingresaron.
    public class DtoAudUsuarioDepuracion
    {
        [JsonPropertyName("identificacion")] public long Identificacion { get; set; }
        [JsonPropertyName("funcionario")] public string? Funcionario { get; set; }
        [JsonPropertyName("usuario")] public string? Usuario { get; set; }
        [JsonPropertyName("unidad")] public string? Unidad { get; set; }
        [JsonPropertyName("bloqueado")] public int Bloqueado { get; set; }
        [JsonPropertyName("totalIngresos")] public long TotalIngresos { get; set; }
        [JsonPropertyName("modulosDistintos")] public long ModulosDistintos { get; set; }
        [JsonPropertyName("ultimoIngreso")] public DateTime? UltimoIngreso { get; set; }
        [JsonPropertyName("diasSinIngresar")] public int? DiasSinIngresar { get; set; }
        [JsonPropertyName("estado")] public string? Estado { get; set; }
    }
}
