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
    public class DbFuncionarios : IDbFuncionarios
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        #endregion

        #region Constructor
        public DbFuncionarios(IConfiguration iConfiguration,
                IDbConsultasPIP dbConsultasPIP,
                                ILogger<DbAdministracion> logger
                                )
        {
            _iConfiguration = iConfiguration;
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
            _iDbConsultasPIP = dbConsultasPIP;
        }
        #endregion

        #region Métodos de Consulta
        public async Task<DtoResultado<DtoUsuario>> F_GetFuncionarios(Int64 V_Identificacion)
        {
            DataTable resultado = new();
            DtoUsuario retorno = new();
            DtoResultado<DtoUsuario> resp = new();

            var respuestaPIP = await _iDbConsultasPIP.ObtenerDatosFuncionarioIdAsync(V_Identificacion);

            try
            {
               
                if (respuestaPIP.Estado)
                {

                    retorno.SituacionLaboral = respuestaPIP.Respuesta.SituacionLaboral;
                    retorno.Funcionario = respuestaPIP.Respuesta.Funcionario;
                    retorno.Correo = respuestaPIP.Respuesta.CorreoElectronico;
                    retorno.Usuario = respuestaPIP.Respuesta.UsuarioEmpresarial;
                    retorno.Celular = (long)respuestaPIP.Respuesta.NumeroCelular;
                    retorno.Dependencia = respuestaPIP.Respuesta.DescripcionDependencia;
                    retorno.Cargo = respuestaPIP.Respuesta.Cargo;
                    retorno.Fisica = respuestaPIP.Respuesta.SiglaFisica;
                    retorno.IdUndeLaborando = respuestaPIP.Respuesta.UndeConsecutivoLaborando;

                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetFuncionarios";
                    resp.Data = retorno;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error ejecutando F_GetFuncionarios: " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException?.Message}";
                resp.Operacion = "0";
            }
            finally
            {
               
            }

            return resp;
        }


        public async Task<DtoFuncionarios> F_GetDatosAdicionalesPorIdentificacion(decimal identificacion)
        {
            var resultado = new DtoFuncionarios();

            using var conexion = new OracleConnection(_strConexionTelepol);
            using var comando = new OracleCommand();
            try
            {
                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;
                comando.CommandText = @"SELECT t.direccion, t.undelaborando, C.NIVEL1
                                          FROM USR_MATERIALIZADAS.VM_CTR_FUNCIONARIOS_ACTIVOS t
                                          LEFT JOIN USR_MATERIALIZADAS.Vm_Ctr_Unidades_Dependencia C 
                                            ON T.UNDELABORANDO = C.CONSECUTIVO  WHERE t.IDENTIFICACION =  :identificacion";
                comando.Parameters.Add(new OracleParameter("identificacion", OracleDbType.Int64)).Value = identificacion;

                await conexion.OpenAsync();

                using var reader = await comando.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    resultado.DIRECCION = reader["DIRECCION"]?.ToString() ?? string.Empty;
                    resultado.ESTACION = reader["NIVEL1"]?.ToString() ?? string.Empty;

                    if (reader["UNDELABORANDO"] != DBNull.Value && int.TryParse(reader["UNDELABORANDO"].ToString(), out int unidad))
                    {
                        resultado.UNDELABORANDO = unidad;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consultando dirección del funcionario: {ex.Message}");
            }
            finally
            {
                comando.Dispose();
                conexion.Close();
                conexion.Dispose();
            }

            return resultado;
        }


        public async Task<DtoResultado<List<DtoFuncionarios>>> F_GetEmpleadoIntel(string V_Busqueda)
        {
            DataTable resultado = new();
            List<DtoFuncionarios> retorno = new();
            DtoResultado<List<DtoFuncionarios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionTelepol);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "USR_MATERIALIZADAS.PK_FUNCIONARIOS.F_GetEmpleadoIntel";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add("V_Busqueda", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Busqueda;

                if (Conexion.State == ConnectionState.Open)
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoFuncionarios>(resultado);

                if (retorno.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetFuncionarios";
                    resp.Data = retorno;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_FUNCIONARIOS.F_GetEmpleadoIntel " + e);

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
        #endregion
    }
}
