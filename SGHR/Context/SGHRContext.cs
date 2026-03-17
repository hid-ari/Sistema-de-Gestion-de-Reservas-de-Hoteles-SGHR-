using Microsoft.EntityFrameworkCore;
using SGHR.Data.Models;


namespace SGHR.Data.Context
{
    public partial class SGHRContext : DbContext

    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options)
        {
            
        }
        #region 'Pisos Entitites'
        public DbSet<Pisos> Pisos { get; set; }
        #endregion

    }
}
