using Comun.Enumeraciones;
using System.Text.Json;

namespace Comun.General
{
    public class DtoRespuesta<T>
    {
        #region Propiedades
        public EstadoOperacion Codigo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public T Respuesta { get; set; }

        #endregion

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
