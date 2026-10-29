using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comun.Areas.Admin;
using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IDbAuditoriaUso
    {
        Task<DtoResultado<DtoAudResumen>> F_GetResumen(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario);
        Task<DtoResultado<List<DtoAudPorModulo>>> F_GetPorModulo(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario);
        Task<DtoResultado<List<DtoAudPorFecha>>> F_GetPorFecha(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario);
        Task<DtoResultado<List<DtoAudPorUnidad>>> F_GetPorUnidad(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario);
        Task<DtoResultado<List<DtoAudPorUsuario>>> F_GetPorUsuario(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario);
    }
}
