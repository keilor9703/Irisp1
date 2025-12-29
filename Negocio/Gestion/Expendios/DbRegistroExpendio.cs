using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Expendios;
using Oracle.ManagedDataAccess.Client;

using System.Data;


namespace Negocio.Gestion.Expendios
{
    public class DbRegistroExpendio : IDbRegistroExpendio
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbRegistroExpendio> _logger;
        #endregion

        #region Constructor
        public DbRegistroExpendio(IConfiguration iConfiguration, ILogger<DbRegistroExpendio> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        #region Consultas

        public async Task<DtoResultado<List<DtoExpendios>>> F_GetAniosIrisP1()
        {
            var resp = new DtoResultado<List<DtoExpendios>>
            {
                Operacion = "F_GetAniosIrisP1",
                Data = new List<DtoExpendios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                // Si tu DTO tiene la propiedad AnoIrisp1 y el cursor devuelve columna compatible, mapea directo.
                var lista = (await connection.QueryAsync<DtoExpendios>(
                    "PK_EXPENDIOS_IRIS.F_GetAniosIrisP1",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                // Si el SP solo devuelve un int y tu DTO no mapea por nombre, puedes mapear manual:
                // var anios = (await connection.QueryAsync<int>(...)).ToList();
                // lista = anios.Select(x => new DtoExpendios { AnoIrisp1 = x }).ToList();

                resp.Data = lista ?? new List<DtoExpendios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encuentran registros en base de datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(int V_Anio, string RolesUsuario, long CodigoUnidad)
        {
            var resp = new DtoResultado<List<DtoExpendios>>
            {
                Operacion = "F_GetInfoGrillas",
                Data = new List<DtoExpendios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Anio", V_Anio, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Roles", RolesUsuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoExpendios>(
                    "PK_EXPENDIOS_IRIS.F_GetInfoGrillas",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoExpendios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_Anio={V_Anio} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, V_Anio, RolesUsuario, CodigoUnidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_Anio={V_Anio} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, V_Anio, RolesUsuario, CodigoUnidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<string>> F_ConsultarSeqIris()
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "F_ConsultarSeqIris",
                Data = string.Empty
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                await connection.OpenAsync();

                // Nota: en tu SQL original faltaban paréntesis si realmente es función.
                // Mantengo tu query. Si en Oracle es función, normalmente sería: SELECT PK_EXPENDIOS_IRIS.f_consultar_seq_Iris() FROM dual
                var consecutivo = await connection.ExecuteScalarAsync<string>(
                    "SELECT PK_EXPENDIOS_IRIS.f_consultar_seq_Iris FROM dual",
                    commandType: CommandType.Text
                );

                consecutivo ??= string.Empty;

                resp.IdRespuesta = !string.IsNullOrWhiteSpace(consecutivo) ? 1 : 0;
                resp.Mensaje = resp.IdRespuesta == 1 ? "Consulta exitosa" : "No se pudo obtener el consecutivo";
                resp.Data = consecutivo;

                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = string.Empty;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error en consulta: {ex.Message}";
                resp.Data = string.Empty;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoIntegrantes>>> P_GetIntegrantes(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoIntegrantes>>
            {
                Operacion = "P_GetIntegrantes",
                Data = new List<DtoIntegrantes>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIntegrantes>(
                    "PK_EXPENDIOS_IRIS.P_GetIntegrantes",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoIntegrantes>();
                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta exitosa";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoIntegrantes>();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = new List<DtoIntegrantes>();
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoDelitosIris>>
            {
                Operacion = "F_GetDelitosIris",
                Data = new List<DtoDelitosIris>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoDelitosIris>(
                    "PK_EXPENDIOS_IRIS.F_GetDelitosIris",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoDelitosIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoIntegrantes>>
            {
                Operacion = "F_GetIntegrantesPreliminar",
                Data = new List<DtoIntegrantes>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIntegrantes>(
                    "PK_EXPENDIOS_IRIS.F_GetIntegrantesPreliminar",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoIntegrantes>();
                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta exitosa";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoIntegrantes>();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = new List<DtoIntegrantes>();
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoInfoAdicional>>> F_GetBitacora(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoInfoAdicional>>
            {
                Operacion = "F_GetBitacora",
                Data = new List<DtoInfoAdicional>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoInfoAdicional>(
                    "PK_EXPENDIOS_IRIS.F_GetBitacora",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoInfoAdicional>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoResultadosExpendio>>> F_GetResultados(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoResultadosExpendio>>
            {
                Operacion = "F_GetResultados",
                Data = new List<DtoResultadosExpendio>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoResultadosExpendio>(
                    "PK_EXPENDIOS_IRIS.F_GetResultados",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoResultadosExpendio>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegranteAll(long V_Identificacion)
        {
            var resp = new DtoResultado<List<DtoIntegrantes>>
            {
                Operacion = "F_GetIntegranteAll",
                Data = new List<DtoIntegrantes>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Identificacion", V_Identificacion, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIntegrantes>(
                    "PK_EXPENDIOS_IRIS.F_GetIntegranteAll",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoIntegrantes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_Identificacion={V_Identificacion}",
                    resp.Operacion, V_Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_Identificacion={V_Identificacion}",
                    resp.Operacion, V_Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoDominios>>> F_GetEstaciones(string V_Sigla)
        {
            var resp = new DtoResultado<List<DtoDominios>>
            {
                Operacion = "F_GetEstaciones",
                Data = new List<DtoDominios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_sigla", V_Sigla, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PK_EXPENDIOS_IRIS.F_GetEstaciones",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoDominios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | V_Sigla={V_Sigla}", resp.Operacion, V_Sigla);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | V_Sigla={V_Sigla}", resp.Operacion, V_Sigla);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public async Task<DtoResultado<List<DtoDominios>>> F_GetEspecialidad(string V_Sigla)
        {
            var resp = new DtoResultado<List<DtoDominios>>
            {
                Operacion = "F_GetEspecialidad",
                Data = new List<DtoDominios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_sigla", V_Sigla, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PK_EXPENDIOS_IRIS.F_GetEspecialidad",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoDominios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | V_Sigla={V_Sigla}", resp.Operacion, V_Sigla);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | V_Sigla={V_Sigla}", resp.Operacion, V_Sigla);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        #endregion

        #region Inserts / Updates 

        public async Task<DtoResultado<string>> P_InsRegistroExpendio(DtoInsExpendios Obj_NuevoExpendio, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsRegistroExpendio",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_CRIMINALIDAD_ID", Obj_NuevoExpendio.CRIMINALIDAD_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_UNIDAD", Obj_NuevoExpendio.ID_UNIDAD, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_BARRIO", Obj_NuevoExpendio.BARRIO, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_DIRECCION", Obj_NuevoExpendio.DIRECCION, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_LONGITUD", Obj_NuevoExpendio.LONGITUD, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_LATITUD", Obj_NuevoExpendio.LATITUD, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CUADRANTE", Obj_NuevoExpendio.CUADRANTE, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CATEGORIA", Obj_NuevoExpendio.CATEGORIA, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_OTRA_CATEGORIA", Obj_NuevoExpendio.OTRA_CATEGORIA, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MUNICIPIO", Obj_NuevoExpendio.MUNICIPIO, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_UNIDAD_INFORMA", Obj_NuevoExpendio.ID_UNIDAD_INFORMA, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_ZONA", Obj_NuevoExpendio.ID_ZONA, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_CLASE", Obj_NuevoExpendio.ID_CLASE, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_EXPENDIO", Obj_NuevoExpendio.ID_EXPENDIO, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_ESTADO", Obj_NuevoExpendio.ID_ESTADO, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_FUENTE", Obj_NuevoExpendio.ID_FUENTE, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_FECHA_INICIO_EXISTENCIA", Obj_NuevoExpendio.FECHA_INICIO_EXISTENCIA, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_CARACTERISTICAS_GENERALES", Obj_NuevoExpendio.CARACTERISTICAS_GENERALES, OracleMappingType.NVarchar2, ParameterDirection.Input);

                // Auditoría
                p.Add("P_IDENTIFICACION_CREA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA_CREACION", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Delitos CSV
                var delitosCsv = Obj_NuevoExpendio.ID_DELITOS is null ? "" : string.Join(",", Obj_NuevoExpendio.ID_DELITOS);
                p.Add("P_ID_DELITOS", delitosCsv, OracleMappingType.NVarchar2, ParameterDirection.Input);

                // Outputs
                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsCriminalidad_Direc",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadId={CriminalidadId}",
                    resp.Operacion, Obj_NuevoExpendio?.CRIMINALIDAD_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadId={CriminalidadId}",
                    resp.Operacion, Obj_NuevoExpendio?.CRIMINALIDAD_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = "";
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_InsIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsIntegrante",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Integrante.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CEDULA", Obj_Integrante.CEDULA, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_ALIAS", Obj_Integrante.ALIAS, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_NOMBRE", Obj_Integrante.NOMBRE, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_APELLIDO", Obj_Integrante.APELLIDO, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsIntegrante",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_InsIntegrantePreliminar(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsIntegrantePreliminar",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Integrante.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CEDULA", Obj_Integrante.CEDULA, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_ALIAS", Obj_Integrante.ALIAS, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_NOMBRE", Obj_Integrante.NOMBRE, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_APELLIDO", Obj_Integrante.APELLIDO, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsIntegrantePreliminar",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_InsDelito(DtoDelitosIris Obj_Delito, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsDelito",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Delito.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_DELITO", Obj_Delito.IdDelito, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsDelito",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Delito?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Delito?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_InsBitacora(DtoInfoAdicional Obj_Bitacora, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsBitacora",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Bitacora.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_DESCRIPCION", Obj_Bitacora.Descripcion, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsBitacora",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Bitacora?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Bitacora?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_InsResultados(DtoResultadosExpendio Obj_Resultados, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsResultados",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Resultados.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_TIPO", Obj_Resultados.ID_TIPO, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_ID_SUBTIPO", Obj_Resultados.ID_SUBTIPO, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_CANTIDAD", Obj_Resultados.CANTIDAD, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_FECHA", Obj_Resultados.FECHA, OracleMappingType.Date, ParameterDirection.Input);

                p.Add("P_USUARIO", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_InsResultados",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Resultados?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_Resultados?.CRIMINALIDAD_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_UpdExpendio(DtoExpendios Obj_UpdExpendio, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_UpdExpendio",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_UpdExpendio.CriminalidadDirecId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_ESTADO", Obj_UpdExpendio.IdEstado, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_NUNC", Obj_UpdExpendio.Nunc, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_SIEDCO", Obj_UpdExpendio.Siedco, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_COD_OPERACION", Obj_UpdExpendio.CodigoMored, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_NOMBRE_OPERACION", Obj_UpdExpendio.NombreMored, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ERRADICADO", Obj_UpdExpendio.Erradicado, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_OBSERVACIONES", Obj_UpdExpendio.Observacion, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_UpdExpendio",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_UpdExpendio?.CriminalidadDirecId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | CriminalidadDirecId={Id}",
                    resp.Operacion, Obj_UpdExpendio?.CriminalidadDirecId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        public async Task<DtoResultado<int>> P_UpdIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_UpdIntegrante",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_DIREC_ID", Obj_Integrante.CRIMINALIDAD_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_INTEGRANTE_DIREC_ID", Obj_Integrante.INTEGRANTE_DIREC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ALIAS", Obj_Integrante.ALIAS, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_NOMBRE", Obj_Integrante.NOMBRE, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_APELLIDO", Obj_Integrante.APELLIDO, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_EXPENDIOS_IRIS.P_UpdIntegrante",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | IntegranteDirecId={IntegranteId}",
                    resp.Operacion, Obj_Integrante?.INTEGRANTE_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | IntegranteDirecId={IntegranteId}",
                    resp.Operacion, Obj_Integrante?.INTEGRANTE_DIREC_ID);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
                return resp;
            }
        }

        #endregion
    }
}
