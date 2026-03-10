using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Data.Models
{
    public class LogAccion
    {
        public int Id { get; set; }

        public string IdEmpleado { get; set; }

        public string Accion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}