 using Comun.Areas.Admin;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Admin;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Admin
{
    public class DbAdministracion : IDbAdministracion
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        #endregion

        #region Constructor
        public DbAdministracion(IConfiguration iConfiguration,
                                IDbConsultasPIP dbConsultasPIP,
                                ILogger<DbAdministracion> logger
                                )
        {
            _iConfiguration = iConfiguration;
            _iDbConsultasPIP = dbConsultasPIP;
            strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        #region Metodos de Consulta     
        

        public DataTable F_GetImagenes(int Consecutivo)
        {
            DataTable objRetorno = new();

            using var Conexion = new OracleConnection(_strConexionTelepol);
            using var objCommand = new OracleCommand();
            using var adaptador = new OracleDataAdapter();

            {
                try
                {
                    objCommand.Connection = Conexion;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.CommandText = "SELECT CONSECUTIVO, CONTENT_TYPE, FILENAME, FOTO FROM IMAGENES WHERE CONSECUTIVO = :consecutivo";
                    Conexion.Open();

                    objCommand.Parameters.Clear();
                    objCommand.Parameters.Add(new OracleParameter(":consecutivo", Consecutivo)).Direction = ParameterDirection.Input;

                    if (Conexion.State == ConnectionState.Open)
                    {
                        adaptador.SelectCommand = objCommand;
                        adaptador.Fill(objRetorno);

                        Conexion.Close();
                        Conexion.Dispose();
                        objCommand.Connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                    _logger.LogError("Error Ejecutando la consulta de imagenes(): [ID error:]", ex);
                }
                finally
                {
                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Dispose();
                    objCommand.Connection.Close();
                }
            }
            return objRetorno;
        }
        public async Task<DtoResultado<List<DtoMenu>>> F_GetMenu(string V_Idrol, Int64 P_Identificacion)
        {
            DataTable resultado = new();
            List<DtoMenu> retorno = new();
            DtoResultado<List<DtoMenu>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetMenu";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add("P_IdRol", OracleDbType.Int32, ParameterDirection.Input).Value = V_Idrol;
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Int64, ParameterDirection.Input).Value = P_Identificacion;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoMenu>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetMenu";
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
                _logger.LogWarning("Error Ejecutando PKS_ADMINISTRACION_IRIS.F_GetMenu " + e);

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
        public async Task<DtoResultado<List<DtoSlider>>> F_GetSilerSuperior()
        {
            DataTable resultado = new();
            List<DtoSlider> retorno = new();
            DtoResultado<List<DtoSlider>> resp = new();

            using var Conexion = new OracleConnection(_strConexionTelepol);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CTR_ADMINISTRACION.F_GetSlider";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoSlider>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetMenu";
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
                _logger.LogWarning("Error Ejecutando PK_CTR_ADMINISTRACION.F_GetSlider " + e);

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


        public async Task<DtoResultado<List<DtoUsuario>>> F_GetListUsuarios()
        {
            DataTable resultado = new();
            List<DtoUsuario> retorno = new();
            DtoResultado<List<DtoUsuario>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetUsuarios";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("v_consulta", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    if (resultado.Rows.Count > 0)
                    {
                        foreach (DataRow fila in resultado.Rows)
                        {
                            retorno.Add(new DtoUsuario
                            {

                                GradAlfabetico = fila["GradAlfabetico"].ToString(),
                                Funcionario = fila["Funcionario"].ToString(),
                                Identificacion = Convert.ToInt32(fila["Identificacion"].ToString()),
                                Cargo = fila["Cargo"].ToString(),
                            });
                        }

                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDominios";
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
                _logger.LogWarning("Error Ejecutando PK_CTR_ADMINISTRACION.F_GetSlider " + e);

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

                
        public async Task<DtoResultado<DtoUsuario>> P_GetValidaUser(string V_Usuario, string V_Maquina)
        {
            DataTable resultado = new();
            DtoUsuario retorno = new();
            DtoResultado<DtoUsuario> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.P_GetValidaUser";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_Usuario", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Usuario;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;
               
                objCommand.Parameters.Add("Out_Identificacion", OracleDbType.Int64).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("Out_IdUsuario", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("Out_Bloqueado", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("CursorRoles", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                

                if (Conexion.State == ConnectionState.Open)
                {

                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno.Identificacion = Convert.ToInt64(objCommand.Parameters["Out_Identificacion"].Value.ToString());
                    //retorno.IdCargo = Convert.ToInt32(objCommand.Parameters["Out_IdCargo"].Value.ToString()); //no 
                    retorno.IdUsuario = Convert.ToInt32(objCommand.Parameters["Out_IdUsuario"].Value.ToString()); // tabla usuarios
                    retorno.Bloqueado = Convert.ToInt32(objCommand.Parameters["Out_Bloqueado"].Value.ToString()); // no tabla

                    var respuestaPIP = await _iDbConsultasPIP.ObtenerDatosFuncionarioIdAsync(retorno.Identificacion);

                    if (respuestaPIP.Estado)
                    {
                        retorno.GradAlfabetico = respuestaPIP.Respuesta.GradAlfabetico;
                        retorno.NombreGrado = respuestaPIP.Respuesta.NombreGrado;
                        retorno.EmplUndeFuerza = respuestaPIP.Respuesta.UndeFuerza;
                        retorno.EmplConsecutivo = respuestaPIP.Respuesta.Consecutivo;
                        retorno.Funcionario = respuestaPIP.Respuesta.Funcionario;
                        retorno.EmplUndeConsecutivo = respuestaPIP.Respuesta.UndeConsecutivo;
                        //retorno.EmplUndeFuerza = respuestaPIP.Respuesta.UndeFuerza;
                        retorno.Cargo = respuestaPIP.Respuesta.Cargo;
                        retorno.Usuario = respuestaPIP.Respuesta.UsuarioEmpresarial;
                        retorno.IdUndeLaborando = respuestaPIP.Respuesta.UndeConsecutivoLaborando;
                        retorno.Fisica = respuestaPIP.Respuesta.SiglaFisica;
                        retorno.Dependencia = respuestaPIP.Respuesta.DescripcionDependencia;
                        retorno.Correo = respuestaPIP.Respuesta.CorreoElectronico;
                        retorno.SituacionLaboral = respuestaPIP.Respuesta.SituacionLaboral;
                        retorno.Celular = (long)respuestaPIP.Respuesta.NumeroCelular;
                        retorno.Nombres = respuestaPIP.Respuesta.Nombres;
                        retorno.ApellidosNombres = respuestaPIP.Respuesta.Apellidos;

                    }



                    retorno.DtoUserRoles = new List<DtoUserRoles>();

                    if (resultado.Rows.Count > 0)
                    {
                        foreach (DataRow fila in resultado.Rows)
                        {
                            DtoUserRoles ObjR = new DtoUserRoles
                            {
                                IdRol = Convert.ToInt32(fila["IdRol"].ToString()),
                                Descripcion = fila["Descripcion"].ToString()
                            };
                            retorno.DtoUserRoles.Add(ObjR);
                        }

                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "P_GetValidaUser";

                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                        resp.Data = retorno;
                    }

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                   
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();

                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PKS_ADMINISTRACION_IRIS.P_GetValidaUser " + e.Message);
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
        
        
        public async Task<DtoResultado<List<DtoRoles>>> F_GetRoles()
        {
            DataTable resultado = new();
            List<DtoRoles> retorno = new();
            DtoResultado<List<DtoRoles>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetRoles";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    if (resultado.Rows.Count > 0)
                    {
                        foreach (DataRow fila in resultado.Rows)
                        {
                            retorno.Add(new DtoRoles
                            {
                                IDROL = Convert.ToInt32(fila["IdRol"].ToString()),
                                DESCRIPCION = fila["Descripcion"].ToString()
                            });
                        }

                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDominios";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetDominios " + e);

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
       
        public async Task<DtoResultado<List<DtoUserRoles>>> F_GetUserRoles(Int64 V_Identificacion)
        {
            List<DtoUserRoles> Retorno = new();
            DtoResultado<List<DtoUserRoles>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.F_GetUserRoles";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Int64, ParameterDirection.Input).Value = V_Identificacion;

                if (Conexion.State == ConnectionState.Open)
                {
                    var reader = await objCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var domi = new DtoUserRoles()
                        {
                            IdRol = reader.GetInt32(0),
                            IdUserRol = reader.GetInt32(1),
                            IdUsuario = reader.GetInt32(2),
                            Descripcion = reader.GetString(3),
                            FechaCreacion = reader.GetString(4),
                            FuncionarioCreacion = reader.GetString(5),
                            FechaFin = reader.GetString(6),
                            Justificacion = reader.GetString(7),
                            Bloqueado = reader.GetInt32(8)
                        };
                        Retorno.Add(domi);
                    }

                    if (Retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetRolesUser";
                        resp.Data = Retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No existe usuario creado en el sistema";
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
                _logger.LogWarning("Error Ejecutando PKS_ADMINISTRACION_IRIS.F_GetRolesUser " + e);

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
        #endregion

        #region Métodos de Inserción y Actualización
        public async Task<DtoResultado<Int32>> P_InsAuditoria(Int64 V_Identificacion, string V_Evento, string V_Descripcion, Int64 V_Identificador, string V_Maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.P_InsAuditoria";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Evento", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Evento;
                objCommand.Parameters.Add("P_Descripcion", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Descripcion;
                objCommand.Parameters.Add("P_Identificador", OracleDbType.Int64, ParameterDirection.Input).Value = V_Identificador;

                objCommand.Parameters.Add("P_Usuario", OracleDbType.Int64, ParameterDirection.Input).Value = V_Identificacion;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;

                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    await objCommand.ExecuteNonQueryAsync();
                    resp.IdRespuesta = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());
                    resp.Mensaje = "Registro grabado exitosamente";
                    resp.Operacion = "Ins_Auditoria";
                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error al conectar a la base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error al Insertar : PK_CTR_ADMINISTRACION.P_InsAuditoria " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

                objCommand.Dispose();
                objCommand.Connection.Close();
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
        public async Task<DtoResultado<Int32>> P_InsRolesUser(DtoInsUserRoles obj, Int64 V_Usuario, string V_Maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.P_InsRolesUser";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_IdUsuario", OracleDbType.Int32, ParameterDirection.Input).Value = obj.IdUsuario;
                objCommand.Parameters.Add("P_IdRol", OracleDbType.Int32, ParameterDirection.Input).Value = obj.IdRol;
                objCommand.Parameters.Add("P_Justificacion", OracleDbType.Varchar2, ParameterDirection.Input).Value = obj.Justificacion;
                objCommand.Parameters.Add("P_FechaFin", OracleDbType.Date, ParameterDirection.Input).Value = obj.FechaFin;

                objCommand.Parameters.Add("P_Usuario", OracleDbType.Int64, ParameterDirection.Input).Value = V_Usuario;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;

                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    await objCommand.ExecuteNonQueryAsync();
                    resp.IdRespuesta = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());
                    resp.Mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();
                    resp.Operacion = "P_InsRoles";
                    resp.Data = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error al conectar a la base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error al Insertar : PK_CTR_ADMINISTRACION.P_InsRolesUser " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

                objCommand.Dispose();
                objCommand.Connection.Close();

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
            }
            return resp;
        }
        public async Task<DtoResultado<Int32>> P_InsUdpUsuarios(Int64 V_Identificacion, int V_Bloqueado, Int64 V_Usuario, string V_Maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.P_InsUdpUsuarios";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Int64, ParameterDirection.Input).Value = V_Identificacion;
                objCommand.Parameters.Add("P_Bloqueado", OracleDbType.Int32, ParameterDirection.Input).Value = V_Bloqueado;


                objCommand.Parameters.Add("P_Usuario", OracleDbType.Int64, ParameterDirection.Input).Value = V_Usuario;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;

                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    await objCommand.ExecuteNonQueryAsync();
                    resp.IdRespuesta = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());
                    resp.Mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();
                    resp.Operacion = "P_InsUdpUsuarios";
                    resp.Data = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error al conectar a la base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error al Insertar : PK_CTR_ADMINISTRACION.P_InsUdpUsuarios " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

                objCommand.Dispose();
                objCommand.Connection.Close();

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
            }
            return resp;
        }
        #endregion

        #region Métodos de Eliminación
        public async Task<DtoResultado<Int32>> P_DelRoles(DtoInsUserRoles obj, Int64 V_Usuario, string V_Maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PKS_ADMINISTRACION_IRIS.P_DelRoles";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_IdUserRol", OracleDbType.Int32, ParameterDirection.Input).Value = obj.IdUserRol;
                objCommand.Parameters.Add("P_Justificacion", OracleDbType.Varchar2, ParameterDirection.Input).Value = obj.Justificacion;

                objCommand.Parameters.Add("P_Usuario", OracleDbType.Int64, ParameterDirection.Input).Value = V_Usuario;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;

                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    await objCommand.ExecuteNonQueryAsync();
                    resp.IdRespuesta = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());
                    resp.Mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();
                    resp.Operacion = "P_DeleteRoles";
                    resp.Data = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error al conectar a la base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error al Insertar : PK_CTR_ADMINISTRACION.P_DelRoles " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

                objCommand.Dispose();
                objCommand.Connection.Close();

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
            }
            return resp;
        }
        #endregion
    }
}
