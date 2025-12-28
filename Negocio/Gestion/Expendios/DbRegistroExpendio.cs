using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Expendios;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Negocio.Gestion.Expendios
{
    public class DbRegistroExpendio: IDbRegistroExpendio
    {


        #region Propiedades
        private readonly IConfiguration _iConfiguration;
       
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbRegistroExpendio(IConfiguration iConfiguration, ILogger<IDbRegistroExpendio> logger)
        {
            _iConfiguration = iConfiguration;
           
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion


        public async Task<DtoResultado<List<DtoExpendios>>> F_GetAniosIrisP1()
        {
            List<DtoExpendios> Retorno = new();
            DtoResultado<List<DtoExpendios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetAniosIrisP1";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    var reader = await objCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var domi = new DtoExpendios()
                        {
                            AnoIrisp1 = reader.GetInt32(0),
                        };
                        Retorno.Add(domi);
                    }

                    if (Retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_AniosIris";
                        resp.Data = Retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encuentran registros en base de datos";
                        resp.Operacion = "0";
                    }

                    reader.Close();
                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_CONSULTA_IRISP.F_AniosIris " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
            }
            return resp;
        }

       

        public async Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(Int32 V_Anio, string RolesUsuario, Int64 CodigoUnidad)
        {
            DataTable resultado = new();
            List<DtoExpendios> retorno = new();
            DtoResultado<List<DtoExpendios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetInfoGrillas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                objCommand.Parameters.Add("P_Roles", OracleDbType.Varchar2, ParameterDirection.Input).Value = RolesUsuario;
                objCommand.Parameters.Add("P_CodigoUnidad", OracleDbType.Int64, ParameterDirection.Input).Value = CodigoUnidad;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoExpendios>(resultado);

                    resp.IdRespuesta = retorno.Count > 0 ? 1 : 0;
                    resp.Mensaje = retorno.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                    resp.Data = retorno;
                    resp.Operacion = "F_GetInfoGrillas";
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_EXPENDIOS_IRIS.F_GetInfoGrillas");
                resp.IdRespuesta = 0;
                resp.Mensaje = e.Message;
            }
            finally
            {
                Conexion.Close();
                objCommand.Dispose();
            }

            return resp;
        }



        public async Task<DtoResultado<string>> F_ConsultarSeqIris()
        {
            var resp = new DtoResultado<string>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);
                using var command = new OracleCommand("SELECT PK_EXPENDIOS_IRIS.f_consultar_seq_Iris FROM dual", conexion)
                {
                    CommandType = CommandType.Text
                };

                await conexion.OpenAsync();

                var result = await command.ExecuteScalarAsync();

                // Convertir el resultado a string
                string consecutivo = result?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(consecutivo))
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta exitosa";
                    resp.Data = consecutivo;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo obtener el consecutivo";
                    resp.Data = string.Empty;
                }
            }
            catch (Exception e)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error en consulta: {e.Message} {(e.InnerException?.Message ?? string.Empty)}";
                resp.Data = string.Empty;
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId)
        {
            var respuesta = new DtoResultado<List<DtoIntegrantes>>();

            try
            {
                using (var conn = new OracleConnection(_strConexionIris_Disec))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OracleCommand("PK_EXPENDIOS_IRIS.F_GetIntegrantes", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, 100).Value = V_CriminalidadId;
                        cmd.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var lista = new List<DtoIntegrantes>();

                            while (await reader.ReadAsync())
                            {
                                lista.Add(new DtoIntegrantes
                                {
                                    INTEGRANTE_DIREC_ID = reader["INTEGRANTE_DIREC_ID"]?.ToString(),
                                    CRIMINALIDAD_DIREC_ID = reader["CRIMINALIDAD_DIREC_ID"]?.ToString(),
                                    ALIAS = reader["ALIAS"]?.ToString(),
                                    NOMBRE = reader["NOMBRE"]?.ToString(),
                                    APELLIDO = reader["APELLIDO"]?.ToString(),
                                    CEDULA = reader["CEDULA"] as long?,
                                    //ID_TIPO_INFO = reader["ID_TIPO_INFO"] as int?,
                                    VIGENTE = reader["VIGENTE"] as int?,
                                    FECHA_CREACION = reader["FECHA_CREACION"] as DateTime?,
                                    IDENTIFICACION_CREACION = reader["IDENTIFICACION_CREACION"] as long?,
                                    MAQUINA_CREACION = reader["MAQUINA_CREACION"]?.ToString(),
                                    FECHA_MODIFICA = reader["FECHA_MODIFICA"] as DateTime?,
                                    IDENTIFICACION_MODIFICA = reader["IDENTIFICACION_MODIFICA"] as long?,
                                    MAQUINA_MODIFICA = reader["MAQUINA_MODIFICA"]?.ToString(),
                                    //TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] as int?,
                                    //CELULAR = reader["CELULAR"] as long?,
                                    //DIRECCION = reader["DIRECCION"]?.ToString(),
                                   // ID_INTEGRANTE = reader["ID_INTEGRANTE"] as long?,
                                    //ID_CRIMINALIDAD = reader["ID_CRIMINALIDAD"] as long?
                                });
                            }

                            respuesta.Data = lista;
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "Consulta exitosa";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 0;
                respuesta.Mensaje = $"Error: {ex.Message}";
                respuesta.Data = new List<DtoIntegrantes>();
            }

            return respuesta;
        }



        public async Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoDelitosIris> retorno = new();
            DtoResultado<List<DtoDelitosIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetDelitosIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoDelitosIris>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDelitosIris";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_REGISTRO_IRIS.F_GetDelitosIris " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }

        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var respuesta = new DtoResultado<List<DtoIntegrantes>>();

            try
            {
                using (var conn = new OracleConnection(_strConexionIris_Disec))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OracleCommand("PK_EXPENDIOS_IRIS.F_GetIntegrantesPreliminar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, 40).Value = V_CriminalidadId;
                        cmd.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var lista = new List<DtoIntegrantes>();

                            while (await reader.ReadAsync())
                            {
                                lista.Add(new DtoIntegrantes
                                {
                                    INTEGRANTE_DIREC_ID = reader["INTEGRANTE_DIREC_ID"]?.ToString(),
                                    CRIMINALIDAD_DIREC_ID = reader["CRIMINALIDAD_DIREC_ID"]?.ToString(),
                                    ALIAS = reader["ALIAS"]?.ToString(),
                                    NOMBRE = reader["NOMBRE"]?.ToString(),
                                    APELLIDO = reader["APELLIDO"]?.ToString(),
                                    CEDULA = reader["CEDULA"] as long?,
                                    //ID_TIPO_INFO = reader["ID_TIPO_INFO"] as int?,
                                    VIGENTE = reader["VIGENTE"] as int?,
                                    FECHA_CREACION = reader["FECHA_CREACION"] as DateTime?,
                                    IDENTIFICACION_CREACION = reader["IDENTIFICACION_CREACION"] as long?,
                                    MAQUINA_CREACION = reader["MAQUINA_CREACION"]?.ToString(),
                                    FECHA_MODIFICA = reader["FECHA_MODIFICA"] as DateTime?,
                                    IDENTIFICACION_MODIFICA = reader["IDENTIFICACION_MODIFICA"] as long?,
                                    MAQUINA_MODIFICA = reader["MAQUINA_MODIFICA"]?.ToString(),
                                    //TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] as int?,
                                    //CELULAR = reader["CELULAR"] as long?,
                                    //DIRECCION = reader["DIRECCION"]?.ToString(),
                                    // ID_INTEGRANTE = reader["ID_INTEGRANTE"] as long?,
                                    //ID_CRIMINALIDAD = reader["ID_CRIMINALIDAD"] as long?
                                });
                            }

                            respuesta.Data = lista;
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "Consulta exitosa";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 0;
                respuesta.Mensaje = $"Error: {ex.Message}";
                respuesta.Data = new List<DtoIntegrantes>();
            }

            return respuesta;
        }


        public async Task<DtoResultado<List<DtoInfoAdicional>>> F_GetBitacora(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoInfoAdicional> retorno = new();
            DtoResultado<List<DtoInfoAdicional>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetBitacora";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoInfoAdicional>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDelitosIris";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetBitacora " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }



        public async Task<DtoResultado<List<DtoResultadosExpendio>>> F_GetResultados(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoResultadosExpendio> retorno = new();
            DtoResultado<List<DtoResultadosExpendio>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetResultados";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoResultadosExpendio>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetResultados";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetResultados " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }


        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegranteAll(Int64 V_Identificacion)
        {
            DataTable resultado = new();
            List<DtoIntegrantes> retorno = new();
            DtoResultado<List<DtoIntegrantes>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetIntegranteAll";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Identificacion;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIntegrantes>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetIntegranteAll";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetIntegranteAll " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }



        public async Task<DtoResultado<string>> P_InsRegistroExpendio(DtoInsExpendios Obj_NuevoExpendio, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsCriminalidad_Direc";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();


                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.CRIMINALIDAD_ID;
                objCommand.Parameters.Add("P_ID_UNIDAD", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_UNIDAD;
                objCommand.Parameters.Add("P_BARRIO", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.BARRIO;
                objCommand.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.DIRECCION;
                objCommand.Parameters.Add("P_LONGITUD", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.LONGITUD;
                objCommand.Parameters.Add("P_LATITUD", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.LATITUD;
                objCommand.Parameters.Add("P_CUADRANTE", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.CUADRANTE;
                objCommand.Parameters.Add("P_CATEGORIA", OracleDbType.Int32).Value = Obj_NuevoExpendio.CATEGORIA;
                objCommand.Parameters.Add("P_OTRA_CATEGORIA", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.OTRA_CATEGORIA;
                objCommand.Parameters.Add("P_MUNICIPIO", OracleDbType.Varchar2).Value = Obj_NuevoExpendio.MUNICIPIO;
                objCommand.Parameters.Add("P_ID_UNIDAD_INFORMA", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_UNIDAD_INFORMA;
                objCommand.Parameters.Add("P_ID_ZONA", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_ZONA;
                objCommand.Parameters.Add("P_ID_CLASE", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_CLASE;
                objCommand.Parameters.Add("P_ID_EXPENDIO", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_EXPENDIO;
                objCommand.Parameters.Add("P_ID_ESTADO", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_ESTADO;
                objCommand.Parameters.Add("P_ID_FUENTE", OracleDbType.Int32).Value = Obj_NuevoExpendio.ID_FUENTE;
                objCommand.Parameters.Add("P_FECHA_INICIO_EXISTENCIA", OracleDbType.Date).Value = Obj_NuevoExpendio.FECHA_INICIO_EXISTENCIA;
                objCommand.Parameters.Add("P_CARACTERISTICAS_GENERALES", OracleDbType.NVarchar2).Value = Obj_NuevoExpendio.CARACTERISTICAS_GENERALES;

                // Auditoría
                objCommand.Parameters.Add("P_IDENTIFICACION_CREA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;

                // Delitos (cadena separada por comas normalmente)

                objCommand.Parameters.Add("P_ID_DELITOS", OracleDbType.NVarchar2).Value = string.Join(",", Obj_NuevoExpendio.ID_DELITOS);

                // Parámetros de salida
                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                    await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value?.ToString() ?? "0");
                string mensaje = objCommand.Parameters["SRV_Message"].Value?.ToString() ?? "";

                if (resultado > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = mensaje;
                    resp.Data = "OK";
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = mensaje;
                    resp.Data = "";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsCriminalidad");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }
            finally
            {
                if (Conexion.State == ConnectionState.Open)
                    Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }
        public async Task<DtoResultado<Int32>> P_InsIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsIntegrante";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Integrante.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_CEDULA", OracleDbType.Int64).Value = Obj_Integrante.CEDULA ;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Integrante.ALIAS ;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Integrante.NOMBRE;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Integrante.APELLIDO;
                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<Int32>> P_InsIntegrantePreliminar(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsIntegrantePreliminar";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Integrante.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_CEDULA", OracleDbType.Int64).Value = Obj_Integrante.CEDULA;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Integrante.ALIAS;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Integrante.NOMBRE;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Integrante.APELLIDO;
                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<Int32>> P_InsDelito(DtoDelitosIris Obj_Delito, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsDelito";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Delito.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_ID_DELITO", OracleDbType.Int64).Value = Obj_Delito.IdDelito;
                
                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<Int32>> P_InsBitacora(DtoInfoAdicional Obj_Bitacora, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsBitacora";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Bitacora.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_DESCRIPCION", OracleDbType.Varchar2).Value = Obj_Bitacora.Descripcion;

                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }

        public async Task<DtoResultado<Int32>> P_InsResultados(DtoResultadosExpendio Obj_Resultados, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_InsResultados";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Resultados.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_ID_TIPO", OracleDbType.Int32).Value = Obj_Resultados.ID_TIPO;
                objCommand.Parameters.Add("P_ID_SUBTIPO", OracleDbType.Int32).Value = Obj_Resultados.ID_SUBTIPO;
                objCommand.Parameters.Add("P_CANTIDAD", OracleDbType.Int32).Value = Obj_Resultados.CANTIDAD;
                objCommand.Parameters.Add("P_FECHA", OracleDbType.Date).Value = Obj_Resultados.FECHA;

                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }



        public async Task<DtoResultado<Int32>> P_UpdExpendio(DtoExpendios Obj_UpdExpendio, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_UpdExpendio";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_UpdExpendio.CriminalidadDirecId;
                objCommand.Parameters.Add("P_ID_ESTADO", OracleDbType.Int32).Value = Obj_UpdExpendio.IdEstado;
                objCommand.Parameters.Add("P_NUNC", OracleDbType.Int32).Value = Obj_UpdExpendio.Nunc;
                objCommand.Parameters.Add("P_SIEDCO", OracleDbType.Int32).Value = Obj_UpdExpendio.Siedco;
                objCommand.Parameters.Add("P_COD_OPERACION", OracleDbType.Varchar2).Value = Obj_UpdExpendio.CodigoMored;
                objCommand.Parameters.Add("P_NOMBRE_OPERACION", OracleDbType.Varchar2).Value = Obj_UpdExpendio.NombreMored;
                objCommand.Parameters.Add("P_ERRADICADO", OracleDbType.Int32).Value = Obj_UpdExpendio.Erradicado;
                objCommand.Parameters.Add("P_OBSERVACIONES", OracleDbType.Varchar2).Value = Obj_UpdExpendio.Observacion;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<Int32>> P_UpdIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.P_UpdIntegrante";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Integrante.CRIMINALIDAD_DIREC_ID;
                objCommand.Parameters.Add("P_INTEGRANTE_DIREC_ID", OracleDbType.Varchar2).Value = Obj_Integrante.INTEGRANTE_DIREC_ID;
                //objCommand.Parameters.Add("P_CEDULA", OracleDbType.Int64).Value = Obj_Integrante.CEDULA;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Integrante.ALIAS;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Integrante.NOMBRE;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Integrante.APELLIDO;
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoDominios>>> F_GetEstaciones(string V_Sigla)
        {
            DataTable resultado = new();
            List<DtoDominios> retorno = new();
            DtoResultado<List<DtoDominios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetEstaciones";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_sigla", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Sigla;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoDominios>(resultado);
                       

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetEstaciones";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetEstaciones " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }



        public async Task<DtoResultado<List<DtoDominios>>> F_GetEspecialidad(string V_Sigla)
        {
            DataTable resultado = new();
            List<DtoDominios> retorno = new();
            DtoResultado<List<DtoDominios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetEspecialidad";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_sigla", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Sigla;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoDominios>(resultado);


                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetEspecialidad";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetEspecialidad " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }




    }
}
