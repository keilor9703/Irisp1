using Comun.Areas.Admin;
using Comun.Areas.Clientes;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Clientes;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Clientes
{
    public class DbClientes : IDbClientes
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string strConexionIris_Test;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbClientes(IConfiguration iConfiguration,
                          ILogger<DbClientes> logger
                         )
        {
            _iConfiguration = iConfiguration;
            strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _logger = logger;
        }
        #endregion

        #region Métodos de Consulta

        public async Task<DtoResultado<List<DtoKardex>>> F_GetKardex(Int64 V_Identificacion)
        {
            DataTable resultado = new();
            List<DtoKardex> retorno = new();
            DtoResultado<List<DtoKardex>> resp = new();

            using var Conexion = new OracleConnection(strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_Clientes.F_GetKardex";
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
                        var domi = new DtoKardex()
                        {
                            Apellidos = reader.GetString(1),
                            Nombres = reader.GetString(2),
                            FechaNace = reader.GetString(3),
                            Dto = reader.GetString(4),
                            Lugar = reader.GetString(5),
                            Genero = reader.GetString(6),
                            Direccion = reader.GetString(7),
                            Unidad = reader.GetString(8),
                            Dependencia = reader.GetString(9),
                            Observaciones = reader.GetString(10)

                        };
                        retorno.Add(domi);
                    }

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetKardex";
                        resp.Data = retorno;
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





                //if (Conexion.State == ConnectionState.Open)
                //{
                //    resultado.Load(await objCommand.ExecuteReaderAsync());

                //    if (resultado.Rows.Count > 0)
                //    {
                //        foreach (DataRow fila in resultado.Rows)
                //        {
                //            retorno.Add(new DtoKardex
                //            {
                //                Identificacion = Convert.ToInt64(fila["Identificacion"].ToString()),
                //                Apellidos = fila["Apellidos"].ToString(),
                //                Nombres = fila["Nombres"].ToString(),
                //                FechaNace = fila["FechaNace"].ToString(),
                //                Dto = fila["Dto"].ToString(),
                //                Lugar = fila["Lugar"].ToString(),
                //                Genero = fila["Genero"].ToString(),
                //                Direccion = fila["Direccion"].ToString(),
                //                Unidad = fila["Unidad"].ToString(),
                //                Dependencia = fila["Dependencia"].ToString(),
                //                Observaciones = fila["Observaciones"].ToString()
                //            });
                //        }

                //        resp.IdRespuesta = 1;
                //        resp.Mensaje = "Consulta Exitosa";
                //        resp.Operacion = "F_GetKardex";
                //        resp.Data = retorno;
                //    }
                //    else
                //    {
                //        resp.IdRespuesta = 0;
                //        resp.Mensaje = "No se encontraron datos, debe seleccionar rango de fechas";
                //        resp.Operacion = "0";
                //    }

                //    Conexion.Close();
                //    Conexion.Dispose();
                //    objCommand.Connection.Close();
                //}
                //else
                //{
                //    resp.IdRespuesta = 0;
                //    resp.Mensaje = "No se pudo establecer la conexión con la base de datos";
                //    resp.Operacion = "0";
                //}

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando pk_Clientes.F_GetKardex " + e);

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

        #region Métodos de Inserción
        public async Task<DtoResultado<Int32>> P_InsUdpKardex(DtoKardex Obj, Int32 V_Usuario, string V_Maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_Clientes.P_InsUdpKardex";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_IdKardex", OracleDbType.Int32, ParameterDirection.Input).Value = Obj.IdKardex;
                objCommand.Parameters.Add("P_Apellidos", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Apellidos;
                objCommand.Parameters.Add("P_Nombres", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Nombres;
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Int64, ParameterDirection.Input).Value = Obj.Identificacion;
                objCommand.Parameters.Add("P_Fecha_Nace", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.FechaNace;
                objCommand.Parameters.Add("P_IdDto", OracleDbType.Int32, ParameterDirection.Input).Value = Obj.IdDto;
                objCommand.Parameters.Add("P_IdLugar", OracleDbType.Int32, ParameterDirection.Input).Value = Obj.IdLugar;
                objCommand.Parameters.Add("P_Direccion", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Direccion;
                objCommand.Parameters.Add("P_IdGenero", OracleDbType.Int32, ParameterDirection.Input).Value = Obj.IdGenero;
                objCommand.Parameters.Add("P_Unidad", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Unidad;
                objCommand.Parameters.Add("P_Dependencia", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Dependencia;
                objCommand.Parameters.Add("P_Observaciones", OracleDbType.Varchar2, ParameterDirection.Input).Value = Obj.Observaciones;

                objCommand.Parameters.Add("P_Usuario", OracleDbType.Int64, ParameterDirection.Input).Value = V_Usuario;
                objCommand.Parameters.Add("P_Maquina", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Maquina;

                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    await objCommand.ExecuteNonQueryAsync();
                    resp.IdRespuesta = Int32.Parse(objCommand.Parameters["P_Resultado"].Value.ToString());
                    resp.Mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();
                    resp.Operacion = "P_InsUdpKardex";
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
                _logger.LogWarning("Error al Insertar : Pk_Clientes.P_InsUdpKardex " + e);

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


        #region Métodos de Actualización

        #endregion

    }
}
