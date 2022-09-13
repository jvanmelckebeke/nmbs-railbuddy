using Eindwerk.Models.Rail.Stations;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class EndPointConnection : BaseConnection
    {
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Station { get; set; }

        public override string ToString()
        {
            return $"EndPointConnection[{nameof(Station)}: {Station}, parent: {base.ToString()}]";
        }

        #region equality

        protected bool Equals(EndPointConnection other)
        {
            return Equals(Station, other.Station);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EndPointConnection) obj);
        }

        public override int GetHashCode()
        {
            return Station != null ? Station.GetHashCode() : 0;
        }

        public static bool operator ==(EndPointConnection left, EndPointConnection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EndPointConnection left, EndPointConnection right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}