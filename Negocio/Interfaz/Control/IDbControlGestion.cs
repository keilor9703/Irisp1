using Comun.Areas.Control;
using Comun.General;

namespace Negocio.Interfaz.Control
{
    public interface IDbControlGestion
    {
        Task<DtoResultado<List<DtoSlaConfig>>> F_GetSlaConfig();

        Task<DtoResultado<int>> P_InsUpdSlaConfig(int idListaTareas, int tiempoAlertaHoras, int tiempoMaximoHoras,
            string? justificacion, long usuario, string maquina);

        Task<DtoResultado<int>> P_DelSlaConfig(string slaConfigId, long usuario, string maquina);

        Task<DtoResultado<List<DtoTareaControlGestion>>> F_GetTareasControlGestion(DateTime fechaInicio, DateTime fechaFin, int? regionCodigo, string? siglaUnidad, string rolesUsuario, long codigoUnidad);

        Task<DtoResultado<List<DtoCasoControlGestion>>> F_GetCasosControlGestion(DateTime fechaInicio, DateTime fechaFin, int? regionCodigo, string? siglaUnidad, string rolesUsuario, long codigoUnidad);

        Task<DtoResultado<List<DtoRegionIrisp1>>> F_GetRegionesIrisp1();

        Task<DtoResultado<List<DtoSiglaUnidad>>> F_GetSiglasUnidadIrisp1(int? regionCodigo);

        Task<DtoResultado<List<DtoMapaIrisp1>>> F_GetMapaIrisp1(DateTime fechaInicio, DateTime fechaFin, int? regionCodigo, string? siglaUnidad, string rolesUsuario, long codigoUnidad);

        Task<DtoResultado<List<DtoResultadoCasoIrisp1>>> F_GetResultadosCasosIrisp1(DateTime fechaInicio, DateTime fechaFin, int? regionCodigo, string? siglaUnidad, string rolesUsuario, long codigoUnidad);
    }
}
