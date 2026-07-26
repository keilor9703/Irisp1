namespace Comun.General
{
    /// <summary>
    /// Validación de firma (magic bytes) de archivos subidos: comprueba que el contenido real
    /// corresponda a la extensión declarada, para que un ejecutable no pueda pasar renombrado
    /// como .pdf/.docx/etc. La lógica es pura para poder probarse de forma unitaria.
    /// </summary>
    public static class FirmaArchivo
    {
        /// <summary>
        /// Indica si los primeros bytes <paramref name="encabezado"/> del archivo son coherentes
        /// con la <paramref name="extension"/> declarada (incluye el punto, p. ej. ".pdf").
        /// Los formatos sin firma binaria fiable (.txt) se aceptan por extensión.
        /// </summary>
        public static bool CoincideConExtension(byte[]? encabezado, string? extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;
            extension = extension.Trim().ToLowerInvariant();

            // .txt no tiene firma binaria; se admite por extensión (ya validada aguas arriba).
            if (extension == ".txt") return true;

            if (encabezado == null || encabezado.Length < 4) return false;

            bool Empieza(params byte[] firma)
            {
                if (encabezado.Length < firma.Length) return false;
                for (int i = 0; i < firma.Length; i++)
                    if (encabezado[i] != firma[i]) return false;
                return true;
            }

            switch (extension)
            {
                case ".pdf":
                    // "%PDF"
                    return Empieza(0x25, 0x50, 0x44, 0x46);

                case ".docx":
                case ".xlsx":
                    // OOXML = contenedor ZIP: "PK\x03\x04" (o los marcadores vacío/spanned de ZIP)
                    return Empieza(0x50, 0x4B, 0x03, 0x04)
                        || Empieza(0x50, 0x4B, 0x05, 0x06)
                        || Empieza(0x50, 0x4B, 0x07, 0x08);

                case ".doc":
                case ".xls":
                    // Compound File Binary (OLE2): D0 CF 11 E0 A1 B1 1A E1
                    return Empieza(0xD0, 0xCF, 0x11, 0xE0);

                default:
                    return false;
            }
        }
    }
}
