using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;

namespace SGHR.Data.Models
{
    public class Recepcionista
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(20)]
        public string IdEmpleado { get; set; }

        [Required]
        [StringLength(20)]
        public string Turno { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailCorporativo { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool EsActivo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
