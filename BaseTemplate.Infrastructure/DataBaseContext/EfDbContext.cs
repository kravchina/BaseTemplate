using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseTemplate.Infrastructure.DataBaseContext
{
    public class EfDbContext : DbContext
    {
        public EfDbContext()
            : base("DefaultConnection")
        {

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EfDbContext>());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}
