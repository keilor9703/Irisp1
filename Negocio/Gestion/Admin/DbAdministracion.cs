using Comun.Areas.Admin;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Admin;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Gestion.Admin
{
    public class DbAdministracion : IDbAdministracion
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        
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
           
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion



        // Convierte Base64 → Texto (como en proyecto viejo)
        public string ConvertirBase64Bytes(string texto)
        {
            byte[] data = Convert.FromBase64String(texto);
            return Encoding.UTF8.GetString(data);
        }

        // MÉTODO DECRIPCIÓN EXACTO YA MIGRADO A DOTNET CORE
        public string Decript(string message, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.Key = Convert.FromBase64String(key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor();

                // limpiar espacios igual que el código original
                string mensajeSinEspacios = message.Replace(" ", "");

                byte[] data = Convert.FromBase64String(mensajeSinEspacios);
                byte[] decryptedData = decryptor.TransformFinalBlock(data, 0, data.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
        }


        #region Metodos de Consulta     

        public async Task<DtoResultado<List<DtoMenu>>> F_GetMenu(int idRol, long identificacion)
        {
            var resp = new DtoResultado<List<DtoMenu>>
            {
                Operacion = "F_GetMenu",
                Data = new List<DtoMenu>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                p.Add("P_IdRol", idRol, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Identificacion", identificacion, OracleMappingType.Int64, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoMenu>(
                    "PKS_ADMINISTRACION_IRIS.F_GetMenu",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoMenu>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | Rol={Rol} | Identificacion={Identificacion}",
                    resp.Operacion, idRol, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoUsuario>>> F_GetListUsuarios()
        {
            var resp = new DtoResultado<List<DtoUsuario>>
            {
                Operacion = "F_GetUsuarios",
                Data = new List<DtoUsuario>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                // connection.BindByName = true;

                var p = new OracleDynamicParameters();

                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoUsuario>(
                    "PKS_ADMINISTRACION_IRIS.F_GetUsuarios",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoUsuario>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoRoles>>> F_GetRoles()
        {
            var resp = new DtoResultado<List<DtoRoles>>
            {
                Operacion = "F_GetRoles",
                Data = new List<DtoRoles>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);


                var p = new OracleDynamicParameters();

                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoRoles>(
                    "PKS_ADMINISTRACION_IRIS.F_GetRoles",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoRoles>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }

       

        public async Task<DtoResultado<List<DtoUserRoles>>> F_GetUserRoles(long V_Identificacion)
        {
            var resp = new DtoResultado<List<DtoUserRoles>>
            {
                Operacion = "F_GetUserRoles",
                Data = new List<DtoUserRoles>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);


                var p = new OracleDynamicParameters();

                p.Add("P_Identificacion", V_Identificacion, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoUserRoles>(
                    "PKS_ADMINISTRACION_IRIS.F_GetUserRoles",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoUserRoles>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }




        public async Task<DtoResultado<DtoUsuario>> P_GetValidaUser(string usuario, string maquina)
        {
            var resp = new DtoResultado<DtoUsuario>();

            var db = await ValidarUsuarioDbAsync(usuario, maquina);

            var u = new DtoUsuario
            {
                Identificacion = db.Identificacion,
                IdUsuario = db.IdUsuario,
                Bloqueado = db.Bloqueado,
                DtoUserRoles = db.Roles
            };

            if (db.Resultado <= 0 || db.IdUsuario <= 0)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = "Usuario no encontrado o inválido";
                resp.Operacion = "P_GetValidaUser";
                resp.Data = u;
                return resp;
            }

            // Enriquecer con PIP (fuera del repositorio)
            var pip = await _iDbConsultasPIP.ObtenerDatosFuncionarioIdAsync(db.Identificacion);
            if (pip.Estado)
            {
                u.GradAlfabetico = pip.Respuesta.GradAlfabetico;
                u.NombreGrado = pip.Respuesta.NombreGrado;
                u.EmplUndeFuerza = pip.Respuesta.UndeFuerza;
                u.EmplConsecutivo = pip.Respuesta.Consecutivo;
                u.Funcionario = pip.Respuesta.Funcionario;
                u.EmplUndeConsecutivo = pip.Respuesta.UndeConsecutivo;
                u.Cargo = pip.Respuesta.Cargo;
                u.Usuario = pip.Respuesta.UsuarioEmpresarial;
                u.IdUndeLaborando = pip.Respuesta.UndeConsecutivoLaborando;
                u.Fisica = pip.Respuesta.SiglaFisica;
                u.Dependencia = pip.Respuesta.DescripcionDependencia;
                u.Correo = pip.Respuesta.CorreoElectronico;
                u.SituacionLaboral = pip.Respuesta.SituacionLaboral;
                u.Celular = (long)pip.Respuesta.NumeroCelular;
                u.Nombres = pip.Respuesta.Nombres;
                u.ApellidosNombres = pip.Respuesta.Apellidos;
            }

            resp.IdRespuesta = 1;
            resp.Mensaje = "Consulta Exitosa";
            resp.Operacion = "P_GetValidaUser";
            resp.Data = u;
            return resp;
        }


        public async Task<DtoValidaUserDb> ValidarUsuarioDbAsync(string usuario, string maquina)
        {
            using var cn = new OracleConnection(_strConexionIris_Disec);

            var p = new OracleDynamicParameters();
            p.Add("P_Usuario", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
            p.Add("P_Maquina", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

            p.Add("Out_Identificacion", dbType: OracleMappingType.Int64, direction: ParameterDirection.Output);
            p.Add("Out_IdUsuario", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            p.Add("Out_Bloqueado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            p.Add("CursorRoles", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            p.Add("P_Resultado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);


            // QueryAsync lee el cursor (CursorRoles) como resultset
            var roles = (await cn.QueryAsync<DtoUserRoles>(
                "PKS_ADMINISTRACION_IRIS.P_GetValidaUser",
                param: p,
                commandType: CommandType.StoredProcedure
            )).AsList();

            var resultado = p.Get<int>("P_Resultado");

            return new DtoValidaUserDb
            {
                Resultado = resultado,
                Identificacion = p.Get<long>("Out_Identificacion"),
                IdUsuario = p.Get<int>("Out_IdUsuario"),
                Bloqueado = p.Get<int>("Out_Bloqueado"),
                Roles = roles
            };
        }




        #endregion

        #region Métodos de Inserción y Actualización
        public async Task<DtoResultado<int>> P_InsAuditoria(long V_Identificacion,string V_Evento, string V_Descripcion,long V_Identificador, string V_Maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsAuditoria",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                //connection.BindByName = true;

                var p = new OracleDynamicParameters();

                // Inputs
                p.Add("P_Evento", (V_Evento ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input );
                p.Add("P_Descripcion", (V_Descripcion ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input );
                p.Add("P_Identificador", V_Identificador, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Usuario", V_Identificacion, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (V_Maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);

                // Outputs
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 1000);
                p.Add("P_Resultado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                // Inserción / ejecución
                await connection.ExecuteAsync(
                    "PKS_ADMINISTRACION_IRIS.P_InsAuditoria",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                // Leer OUTs
                var resultado = p.Get<int?>("P_Resultado") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;          
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Registro grabado exitosamente" : "No fue posible registrar la auditoría.");
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Usuario={Usuario} | Evento={Evento} | Identificador={Identificador} | Maquina={Maquina}",
                    resp.Operacion, V_Identificacion, V_Evento, V_Identificador, V_Maquina);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Operacion = "0";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Usuario={Usuario} | Evento={Evento} | Identificador={Identificador} | Maquina={Maquina}",
                    resp.Operacion, V_Identificacion, V_Evento, V_Identificador, V_Maquina);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = ex.Message;
                resp.Operacion = "0";
            }

            return resp;
        }

        public async Task<DtoResultado<int>> P_InsRolesUser(DtoInsUserRoles obj, long V_Usuario, string V_Maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsRolesUser",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
              //  connection.BindByName = true;

                var p = new OracleDynamicParameters();

                // IN
                p.Add("P_IdUsuario", obj.IdUsuario, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_IdRol", obj.IdRol, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Justificacion", (obj.Justificacion ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 2000);

                // FechaFin: si puede venir null, ideal que el DTO sea DateTime? y se envíe null
                // Si NO es nullable, envía el valor tal cual.
                p.Add("P_FechaFin", obj.FechaFin, OracleMappingType.Date, ParameterDirection.Input);

                p.Add("P_Usuario", V_Usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (V_Maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 200);

                // OUT
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);
                p.Add("P_Resultado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PKS_ADMINISTRACION_IRIS.P_InsRolesUser",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_Resultado") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Registro grabado exitosamente" : "No fue posible registrar el rol.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | IdUsuario={IdUsuario} | IdRol={IdRol} | UsuarioAudita={UsuarioAudita} | Maquina={Maquina}",
                    resp.Operacion, obj?.IdUsuario, obj?.IdRol, V_Usuario, V_Maquina);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = ex.Message;
                resp.Operacion = "0";
            }

            return resp;
        }

        public async Task<DtoResultado<int>> P_InsUdpUsuarios(long V_Identificacion, int V_Bloqueado, long V_Usuario, string V_Maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsUdpUsuarios",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
               // connection.BindByName = true;

                var p = new OracleDynamicParameters();

                // IN
                p.Add("P_Identificacion", V_Identificacion, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Bloqueado", V_Bloqueado, OracleMappingType.Int32, ParameterDirection.Input);

                p.Add("P_Usuario", V_Usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (V_Maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 200);

                // OUT
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);
                p.Add("P_Resultado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PKS_ADMINISTRACION_IRIS.P_InsUdpUsuarios",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_Resultado") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Registro grabado exitosamente" : "No fue posible actualizar el usuario.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion} | Bloqueado={Bloqueado} | UsuarioAudita={UsuarioAudita} | Maquina={Maquina}",
                    resp.Operacion, V_Identificacion, V_Bloqueado, V_Usuario, V_Maquina);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = ex.Message;
                resp.Operacion = "0";
            }

            return resp;
        }

        #endregion

        #region Métodos de Eliminación
        public async Task<DtoResultado<int>> P_DelRoles(DtoInsUserRoles obj, long V_Usuario, string V_Maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_DelRoles",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
               // connection.BindByName = true;

                var p = new OracleDynamicParameters();

                // IN
                p.Add("P_IdUserRol", obj.IdUserRol, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Justificacion", (obj.Justificacion ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 2000);

                p.Add("P_Usuario", V_Usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (V_Maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 200);

                // OUT
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);
                p.Add("P_Resultado", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PKS_ADMINISTRACION_IRIS.P_DelRoles",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_Resultado") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Eliminación exitosa" : "No fue posible eliminar el rol.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | IdUserRol={IdUserRol} | UsuarioAudita={UsuarioAudita} | Maquina={Maquina}",
                    resp.Operacion, obj?.IdUserRol, V_Usuario, V_Maquina);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = ex.Message;
                resp.Operacion = "0";
            }

            return resp;
        }

        #endregion
    }
}
