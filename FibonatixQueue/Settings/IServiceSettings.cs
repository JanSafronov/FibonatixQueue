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
        string ConnectionString { get; set; }

        string Password { get; set; }
    }

    public interface ISecureServiceSettings : IServiceSettings
    {
        string Algorithm { get; set; }
    }

    public class CommonDBSettings : IServiceSettings
    {
        public string ConnectionString { get; set; }

        public string Password { get; set; }
    }

    public class SecureDBSettings : CommonDBSettings, ISecureServiceSettings
    {
        public string Algorithm { get; set; }
    }

    public class MongoDBSettings : SecureDBSettings
    {
        public string Username { get; set; }

        public string Database { get; set; }

        public string Collection { get; set; }
    }
}
