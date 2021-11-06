using System;
using Newtonsoft.Json;

namespace Eindwerk.Models
{
    public class Station : IDtoModel, IComparable
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "standardname")]
        public string StandardName { get; set; }

        public bool IsFilled()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(StandardName);
        }

        public override string ToString()
        {
            return $"Station[Id='{Id}', Name='{Name}', StandardName='{StandardName}']";
        }

        
        public int CompareTo(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return 1;
            }

            Station other = (Station) obj;


            return string.Compare(StandardName, other.StandardName, StringComparison.OrdinalIgnoreCase);
        }
    }
}