namespace Comun.Areas.Admin
{
    public class DtoInsUserRoles
    {
        public Int32 IdUsuario { get; set; }
        public Int32 IdUserRol { get; set; }
        public Int32 IdRol { get; set; }
        public string? Justificacion { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
