using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.Areas.Reportes;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.General;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Reportes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Reportes
{
    public class DbReportesGeneral: IDbReportesGeneral
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
       
       
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion


        public DbReportesGeneral(IConfiguration iConfiguration, ILogger<IDbIrisp1> logger)
        {
            _iConfiguration = iConfiguration;
            
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;

        }

        public async Task<DtoResultado<List<DtoAnio>>> F_GetAniosIrisP1()
        {
            var resultado = new DtoResultado<List<DtoAnio>>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
             
                parametros.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await conexion.QueryAsync<DtoAnio>(
                    "PK_CONSULTA_IRISP.F_GetAniosIrisP1",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resultado.IdRespuesta = 1;
                resultado.Data = lista;
                resultado.Mensaje = "Consulta realizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en F_GetAniosIrisP1");
                resultado.IdRespuesta = 0;
                resultado.Mensaje = "No es posible completar la consulta";
            }

            return resultado;
        }

        public async Task<DtoResultado<List<DtoGeneralIrisp>>> F_GetReporteGeneral(string roles, Int32? unidad, int anio)
        {
            var resultado = new DtoResultado<List<DtoGeneralIrisp>>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                parametros.Add("P_Anio", anio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_Roles", roles, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CodigoUnidad", unidad, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CURSOR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);


                var lista = (await conexion.QueryAsync<DtoGeneralIrisp>(
                    "PK_REPORTES_IRIS.P_GetReporteGeneral",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resultado.IdRespuesta = 1;
                resultado.Data = lista;
                resultado.Mensaje = "Consulta realizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en F_GetReporteGeneral");
                resultado.IdRespuesta = 0;
                resultado.Mensaje = "No es posible completar la consulta";
            }

            return resultado;
        }



    }
}
