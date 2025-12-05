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

        //public async Task<DtoResultado<List<DtoDominios>>> F_GetDominiosIris(Int32 V_Id)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionIris_Disec);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetDominiosIris";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input).Value = V_Id;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["IdDominio"].ToString()),
        //                        Descripcion = fila["Descripcion"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetDominios";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando PK_IRIS_PRUEBA.F_GetDominiosIris " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}




        //public async Task<DtoResultado<List<DtoDominios>>> F_GetDominios(Int32 V_Id)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionDesa);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetDominios";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input).Value = V_Id;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["IdDominio"].ToString()),
        //                        Descripcion = fila["Descripcion"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetDominios";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetDominios " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}


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

        //public async Task<DtoResultado<List<DtoDominios>>> F_GetDepartamentos(Int32 V_Id)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionDesa);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "pk_Clientes.F_GetDepartamentos";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input).Value = V_Id;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["Codigo"].ToString()),
        //                        Descripcion = fila["Descripcion"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetDepartamentos";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetDepartamentos " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}

        //public async Task<DtoResultado<List<DtoDominios>>> F_GetMunicipios(Int32 V_Id)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionDesa);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "pk_Clientes.F_GetMunicipios";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input).Value = V_Id;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["Codigo"].ToString()),
        //                        Descripcion = fila["Descripcion"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetMunicipios";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetMunicipios " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}

        //public async Task<DtoResultado<List<DtoDominios>>> F_GetDependencias(string V_SiglaPapa)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionDesa);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "pk_Clientes.F_GetDepedencias";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("v_unidad", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_SiglaPapa;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["Consecutivo"].ToString()),
        //                        Descripcion = fila["Dependencia"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetDepedencias";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetDepedencias " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}
        //public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPoliciales(Int32 V_Id)
        ////public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPoliciales(string V_Id)
        //{
        //    DataTable resultado = new();
        //    List<DtoDominios> retorno = new();
        //    DtoResultado<List<DtoDominios>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionDesa);
        //    using var objCommand = new OracleCommand();
        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "pk_Clientes.F_GetUnidadesPoliciales";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input).Value = V_Id;
        //        //objCommand.Parameters.Add("P_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Id;


        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            if (resultado.Rows.Count > 0)
        //            {
        //                foreach (DataRow fila in resultado.Rows)
        //                {
        //                    retorno.Add(new DtoDominios
        //                    {
        //                        IdDominio = Convert.ToInt32(fila["consecutivo"].ToString()),
        //                        Descripcion2 = fila["sigla_fisica"].ToString(),
        //                        Descripcion = fila["descripcion_dependencia"].ToString()
        //                    });
        //                }

        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetUnidadesPoliciales";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }

        //            Conexion.Close();
        //            Conexion.Dispose();
        //            objCommand.Connection.Close();
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetUnidadesPoliciales " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}


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
