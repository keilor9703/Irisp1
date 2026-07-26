namespace Comun.General
{
    /// <summary>
    /// Utilidades para resolver rutas de archivos provenientes de entrada del usuario
    /// confinándolas dentro de una carpeta base, evitando ataques de path traversal
    /// (por ejemplo <c>..\..\Windows\win.ini</c>). La lógica es pura y sin dependencias
    /// para poder probarse de forma unitaria.
    /// </summary>
    public static class RutaArchivoSegura
    {
        /// <summary>
        /// Combina <paramref name="rutaBase"/> con la <paramref name="rutaRelativa"/> recibida
        /// del cliente y valida que el resultado quede dentro de la carpeta base. Devuelve la
        /// ruta absoluta segura, o <c>null</c> si la entrada es inválida o intenta escapar de la base.
        /// </summary>
        public static string? Resolver(string? rutaBase, string? rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaBase) || string.IsNullOrWhiteSpace(rutaRelativa))
                return null;

            // Rechazar rutas absolutas o con raíz de unidad/UNC en la entrada del cliente:
            // solo se admiten segmentos relativos que cuelguen de la base.
            var relativaNormalizada = rutaRelativa.Replace('/', '\\').TrimStart('\\');

            if (Path.IsPathRooted(relativaNormalizada) || relativaNormalizada.Contains(':'))
                return null;

            string baseCompleta;
            string rutaCompleta;
            try
            {
                baseCompleta = Path.GetFullPath(rutaBase);
                rutaCompleta = Path.GetFullPath(Path.Combine(baseCompleta, relativaNormalizada));
            }
            catch
            {
                return null;
            }

            // Confinamiento: la ruta resuelta debe empezar por la base (comparando con
            // separador final para que "C:\baseX" no pase como si estuviera dentro de "C:\base").
            var baseConSeparador = baseCompleta.EndsWith(Path.DirectorySeparatorChar)
                ? baseCompleta
                : baseCompleta + Path.DirectorySeparatorChar;

            if (!rutaCompleta.StartsWith(baseConSeparador, StringComparison.OrdinalIgnoreCase))
                return null;

            return rutaCompleta;
        }
    }
}
