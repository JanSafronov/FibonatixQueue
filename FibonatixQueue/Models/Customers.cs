using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using StackExchange.Redis.Profiling;

namespace FibonatixQueue.Models
{

    public interface IPartialCustomer
    {
        string Name { get; set; }

        DateTime Date { get; set; }

        string Profession { get; set; }
    }

    public class PartialCustomer : IPartialCustomer
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Profession { get; set; }

        public PartialCustomer(string Name, DateTime Date, string Profession)
        {
            this.Name = Name;
            this.Date = Date;
            this.Profession = Profession;
        }

        public void Listify()
        {
            new List<object>(new Dictionary({ "Name": Name });
        }
    }

    public class Customer : PartialCustomer
    {
        public int Age { get; set; }

        public Customer(string Name, DateTime Date, int Age, string Profession) :
            base(Name, Date, Profession) { this.Age = Age; }
    }
}
