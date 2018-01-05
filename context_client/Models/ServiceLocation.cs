using System.ComponentModel.DataAnnotations;
using System;

namespace context_client.Models
{
    public class ServiceLocation
    {
        [Key]
        public int ServiceId { get; private set; }
        public string Server { get; private set; }
        public string DatabaseNm { get; private set; }
        public bool IsServing { get; private set; }

        public ServiceLocation()
        {
        }

        public ServiceLocation(int serviceId, string server, string database, bool isServing)
        {
            ServiceId = serviceId;
            Server = server;
            DatabaseNm = database;
            IsServing = isServing;
        }

        public override string ToString()
        {
            return string.Format("ServiceLocation [ {0} {1} {2} {3} ]"
                , ServiceId, Server, DatabaseNm, IsServing);
        }
    }

	public class FeatureMeta
	{
		[Key]
		public int ID { get; set; }
		public string Name { get; set; }
		public string KeyName { get; set; }
		public string Description { get; set; }
		public string Registrant { get; set; }
		public DateTime CreateDate { get; set; }

		public FeatureMeta()
		{
		}
	}

}
