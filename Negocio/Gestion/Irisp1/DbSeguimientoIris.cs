using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Irisp1
{
    public class DbSeguimientoIris : IDbSeguimientoIris
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbSeguimientoIris(IConfiguration iConfiguration, ILogger<IDbSeguimientoIris> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
        }
        #endregion

        public async Task<DtoResultado<List<SeguimientoIrisDto>>> F_GetAniosIrisP1()
        {
            List<SeguimientoIrisDto> Retorno = new();
            DtoResultado<List<SeguimientoIrisDto>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetAniosIrisP1";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    var reader = await objCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var domi = new SeguimientoIrisDto()
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

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio)
        {
            DataTable resultado = new();
            List<DtoIrispCriminalidad> retorno = new();
            DtoResultado<List<DtoIrispCriminalidad>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.F_GetInfoGrillas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetInfoGrillas";
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
                _logger.LogWarning("Error Ejecutando PK_SEGUIMIENTO_IRIS.F_GetInfoGrillas " + e);

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
        public async Task<List<SeguimientoDto>> ConsultarSeguimientoIris(string _anio)
        {
            var seguimientoIris = new List<SeguimientoDto>();

            try
             {
                using (var conexion = new OracleConnection(_strConexionIris_Test))
                {
                    await conexion.OpenAsync();

                    using (var comando = new OracleCommand("PK_SEGUIMIENTO_IRIS.F_GetConsultIris", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        // 🔹 La función devuelve un REF CURSOR
                        comando.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                        // 🔹 Parámetro de entrada
                        comando.Parameters.Add("P_Anio", OracleDbType.Decimal).Value = Convert.ToInt32(_anio);

                        using (var reader = await comando.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var Iris = new SeguimientoDto
                                {
                                    CriminalidadId = reader["CRIMINALIDADID"]?.ToString(),                                   
                                    IdResponsable = reader["IDRESPONSABLE"]?.ToString(),
                                    IdEstado = reader["IDESTADO"] != DBNull.Value ? Convert.ToInt32(reader["IDESTADO"]) : (int?)null,
                                    EstadoDescripcion = reader["ESTADODESCRIPCION"]?.ToString(),
                                    IdEstadoExistencia = reader["IDESTADOEXISTENCIA"] != DBNull.Value ? Convert.ToInt32(reader["IDESTADOEXISTENCIA"]) : (int?)null,
                                    EstadoExistenciaDescripcion = reader["ESTADOEXISTENCIADESCRIPCION"]?.ToString(),
                                    Codigo = reader["CODIGO"]?.ToString(),
                                    IdUnidadResponsable = reader["IDUNIDADRESPONSABLE"] != DBNull.Value ? Convert.ToInt32(reader["IDUNIDADRESPONSABLE"]) : (int?)null,
                                    UnidadResponsable = reader["UNIDADRESPONSABLE"]?.ToString(),
                                    IdUnidad = reader["IDUNIDAD"] != DBNull.Value ? Convert.ToInt32(reader["IDUNIDAD"]) : (int?)null,
                                    Unidad = reader["UNIDAD"]?.ToString(),
                                    Dependencia = reader["DEPENDENCIA"]?.ToString(),
                                    Municipio = reader["MUNICIPIO"]?.ToString(),
                                    FechaInicioExistencia = reader["FECHAINICIOEXISTENCIA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAINICIOEXISTENCIA"]) : (DateTime?)null,
                                    IdClase = reader["IDCLASE"] != DBNull.Value ? Convert.ToInt32(reader["IDCLASE"]) : (int?)null,
                                    Clase = reader["CLASE"]?.ToString(),
                                    NombreClase = reader["NOMBRECLASE"]?.ToString(),
                                    CantidadIntegrantes = reader["CANTIDADINTEGRANTES"] != DBNull.Value ? Convert.ToInt32(reader["CANTIDADINTEGRANTES"]) : (int?)null,
                                    CaracteristicasGenerales = reader["CARACTERISTICASGENERALES"]?.ToString(),
                                    DescripcionTramite = reader["DESCRIPCIONTRAMITE"]?.ToString(),
                                    IdZona = reader["IDZONA"] != DBNull.Value ? Convert.ToInt32(reader["IDZONA"]) : (int?)null,
                                    Zona = reader["ZONA"]?.ToString(),
                                    TipoServicio = reader["TIPOSERVICIO"]?.ToString(),
                                    IdFuente = reader["IDFUENTE"] != DBNull.Value ? Convert.ToInt32(reader["IDFUENTE"]) : (int?)null,
                                    Fuente = reader["FUENTE"]?.ToString(),
                                    FechaCreacion = reader["FECHACREACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACREACION"]) : (DateTime?)null,
                                    IdentificacionInforma = reader["IDENTIFICACIONINFORMA"] != DBNull.Value ? Convert.ToInt64(reader["IDENTIFICACIONINFORMA"]) : (long?)null,
                                    Celular = reader["CELULAR"]?.ToString(),
                                    IdTipoServicio = reader["IDTIPOSERVICIO"] != DBNull.Value ? Convert.ToInt32(reader["IDTIPOSERVICIO"]) : (int?)null,
                                    IdCuadrante = reader["IDCUADRANTE"] != DBNull.Value ? Convert.ToInt32(reader["IDCUADRANTE"]) : (int?)null,
                                    Vigente = reader["VIGENTE"] != DBNull.Value ? Convert.ToInt16(reader["VIGENTE"]) : (short?)null,
                                    MaquinaCrea = reader["MAQUINACREA"]?.ToString(),
                                    IdentificacionCrea = reader["IDENTIFICACIONCREA"] != DBNull.Value ? Convert.ToInt64(reader["IDENTIFICACIONCREA"]) : (long?)null,
                                    FechaModifica = reader["FECHAMODIFICA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAMODIFICA"]) : (DateTime?)null,
                                    IdentificacionModifica = reader["IDENTIFICACIONMODIFICA"] != DBNull.Value ? Convert.ToInt64(reader["IDENTIFICACIONMODIFICA"]) : (long?)null,
                                    MaquinaModifica = reader["MAQUINAMODIFICA"]?.ToString(),
                                    ConsecutivoCodigo = reader["CONSECUTIVOCODIGO"] != DBNull.Value ? Convert.ToInt32(reader["CONSECUTIVOCODIGO"]) : (int?)null,
                                    SiglaUnidad = reader["SIGLAUNIDAD"]?.ToString(),
                                    Cuadrante = reader["CUADRANTE"]?.ToString(),
                                    DependCuadrante = reader["DEPENDCUADRANTE"]?.ToString(),
                                    EstacionCuadrante = reader["ESTACIONCUADRANTE"]?.ToString(),
                                    Nivel1Cuadrante = reader["NIVEL1CUADRANTE"]?.ToString(),
                                    CelularCuadrante = reader["CELULARCUADRANTE"]?.ToString(),
                                    //FechaAsignacionVerificacionExistencia = reader["FECHAASIGNACIONVERIFICACIONEXISTENCIA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAASIGNACIONVERIFICACIONEXISTENCIA"]) : (DateTime?)null,
                                    IdTipoResultado = reader["IDTIPORESULTADO"] != DBNull.Value ? Convert.ToInt32(reader["IDTIPORESULTADO"]) : (int?)null,
                                    DescTipoResultado = reader["DESCTIPORESULTADO"]?.ToString(),
                                    NumeroResultado = reader["NUMERORESULTADO"]?.ToString(),
                                    EstadoResultados = reader["ESTADORESULTADOS"]?.ToString()
                                };

                                seguimientoIris.Add(Iris);
                            }
                        }
                    }

                }
            }
            catch (OracleException ex)
            {
                _logger.LogError($"Error Oracle en Consultar SeguimientoIris: {ex.Message}");
                throw new Exception("Error Oracle al consultar seguimientos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado en ConsultarSeguimientoIris: {ex.Message}");
                throw new Exception("Error inesperado al consultar seguimientos.", ex);
            }

            return seguimientoIris;
        }

        //Task<DtoResultado<List<SeguimientoIrisDto>>> IDbSeguimientoIris.F_GetAniosIrisP1()
        //{
        //    throw new NotImplementedException();
        //}

        //Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(int V_Anio)
        //{
        //    throw new NotImplementedException();
        //}

        Task<DtoResultado<List<DtoIrispCriminalidad>>> IDbSeguimientoIris.F_GetInfoGrillas(int V_Anio)
        {
            return F_GetInfoGrillas(V_Anio);
        }
    }
}
