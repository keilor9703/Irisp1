using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.General;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.General
{
    public class Unidades : IUnidades
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public Unidades(IConfiguration iConfiguration,
                                ILogger<DbAdministracion> logger
                                )
        {
            _iConfiguration = iConfiguration;
            strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
        }
        #endregion

        #region Metodos de Consulta

public async Task<DtoResultado<List<UnidadesDTO>>> ConsultarUnidades()
        {
            var resultados = new List<UnidadesDTO>();
            var resultadoDto = new DtoResultado<List<UnidadesDTO>>();

            try
            {
                using (var connection = new OracleConnection(strConexionIris_Test))
                {
                    using (var command = new OracleCommand("ConsultarUnidades", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new OracleParameter("p_resultados", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                resultados.Add(new UnidadesDTO
                                {
                                    CONSECUTIVO = reader.GetInt32(0),
                                    SIGLA_PAPA = reader.GetString(1),
                                    DESCRIPCION_DEPENDENCIA = reader.GetString(2)
                                });
                            }
                        }
                    }
                }

                //resultadoDto.Resultado = resultados;
                //resultadoDto.Exito = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar unidades");
                //resultadoDto.Exito = false;
                resultadoDto.Mensaje = "Error al consultar unidades";
            }

            return resultadoDto;
        }
        #endregion
    }
}