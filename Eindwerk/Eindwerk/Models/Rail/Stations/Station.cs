using System;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Stations
{
    public class Station : IDtoModel, IComparable
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "standardname")]
        public string StandardName { get; set; }


        public string FormattedName => Name.Split('/')[0];


        public int CompareTo(object obj)
        {
            if (obj.GetType() != GetType()) return 1;

            var other = (Station) obj;


            return string.Compare(FormattedName, other.FormattedName, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsFilled()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(StandardName);
        }

        public override string ToString()
        {
            return $"Station[{nameof(Id)}: {Id}, " +
                   $"{nameof(Name)}: {Name}, " +
                   $"{nameof(StandardName)}: {StandardName}, {nameof(FormattedName)}: {FormattedName}]";
        }
    }
}