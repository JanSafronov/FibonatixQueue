using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FibonatixQueue.Settings
{
    public interface IServiceSettings
    {
        string connectionString { get; set; }

        string password { get; set; }
    }

    public class RedisDBSettings : IServiceSettings
    {
        public string connectionString { get; set; }

        public string password { get; set; }
    }

    public class AzureDBSettings : IServiceSettings
    {
        public string connectionString { get; set; }

        public string password { get; set; }
    }

    public class MongoDBSettings : IServiceSettings
    {
        public string connectionString { get; set; }

        public string password { get; set; }

        public string database { get; set; }

        public string collection { get; set; }
    }
}
