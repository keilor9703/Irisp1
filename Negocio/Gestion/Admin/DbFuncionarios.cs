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

namespace Negocio.Gestion.Admin
{
    public class DbFuncionarios : IDbFuncionarios
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionTelepol;
        private readonly ILogger<DbFuncionarios> _logger;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        #endregion

        #region Constructor
        public DbFuncionarios(
            IConfiguration iConfiguration,
            IDbConsultasPIP dbConsultasPIP,
            ILogger<DbFuncionarios> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
            _iDbConsultasPIP = dbConsultasPIP;
        }
        #endregion

        #region Métodos de Consulta

        public async Task<DtoResultado<DtoUsuario>> F_GetFuncionarios(long V_Identificacion)
        {
            var resp = new DtoResultado<DtoUsuario>
            {
                Operacion = "F_GetFuncionarios",
                Data = new DtoUsuario()
            };

            try
            {
                var respuestaPIP = await _iDbConsultasPIP.ObtenerDatosFuncionarioIdAsync(V_Identificacion);

                if (respuestaPIP is null)
                {
                    _logger.LogWarning("{Operacion} | Respuesta PIP nula | Identificacion={Identificacion}",
                        resp.Operacion, V_Identificacion);

                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No fue posible consultar el funcionario (respuesta nula).";
                    return resp;
                }

                if (!respuestaPIP.Estado || respuestaPIP.Respuesta is null)
                {
                    _logger.LogWarning(
                        "{Operacion} | PIP no exitoso | Identificacion={Identificacion} | Estado={Estado} | Codigo={Codigo} | Mensaje={Mensaje}",
                        resp.Operacion, V_Identificacion, respuestaPIP.Estado, respuestaPIP.Codigo, respuestaPIP.Mensaje);

                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    return resp;
                }


                // Mapear respuesta PIP -> DtoUsuario
                var retorno = new DtoUsuario
                {
                    SituacionLaboral = respuestaPIP.Respuesta.SituacionLaboral,
                    Funcionario = respuestaPIP.Respuesta.Funcionario,
                    Correo = respuestaPIP.Respuesta.CorreoElectronico,
                    Usuario = respuestaPIP.Respuesta.UsuarioEmpresarial,
                    Celular = (long)respuestaPIP.Respuesta.NumeroCelular,
                    Dependencia = respuestaPIP.Respuesta.DescripcionDependencia,
                    Cargo = respuestaPIP.Respuesta.Cargo,
                    Fisica = respuestaPIP.Respuesta.SiglaFisica,
                    IdUndeLaborando = respuestaPIP.Respuesta.UndeConsecutivoLaborando
                };



                resp.Data = retorno;
                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta Exitosa";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Operacion} | Error consultando funcionario por PIP | Identificacion={Identificacion}",
                    resp.Operacion, V_Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }

            

        }

        public async Task<DtoResultado<List<DtoFuncionarios>>> F_GetEmpleadoIntel(string V_Busqueda)
        {
            var resp = new DtoResultado<List<DtoFuncionarios>>
            {
                Operacion = "F_GetEmpleadoIntel",
                Data = new List<DtoFuncionarios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionTelepol);

                var parametros = new OracleDynamicParameters();

                // En tu código original el cursor estaba como ReturnValue.
                // En Dapper.Oracle puedes mapearlo como Output RefCursor con el mismo nombre.
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                parametros.Add("V_Busqueda", V_Busqueda, OracleMappingType.Varchar2, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoFuncionarios>(
                    "USR_MATERIALIZADAS.PK_FUNCIONARIOS.F_GetEmpleadoIntel",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoFuncionarios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";

                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_Busqueda={V_Busqueda}",
                    resp.Operacion, V_Busqueda);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_Busqueda={V_Busqueda}",
                    resp.Operacion, V_Busqueda);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        #endregion
    }
}
