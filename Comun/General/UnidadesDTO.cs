using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.General
{
    public class UnidadesDTO
    {
        public bool FUERZA { get; set; }
        public decimal CONSECUTIVO { get; set; }
        public string DESCRIPCION_DEPENDENCIA { get; set; }
        public string VIGENTE { get; set; }
        public string NIVEL_JERARQUICO { get; set; }
        public decimal NIVEL_ORGANIZACIONAL { get; set; }
        public string NIVEL_FUNCIONAL { get; set; }
        public decimal? ORDENAMIENTO { get; set; }
        public decimal? NIT { get; set; }
        public string DIRECCION { get; set; }
        public string EMAIL { get; set; }
        public string ORDEN_PUBLICO { get; set; }
        public string PRIMA_CLIMA { get; set; }
        public string PARTIDA_DE_ALIMENTACION { get; set; }
        public string NOMINA { get; set; }
        public string INDICATIVO_MICROONDAS { get; set; }
        public DateTime? FECHA_ACTIVACION { get; set; }
        public DateTime? FECHA_DESACTIVACION { get; set; }
        public decimal ID_SIGLA { get; set; }
        public decimal TIUN_CODIGO { get; set; }
        public decimal DEPE_CODIGO { get; set; }
        public decimal? DEPE_CODIGO_PERTENECE { get; set; }
        public decimal? DEPE_CODIGO_CORRESPONDER { get; set; }
        public decimal? EMPL_UNDE_CONSECUTIVO { get; set; }
        public bool? EMPL_UNDE_FUERZA { get; set; }
        public decimal? EMPL_CONSECUTIVO { get; set; }
        public decimal REGI_CODIGO { get; set; }
        public decimal? BARRI_CODIGO { get; set; }
        public int? BARRI_LUGE_CODIGO { get; set; }
        public int LUGE_CODIGO { get; set; }
        public bool GUCO_FUERZA { get; set; }
        public decimal GUCO_CODIGO { get; set; }
        public bool? UNDE_FUERZA { get; set; }
        public decimal? UNDE_CONSECUTIVO { get; set; }
        public string CREADO_POR { get; set; }
        public DateTime FECHA_CREACION { get; set; }
        public string MAQUINA_CREACION { get; set; }
        public string ACTUALIZADO_POR { get; set; }
        public DateTime? FECHA_ACTUALIZA { get; set; }
        public string MAQUINA_ACTUALIZA { get; set; }
        public byte? DIFICULTAD { get; set; }
        public string TIPO_PAGO { get; set; }
        public string SIGLA_PAPA { get; set; }
        public decimal? CODIGO_DELEGACION { get; set; }
        public decimal NIVEL_RIESGO { get; set; }
        public byte CONDICION_PARTIDA_ALIMENTACION { get; set; }
        public decimal? TELEFONO { get; set; }
        public string CATEGORIA_UNIDAD { get; set; }
        public string CENTRO_DE_COSTOS { get; set; }
        public string PRIMA_CARABINERO { get; set; }
        public string SIGLA_DEPENDE { get; set; }
        public string SIGLA_FISICA { get; set; }
        public string SUBMARINISTA { get; set; }
        public string ZONA_CRITICA { get; set; }
        public decimal? TIPO_DOTACION { get; set; }
        public string SIGLA_SIVICC { get; set; }
        public string MOTIVO_OPERACION { get; set; }
        public string TELEFONO_IP { get; set; }
        public decimal? DISP_ID_DISPOSICION { get; set; }
        public string NUMERO_DISPOSICION { get; set; }
        public DateTime? FECHA_DISPOSICION { get; set; }
        public string MUNICIPIO { get; set; }
        public string TIPO_DESCRIPCION { get; set; }
        public int CODIGO_PADRE { get; set; }
        public string DEPARTAMENTO_PADRE { get; set; }
        public string TIPO_PADRE { get; set; }
        public int CODIGO_DEPARTAMENTO { get; set; }
        public string DEPARTAMENTO { get; set; }
        public string TIPO { get; set; }
        public string ZONA { get; set; }
        public string CODIGO_DANE { get; set; }
        public string DESC_REGIONAL { get; set; }
        public decimal? COD_REGIONAL { get; set; }

        public string DEPENDENCIA_SIGLA { get => $"{SIGLA_PAPA} - {DESCRIPCION_DEPENDENCIA}"; }
        public string MUNICIPIO_DEPT { get => $"{MUNICIPIO} - {DEPARTAMENTO}"; }
    }
}
