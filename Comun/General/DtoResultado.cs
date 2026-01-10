using System.Text.Json.Serialization;

namespace Comun.General
{
    /// <summary>
    /// Respuesta del Sistema al ejecutarlos eventos
    /// </summary>
    public class DtoResultado<T>
    {

        private string? _Mensaje;
        private Int64 _IdRespuesta;
        private string? _Operacion;
        private Int64 _CodigoExito;
        private T _Data;

        /// <summary>
        /// Codigo de exito que se presenta en el evento, representa el ID de almacenamientoen base de datos
        /// </summary>
        [JsonPropertyName("IdRespuesta")]
        public Int64 IdRespuesta
        {
            get
            {
                return _IdRespuesta;
            }
            set
            {
                _IdRespuesta = value;
            }
        }


        /// <summary>
        /// Codigo de exito que se presenta en el evento, representa el ID de almacenamientoen base de datos
        /// </summary>
        [JsonPropertyName("CodigoExito")]
        public Int64 CodigoExito
        {
            get
            {
                return _CodigoExito;
            }
            set
            {
                _CodigoExito = value;
            }
        }
        /// <summary>
        /// Descripción mensaje que se presenta en el evento
        /// </summary>
        [JsonPropertyName("Mensaje")]
        public string? Mensaje
        {
            get
            {
                return _Mensaje;
            }
            set
            {
                _Mensaje = value;
            }
        }
        /// <summary>
        /// Operación que se presenta en el evento
        /// </summary>
        [JsonPropertyName("Operacion")]
        public string? Operacion
        {
            get
            {
                return _Operacion;
            }
            set
            {
                _Operacion = value;
            }
        }

        /// <summary>
        /// Colección de datos de la respuesta
        /// </summary>
        [JsonPropertyName("Data")]
        public T Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

       
    }
}
