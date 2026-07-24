using Comun.Areas.Reportes;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public class DbReporteVerificacion: IDbReporteVerificacion
    {

        #region Propiedades
        private readonly IConfiguration _iConfiguration;


        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion


        public DbReporteVerificacion(IConfiguration iConfiguration, ILogger<IDbIrisp1> logger)
        {

            _iConfiguration = iConfiguration;

            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;

        }

        public async Task<DtoResultado<List<DtoReporteVerificacion>>> F_GetReporteVerificacion(int? Anio, string RolesUsuario, long CodigoUnidad)
        {
            var resultado = new DtoResultado<List<DtoReporteVerificacion>>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // ESTE ES EL CORRECTO PARA FUNCIONES
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Anio", Anio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_Roles", RolesUsuario ?? string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                var lista = (await conexion.QueryAsync<DtoReporteVerificacion>(
                    "PK_REPORTES_IRIS.F_GetReporteVerificacion",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resultado.IdRespuesta = 1;
                resultado.Data = lista;
                resultado.Mensaje = "Consulta realizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en F_GetReporteVerificacion");
                resultado.IdRespuesta = 0;
                resultado.Mensaje = "Error al consultar el reporte.";
            }

            return resultado;
        }





    }






}
