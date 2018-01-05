using Microsoft.EntityFrameworkCore;
using context_client.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace context_client.Contexts
{
    public class ServiceContentsContext : DbContext
    {
        public DbSet<ServiceLocation> ServiceLocationSet { get; set; }

        /*public ServiceContentsContext(DbContextOptions<ServiceContentsContext> options)
            : base(options)
        {
        }*/

        public List<ServiceLocation> GetAll()
        {
            var a = ServiceLocationSet.FromSql("CALL GetAllServiceLocation()").ToList();
            // var a = ServiceLocationSet.FromSql("CALL getallservicelocation").ToList();


            return a;
        }

        public async Task<List<ServiceLocation>> GetAllAsync()
        {
            var a = await ServiceLocationSet.FromSql("CALL GetAllServiceLocation()").ToListAsync();
            return a;
        }

		public void GetMultipleResultSets()
		{
			this.Database.OpenConnection();

			using (var cnn = Database.GetDbConnection())
			{
				var cmm = cnn.CreateCommand();
				cmm.CommandType = System.Data.CommandType.StoredProcedure;
				cmm.CommandText = "GetAllServiceLocation";
				cmm.Connection = cnn;
				using (var reader = cmm.ExecuteReader())
				{
					var dataSource = reader;

					List<ServiceLocation> locations = new List<ServiceLocation>();

					List<FeatureMeta> features = new List<FeatureMeta>();

					if (dataSource != null && dataSource.HasRows)
					{
						while (reader.Read())
						{
							var location = new ServiceLocation(
								reader.GetInt32(0),
								reader.GetString(1),
								reader.GetString(2),
								reader.GetBoolean(3)
							);
							locations.Add(location);
						}

						//MEMO: [gordonlee]. 첫번째 select문을 읽은 후, 다음 select 문을 읽는다. (2번째 result set)
						reader.NextResult();	

						while (reader.Read())
						{
							var featureMeta = new FeatureMeta()
							{
								ID = reader.GetInt32(0),
								Name = reader.IsDBNull(1) ? "" : reader.GetString(1), 
								KeyName = reader.IsDBNull(2) ? "" : reader.GetString(2), 
								Description = reader.IsDBNull(3) ? "" : reader.GetString(3), 
								Registrant = reader.IsDBNull(4) ? "" : reader.GetString(4), 
								CreateDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5)
							};

							features.Add(featureMeta);
						}
					}

					/*
					if (reader != null && reader.HasRows)
					{
						reader.
						var location = new ServiceLocation(
								(int)reader.GetValue(0),
								(string)reader.GetValue(1),
								(string)reader.GetValue(2),
								(bool)reader.GetValue(3)
							);
					}
					*/

				}
			}
			// var query = ServiceLocationSet.FromSql("Call GetAllSErviceLocation()")
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=live-openapi-testlab-cluster.cluster-cn1i0gabzxb5.ap-northeast-1.rds.amazonaws.com;database=contents;userid=admin;pwd=djemals!2;port=3306;sslmode=none;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceLocation>().ToTable("ServiceLocation");
        }

    }
}
