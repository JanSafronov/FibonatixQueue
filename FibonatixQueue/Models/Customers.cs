using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
    }

    public class Customer : PartialCustomer
    {
        [JsonIgnore] // Json מתעלם מהשדה "גיל" ברשימת
        public int Age { get; set; }

        [JsonConstructor]
        public Customer(string Name, DateTime Date, int Age, string Profession) :
        base(Name, Date, Profession)
        { this.Age = Age; }

        public Customer() :
        base("string", DateTime.Now, "string")
        { }

        public string Jsonify(bool include = false)
        {

            string json = JsonConvert.SerializeObject(this);

            if (include)
            {
                var temp = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
                temp.Add("Age", Age);

                JsonConvert.SerializeObject(temp);
            }

            return json;
        }

        public List<object> ToList()
        {
            return new List<object>(new object[] { this.Name, this.Date, this.Age, this.Profession });
        }
    }
}
