using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace multiple_dbcontext_web
{
    public class CustomDbContext : DbContext
    {
        public int ID { get; set; } = -1;
        public CustomDbContext(DbContextOptions options, int id ) 
            : base(options)
        {
            ID = id;
        }
    }


    //MEMO: singleton call.
    public class DbContextPool
    {
        // int(key) : serviceID, DbContext(value) : class obj
        public Dictionary<int, DbContext> PoolDict { get; set; } = new Dictionary<int, DbContext>();


        public DbContextPool(IServiceCollection services)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("TEST2");
            PoolDict.Add(2, new CustomDbContext(builder.Options, 2));

            builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("TEST3");
            PoolDict.Add(3, new CustomDbContext(builder.Options, 3 ));
        }

        private DbContext CreateContext(int serviceId)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase(string.Format("{0}", serviceId));
            return new CustomDbContext(builder.Options, serviceId);
        }
    }
}
