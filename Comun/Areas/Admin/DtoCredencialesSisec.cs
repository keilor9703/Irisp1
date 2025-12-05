using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class DtoCredencialesSisec
    {
        [Required]
        [StringLength(30, ErrorMessage = "Debe tener entre 5 y 30 caracteres.", MinimumLength = 5)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Debe tener entre 5 y 50 caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}
