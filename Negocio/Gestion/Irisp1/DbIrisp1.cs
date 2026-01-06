using Comun.Areas.Admin;
using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Negocio.Gestion.Irisp1
{
    public class DbIrisp1 : IDbIrisp1
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbIrisp1(IConfiguration iConfiguration, ILogger<IDbIrisp1> logger  )
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion


        #region Métodos de Consulta (Dapper)


        public async Task<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1()
        {
            var resp = new DtoResultado<List<DtoIrisp1>>
            {
                Operacion = "F_GetAniosIrisP1",
                Data = new List<DtoIrisp1>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                var parametros = new OracleDynamicParameters();
                
                // RefCursor de salida
                parametros.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIrisp1>("PK_CONSULTA_IRISP.F_GetAniosIrisP1", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<DtoIrisp1>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion}",
                    resp.Operacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} ",
                    resp.Operacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(int V_Anio,string RolesUsuario, long CodigoUnidad)
        {
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>
            {
                Operacion = "F_GetInfoGrillas",
                Data = new List<DtoIrispCriminalidad>()
            };

            try
            {
                // Validaciones mínimas (evita llamadas innecesarias a BD)
                if (V_Anio <= 0)
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "El año (V_Anio) no es válido.";
                    return resp;
                }

                RolesUsuario = (RolesUsuario ?? string.Empty).Trim();

                using var connection = new OracleConnection(_strConexionIris_Disec);

                // Recomendado para Oracle: bind por nombre
               // connection.BindByName = true;

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Anio", V_Anio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_Roles", RolesUsuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                // RefCursor de salida
                parametros.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIrispCriminalidad>( "PK_CONSULTA_IRISP.F_GetInfoGrillas", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120 )).AsList();

                resp.Data = lista ?? new List<DtoIrispCriminalidad>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_Anio={Anio} | CodigoUnidad={CodigoUnidad} | Roles={Roles}",
                    resp.Operacion, V_Anio, CodigoUnidad, RolesUsuario);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_Anio={Anio} | CodigoUnidad={CodigoUnidad} | Roles={Roles}",
                    resp.Operacion, V_Anio, CodigoUnidad, RolesUsuario);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoCuadrantes>>> P_GetCuadrantes(string V_unidadLabora, string V_unidadLabora2)
        {
            var resp = new DtoResultado<List<DtoCuadrantes>>
            {
                Operacion = "P_GetCuadrantes",
                Data = new List<DtoCuadrantes>()
            };

            try
            {
               
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Dependencia", V_unidadLabora, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_Dependencia2", V_unidadLabora2, OracleMappingType.Varchar2, ParameterDirection.Input);
               

                // RefCursor de salida
                parametros.Add("p_resultados", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoCuadrantes>("PK_REGISTRO_IRIS.P_GetCuadrantes", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<DtoCuadrantes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_unidadLabora={V_unidadLabora} | V_unidadLabora2={V_unidadLabora2} ",
                    resp.Operacion, V_unidadLabora, V_unidadLabora2);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_unidadLabora={V_unidadLabora} | V_unidadLabora2={V_unidadLabora2} ",
                    resp.Operacion, V_unidadLabora, V_unidadLabora2);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }



        public async Task<DtoResultado<long>> F_ConsultarSeqIris()
        {
            var resp = new DtoResultado<long>
            {
                Operacion = "f_consultar_seq_Iris",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                string sql = @"SELECT PK_REGISTRO_IRIS.f_consultar_seq_Iris FROM dual";

                await connection.OpenAsync();

                var resultado = await connection.ExecuteScalarAsync<long>(
                    sql,
                    commandType: CommandType.Text
                );

                resp.Data = resultado;
                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = resultado > 0 ? "Consulta exitosa" : "No se encontraron registros";
            }
            catch (OracleException oex)
            {
                // 🔹 Captura específica de errores Oracle
                _logger.LogError(oex, "Error Oracle ejecutando PK_REGISTRO_IRIS.f_consultar_seq_Iris");

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error Oracle consultando secuencia (Código {oex.Number}): {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                // 🔹 Captura genérica de cualquier otra excepción
                _logger.LogError(ex, "Error general ejecutando PK_REGISTRO_IRIS.f_consultar_seq_Iris");

                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando secuencia: " + ex.Message;
                resp.Data = 0;
            }

            return resp;
        
        
        }

        public async Task<DtoResultado<long>> F_ConsultarSeqIntegrante()
        {
            var resp = new DtoResultado<long>
            {
                Operacion = "f_consultar_seq_integrante",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                string sql = @"SELECT PK_REGISTRO_IRIS.f_consultar_seq_integrante FROM dual";

                await connection.OpenAsync();

                var resultado = await connection.ExecuteScalarAsync<long>(
                    sql,
                    commandType: CommandType.Text
                );

                resp.Data = resultado;
                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = resultado > 0 ? "Consulta exitosa" : "No se encontraron registros";
            }
            catch (OracleException oex)
            {
                // 🔹 Captura específica de errores Oracle
                _logger.LogError(oex, "Error Oracle ejecutando PK_REGISTRO_IRIS.f_consultar_seq_integrante");

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error Oracle consultando secuencia (Código {oex.Number}): {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                // 🔹 Captura genérica de cualquier otra excepción
                _logger.LogError(ex, "Error general ejecutando PK_REGISTRO_IRIS.f_consultar_seq_integrante");

                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando secuencia: " + ex.Message;
                resp.Data = 0;
            }

            return resp;


        }


        public async Task<DtoResultado<List<DtoIntegrantes>>> P_GetIntegrantes (string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoIntegrantes>>
            {
                Operacion = "P_GetIntegrantes",
                Data = new List<DtoIntegrantes>()
            };

            try
            {

                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
               
                // RefCursor de salida
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIntegrantes>("PK_REGISTRO_IRIS.P_GetIntegrantes", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<DtoIntegrantes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }



        public async Task<DtoResultado<List<DtoIntegrantes>>> P_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoIntegrantes>>
            {
                Operacion = "P_GetIntegrantesPreliminar",
                Data = new List<DtoIntegrantes>()
            };

            try
            {

                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);

                // RefCursor de salida
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIntegrantes>("PK_REGISTRO_IRIS.P_GetIntegrantesPreliminar", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<DtoIntegrantes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoDelitosIris>>> P_GetDelitosIris(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoDelitosIris>>
            {
                Operacion = "P_GetDelitosIris",
                Data = new List<DtoDelitosIris>()
            };

            try
            {

                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);

                // RefCursor de salida
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoDelitosIris>("PK_REGISTRO_IRIS.P_GetDelitosIris", parametros, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<DtoDelitosIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";

            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }




        public async Task<DtoResultado<List<DtoCriminalidadFoto>>> P_GetCriminalidadFotos(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoCriminalidadFoto>>
            {
                Operacion = "P_GetCriminalidadFotos",
                Data = new List<DtoCriminalidadFoto>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);

                // RefCursor de salida (en tu SP se llama P_Result)
                parametros.Add("P_Result", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoCriminalidadFoto>(
                    "PK_REGISTRO_IRIS.P_GetCriminalidadFotos",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoCriminalidadFoto>();

                if (resp.Data.Count > 0)
                {
                    var uncBase = _iConfiguration["RutasArchivosIris:RutaFotos"] ?? string.Empty;

                    foreach (var foto in resp.Data)
                    {
                        if (!string.IsNullOrWhiteSpace(foto.Ruta) && !string.IsNullOrWhiteSpace(uncBase))
                        {
                            foto.Ruta = foto.Ruta
                                .Replace(uncBase, "")
                                .Replace("\\", "/")
                                .TrimStart('/', '\\');
                        }
                    }

                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron fotos";
                }
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoCriminalidadFoto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoCriminalidadFoto>();
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoInfoAdicional>>> P_GetInfoAdicional(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoInfoAdicional>>
            {
                Operacion = "P_GetInfoAdicional",
                Data = new List<DtoInfoAdicional>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);

                // RefCursor de salida
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoInfoAdicional>(
                    "PK_REGISTRO_IRIS.P_GetInfoAdicional",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoInfoAdicional>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoDocumentoIris>>> F_GetDocIris(string criminalidadId)
        {
            var resp = new DtoResultado<List<DtoDocumentoIris>>();

            using var connection = new OracleConnection(_strConexionIris_Disec);

            var parameters = new OracleDynamicParameters();
            parameters.Add("P_Criminalidad_Id", criminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            var lista = (await connection.QueryAsync<DtoDocumentoIris>(
                "PK_REGISTRO_IRIS.F_GetDocIris",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

          
            if (lista.Count > 0)
            {
               // string uncBase = _iConfiguration["RutasArchivosIris:RutaDocumentos"];

                string uncBase;

                // Comparar con un DateTime
                DateTime fechaReferencia = DateTime.Parse("2025-10-01 19:17:24");


                foreach (var doc in lista)
                {
                    if (!string.IsNullOrEmpty(doc.Ruta))
                    {
                        if (doc.FechaCreacion >= fechaReferencia)
                        {
                            uncBase = _iConfiguration["RutasArchivosIris:RutaDocumentos"];
                            doc.TipoRuta = 1;
                        }
                        else
                        {
                            uncBase = _iConfiguration["RutasArchivosIris:RutaDocumentosAnterior"];
                            doc.TipoRuta = 2;
                        }



                        doc.Ruta = doc.Ruta
                            .Replace(uncBase, "")
                            .Replace("\\", "/")
                            .TrimStart('/', '\\');  // <-- remover slashes iniciales
                    }
                }



                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta Exitosa";
                resp.Operacion = "F_GetDocIris";
                resp.Data = lista;
            }

            else
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = "No se encontraron documentos";
                resp.Operacion = "0";
            }


            return resp;
        }




        public async Task<DtoResultado<List<DtoUbicacionIris>>> P_GetUbicacionIris(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoUbicacionIris>>
            {
                Operacion = "P_GetUbicacionIris",
                Data = new List<DtoUbicacionIris>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);

                // RefCursor de salida
                parametros.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoUbicacionIris>(
                    "PK_REGISTRO_IRIS.P_GetUbicacionIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoUbicacionIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                // resp.Data ya va como lista vacía
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | V_CriminalidadId={V_CriminalidadId} ",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                // resp.Data ya va como lista vacía
            }

            return resp;
        }



        #endregion



        #region Métodos de Insersión (Dapper)

        public async Task<DtoResultado<int>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsIntegrantes",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", Obj_Integrante.CRIMINALIDAD_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ALIAS", Obj_Integrante.ALIAS, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_NOMBRE", Obj_Integrante.NOMBRE, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_APELLIDO", Obj_Integrante.APELLIDO, OracleMappingType.Varchar2, ParameterDirection.Input);

             
                parametros.Add("P_CEDULA", Obj_Integrante.CEDULA, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_TIPO_INFO", Obj_Integrante.ID_TIPO_INFO, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_IDENTIFICACION_CREACION", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_TIPO_DOCUMENTO", Obj_Integrante.TIPO_DOCUMENTO, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CELULAR", Obj_Integrante.CELULAR, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_DIRECCION", Obj_Integrante.DIRECCION, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                // Execute (no retorna rows; solo llena outputs)
                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsIntegrantes",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                // Leer outputs
                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0; // mantengo tu misma lógica
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | CRIMINALIDAD_ID={CRIMINALIDAD_ID} | usuario={usuario} | maquina={maquina}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_ID, usuario, maquina);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | CRIMINALIDAD_ID={CRIMINALIDAD_ID} | usuario={usuario} | maquina={maquina}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_ID, usuario, maquina);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }


        public async Task<DtoResultado<int>> P_InsIntegrantesPreliminar(
         DtoIntegrantes Obj_Integrante,
         string usuario,
         string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsIntegrantesPreliminar",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", Obj_Integrante.CRIMINALIDAD_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ALIAS", Obj_Integrante.ALIAS, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_NOMBRE", Obj_Integrante.NOMBRE, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_APELLIDO", Obj_Integrante.APELLIDO, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CEDULA", Obj_Integrante.CEDULA, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_TIPO_INFO", Obj_Integrante.ID_TIPO_INFO, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_IDENTIFICACION_CREACION", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_TIPO_DOCUMENTO", Obj_Integrante.TIPO_DOCUMENTO, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CELULAR", Obj_Integrante.CELULAR, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_DIRECCION", Obj_Integrante.DIRECCION, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsIntegrantesPreliminar",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | CRIMINALIDAD_ID={CRIMINALIDAD_ID} | usuario={usuario} | maquina={maquina}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_ID, usuario, maquina);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | CRIMINALIDAD_ID={CRIMINALIDAD_ID} | usuario={usuario} | maquina={maquina}",
                    resp.Operacion, Obj_Integrante?.CRIMINALIDAD_ID, usuario, maquina);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }


        public async Task<DtoResultado<string>> P_InsRegistroIrisP1(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsRegistroIrisP1",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_UNIDAD", datos.IdUnidad, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_ID_ZONA",
                    datos.IdZona.HasValue ? datos.IdZona.Value : (object)DBNull.Value,
                    OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_IDENTIFICACION_INFORMA",
                    datos.IdentificacionInforma.HasValue ? datos.IdentificacionInforma.Value : (object)DBNull.Value,
                    OracleMappingType.Int64, ParameterDirection.Input);

                parametros.Add("P_CELULAR", datos.Celular ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_TIPO_SERVICIO", datos.IdTipoServicio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_ID_CUADRANTE", datos.IdCuadrante, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_ID_CLASE", datos.IdClase, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_NOMBRE_CLASE", datos.NombreClase ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CANTIDAD_INTEGRANTE", datos.CantidadIntegrantes ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CARACTERISTICAS_GENERALES", datos.CaracteristicasGenerales ?? "", OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_VIGENTE", datos.Vigente, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_IDENTIFICACION_CREA", Convert.ToInt64(usuario), OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_SIGLA_UNIDAD", datos.SiglaUnidad ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_ESTADO", datos.IdEstado, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_ID_FUENTE", datos.IdFuente, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_ENTORNO_AFECTADO", datos.EntornoAfectado, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_ID_TIEMPO_DELITO", datos.IdtiempoDelito, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CLASIFICACION", datos.Clasificacion ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_MODALIDAD_EXPENDIO", datos.Modalidadexpendio ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_ORIGEN", datos.Origen ?? "WEB", OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_NOMBRE_ENTORNO_AFECTADO", datos.NombreEntornoAfectado ?? "", OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_ESPECIALIDAD_APORTA_INFO", datos.EspecialidadAporta ?? 0, OracleMappingType.Int32, ParameterDirection.Input);

                parametros.Add("P_ID_CRIMINALIDAD", datos.IdCriminalidad, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_DELITO_PRIN", datos.IdDelitoPrincipal, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_DELITO_SECUNDARIO", string.Join(",", datos.IdDelitoSecundario), OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_ID_TINFO", datos.IdTipoInfo, OracleMappingType.Int64, ParameterDirection.Input);

                parametros.Add("P_LONGITUD", datos.Longitud ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_LATITUD", datos.Latitud ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_MUNICIPIO", datos.MunicipioUbica ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_BARRIO", datos.Barrio ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CUADRANTE", datos.Cuadrante ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_RADACCION", datos.RadioAccion ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_DIRECCION", datos.Direccion ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_COD_DANE", datos.CodigoDane ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_COD_ESTACION", datos.CodigoEstacion ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_COD_SIEDO_CTE", datos.CodigoSiedcoCuadrante ?? 0, OracleMappingType.Int32, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsCriminalidad",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsCriminalidad");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }


        public async Task<DtoResultado<string>> P_InsDelitosIris(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsDelitosIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_CRIMINALIDAD", datos.IdCriminalidad, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_DELITO_PRIN", datos.IdDelitoPrincipal, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_DELITO_SECUNDARIO", string.Join(",", datos.IdDelitoSecundario), OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_CREA", Convert.ToInt64(usuario), OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsDelito",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsDelito");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }


        public async Task<DtoResultado<string>> P_InsInfoAdicionalIris(DtoInfoAdicional datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsInfoAdicionalIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_DESCRIPCION", datos.Descripcion, OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_CREACION", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsInfoAdicionalIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsInfoAdicionalIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }




        public async Task<DtoResultado<string>> P_InsUbicacionIris(DtoUbicacionIris datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsUbicacionIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                parametros.Add("P_LATITUD", datos.Latitud ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_LONGITUD", datos.Longitud ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_MUNICIPIO", datos.MunicipioUbica ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_BARRIO", datos.Barrio ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CUADRANTE", datos.CuadranteUbica ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_RADIO_ACCION", datos.RadioAccion ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_IDENTIFICACION_CREACION", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_CREACION", maquina ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_DIRECCION", datos.Direccion ?? "", OracleMappingType.NVarchar2, ParameterDirection.Input);

                parametros.Add("P_CODIGO_DANE", datos.CodigoDane, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_CODIGO_ESTACION", datos.CodigoEstacion, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_CODIGO_SIEDCO_CUADRANTE", datos.CodigoSiedcoCuadrante, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_ID_CRIMINALIDAD", datos.IdCriminalidad, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsUbicacion",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsUbicacion");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }



        #endregion


        #region Métodos de Actualización (Dapper)

        public async Task<DtoResultado<string>> P_UpdCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_UpdCriminalidad",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_CLASE", datos.IdClase, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_NOMBRE_CLASE", datos.NombreClase, OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_CANTIDAD_INTEGRANTE", datos.CantidadIntegrantes, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_CARACTERISTICAS_GENERALES", datos.CaracteristicasGenerales, OracleMappingType.NVarchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_FUENTE", datos.IdFuente, OracleMappingType.Int32, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_UpdCriminalidad",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadId={CriminalidadId}", resp.Operacion, datos?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdCriminalidad");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_UpdEstadoCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_UpdEstadoCriminalidad",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_ESTADO", datos.IdEstado, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_UpdEstadoCriminalidad",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadId={CriminalidadId}", resp.Operacion, datos?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdEstadoCriminalidad");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_UpdExistenciaCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_UpdExistenciaCriminalidad",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_ID_ESTADO_EXISTENCIA", datos.IdEstadoExistencia, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_UpdExistenciaCriminalidad",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadId={CriminalidadId}", resp.Operacion, datos?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdExistenciaCriminalidad");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        #endregion


        #region Métodos de Eliminación (Dapper)

        public async Task<DtoResultado<string>> P_DellIris(string CriminalidadId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DellIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_CRIMINALIDAD_ID", CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DellIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | CriminalidadId={CriminalidadId}", resp.Operacion, CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DellIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_DelIntegranteIris(string IntegranteId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DelIntegranteIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_INTEGRANTE_ID", IntegranteId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DelIntegranteIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | IntegranteId={IntegranteId}", resp.Operacion, IntegranteId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelIntegranteIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_DelDelitosIris(string DelitoId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DelDelitosIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_DELITO_ID", DelitoId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DelDelitosIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | DelitoId={DelitoId}", resp.Operacion, DelitoId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDelitosIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_DelDelInfoAdicionalIris(string InfoId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DelDelInfoAdicionalIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_INFO_ID", InfoId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DelDelInfoAdicionalIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | InfoId={InfoId}", resp.Operacion, InfoId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDelInfoAdicionalIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_DelUbicacionIris(string UbicacionId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DelUbicacionIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_UBICACION_ID", UbicacionId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DelUbicacionIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | UbicacionId={UbicacionId}", resp.Operacion, UbicacionId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelUbicacionIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        public async Task<DtoResultado<string>> P_DelDocumentoIris(string DocumentoId, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DelDocumentoIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                // Entradas
                parametros.Add("P_DOCUMENTO_ID", DocumentoId, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                parametros.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Salidas
                parametros.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_DelDocumentoIris",
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = parametros.Get<int>("P_RESULTADO");
                var mensaje = parametros.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | DocumentoId={DocumentoId}", resp.Operacion, DocumentoId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDocumentoIris");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }

            return resp;
        }

        #endregion


    }
}
