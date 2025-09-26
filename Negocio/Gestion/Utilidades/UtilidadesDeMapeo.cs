using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace Negocio.Gestion.Utilidades

{

    /// <summary>

    /// Funcionalidades para mapear una estructura

    /// </summary>

    public static class UtilidadesDeMapeo

    {

        /// <summary>

        /// Convierte cualquier DataTable a una lista de la clase que le envíen

        /// </summary>

        /// <typeparam name="T"> Clase que se envia</typeparam>

        /// <param name="dt"></param>

        /// <returns>Lista convertida en la clase</returns>

        public static List<T> ConvertirDataTableAListaDto<T>(DataTable dt) where T : new()

        {

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            // Diccionario para acceder rápido a los valores por nombre de columna

            var columnas = dt.Columns.Cast<DataColumn>()

                .ToDictionary(c => c.ColumnName.Replace("_", "").ToLower(), c => c.ColumnName);

            var propiedades = typeof(T).GetProperties(flags);

            var lista = new List<T>();

            foreach (DataRow row in dt.Rows)

            {

                T instancia = new T();

                foreach (var propiedad in propiedades)

                {

                    string nombrePropNormalizado = propiedad.Name.Replace("_", "").ToLower();

                    if (columnas.TryGetValue(nombrePropNormalizado, out string nombreColumnaReal))

                    {

                        var valor = row[nombreColumnaReal];

                        if (valor != DBNull.Value)

                        {

                            try

                            {

                                var tipoDestino = Nullable.GetUnderlyingType(propiedad.PropertyType) ?? propiedad.PropertyType;

                                var valorConvertido = Convert.ChangeType(valor, tipoDestino);

                                propiedad.SetValue(instancia, valorConvertido);

                            }

                       

                                catch (Exception ex)
                                {
                                Debug.WriteLine($"❌ Error mapeando columna: '{nombreColumnaReal}' → propiedad: '{propiedad.Name}'");
                                Debug.WriteLine($"   Valor: {valor} (tipo: {valor.GetType()}) → Tipo destino: {propiedad.PropertyType}");
                                Debug.WriteLine($"   Excepción: {ex.Message}");
                            }

                        }

                    }

                }

                lista.Add(instancia);

            }

            return lista;

        }

    }

}
