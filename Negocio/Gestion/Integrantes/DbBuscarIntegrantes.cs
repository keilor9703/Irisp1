using Comun.Areas.Integrantes;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Integrantes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Integrantes
{
    public class DbBuscarIntegrantes: IDbBuscarIntegrantes
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;



        public DbBuscarIntegrantes(IConfiguration iConfiguration, ILogger<IDbRegistroInteg> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }


        public async Task<DtoResultado<List<DtoDatosBasicos>>> F_GetIntegrantesPorId(long identificacion)
        {
            var resultado = new DtoResultado<List<DtoDatosBasicos>>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_IDENTIFICACION", identificacion, OracleMappingType.Long, ParameterDirection.Input);
                parametros.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await conexion.QueryAsync<DtoDatosBasicos>(
                    "PK_INTEGRANTES_IRIS.F_GetIntegrantesPorId",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resultado.IdRespuesta = 1;
                resultado.Data = lista;
                resultado.Mensaje = "Consulta realizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en F_GetIntegrantesPorId");

                resultado.IdRespuesta = 0;
                resultado.Mensaje = "No es posible completar la consulta";
            }

            return resultado;
        }


    }
}
