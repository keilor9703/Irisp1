using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.General;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.General
{
    public class DbDominios : IDbDominios
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionDesa;
        private readonly ILogger _logger;
        private readonly string _strConexionIris_Disec;
        #endregion

        #region Constructor
        public DbDominios(IConfiguration iConfiguration,
                          ILogger<DbDominios> logger
                         )
        {
            _iConfiguration = iConfiguration;
            _strConexionDesa = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        #region Métodos para Consulta

        public async Task<DtoResultado<List<DtoDominios>>> F_GetDominiosIris(Int32 V_Id)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Id", V_Id, OracleMappingType.Int32, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PKS_ADMINISTRACION_IRIS.F_GetDominiosIris",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista != null && lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetDominiosIris";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando PKS_ADMINISTRACION_IRIS.F_GetDominiosIris");

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{ex.Message}";
                resp.Operacion = "0";
            }

            return resp;
        }



        public async Task<DtoResultado<List<DtoDominios>>> F_GetDominios(Int32 V_Id)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Id", V_Id, OracleMappingType.Int32, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PKS_ADMINISTRACION_IRIS.F_GetDominios",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista != null && lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetDominios";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando PKS_ADMINISTRACION_IRIS.F_GetDominios");

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{ex.Message}";
                resp.Operacion = "0";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoDominios>>> F_GetDepartamentos(Int32 V_Id)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionDesa);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Id", V_Id, OracleMappingType.Int32, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "pk_Clientes.F_GetDepartamentos",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetDepartamentos";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando pk_Clientes.F_GetDepartamentos");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoDominios>>> F_GetMunicipios(Int32 V_Id)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionDesa);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Id", V_Id, OracleMappingType.Int32, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "pk_Clientes.F_GetMunicipios",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetMunicipios";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando pk_Clientes.F_GetMunicipios");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }



        public async Task<DtoResultado<List<DtoDominios>>> F_GetDependencias(string V_SiglaPapa)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionDesa);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("v_unidad", V_SiglaPapa, OracleMappingType.Varchar2, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "pk_Clientes.F_GetDepedencias",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetDependencias";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando pk_Clientes.F_GetDepedencias");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPoliciales(Int32 V_Id)
        {
            var resp = new DtoResultado<List<DtoDominios>>();

            try
            {
                using var connection = new OracleConnection(_strConexionDesa);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                parametros.Add("P_Id", V_Id, OracleMappingType.Int32, ParameterDirection.Input);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "pk_Clientes.F_GetUnidadesPoliciales",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (lista.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetUnidadesPoliciales";
                    resp.Data = lista;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando pk_Clientes.F_GetUnidadesPoliciales");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }




        #endregion


        #region Métodos para Inserción

        #endregion



        #region Métodos para Actualización

        #endregion

    }
}
