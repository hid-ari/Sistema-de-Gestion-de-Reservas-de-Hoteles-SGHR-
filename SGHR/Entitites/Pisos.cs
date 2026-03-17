using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Data.Models
{
    public class Pisos
    {
        public int PisosId { get; set; }
        public string NumeroPiso { get; set; } 
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }

    }
}
