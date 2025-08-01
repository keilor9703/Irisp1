using System.Data;
using System.Reflection;

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
        public static List<T> ConvertirDataTableAListaDto<T>(DataTable dt)
        {
            const BindingFlags bandera = BindingFlags.Public | BindingFlags.Instance;
            var NombreDeLasColumnas = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName.ToUpper())
                .ToList();
            var PropiedadesDelObjeto = typeof(T).GetProperties(bandera);
            var ListaDto = dt.AsEnumerable().Select(datosFila =>
            {
                var crearInstancia = Activator.CreateInstance<T>();

                foreach (var propiedad in PropiedadesDelObjeto.Where(propiedades => NombreDeLasColumnas.Contains(propiedades.Name) && datosFila[propiedades.Name] != DBNull.Value))
                {
                    propiedad.SetValue(crearInstancia, datosFila[propiedad.Name.ToUpper()], null);
                }
                return crearInstancia;
            }).ToList();



            return ListaDto;
        }
    }
}
