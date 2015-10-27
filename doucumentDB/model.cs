using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace doucumentDB
{
    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int Grade { get; set; }
        public Pet[] Pets { get; set; }
    }

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "County")]
        public string County { get; set; }
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
    }

    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        public Parent[] Parents { get; set; }
        public Child[] Children { get; set; }
        [JsonProperty(PropertyName = "Address")]
        public Address Address { get; set; }
        [JsonProperty(PropertyName = "IsRegistered")]
        public bool IsRegistered { get; set; }
    }

 
}
