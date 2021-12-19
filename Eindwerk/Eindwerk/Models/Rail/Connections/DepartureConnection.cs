using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class DepartureConnection : EndPointConnection
    {
        [JsonProperty("left")]
        [JsonConverter(typeof(RailConverter))]
        public bool Departed { get; set; }

        public override string ToString()
        {
            return $"DepartureConnection[{nameof(Departed)}: {Departed}, parent: {base.ToString()}]";
        }

        #region equality

        protected bool Equals(DepartureConnection other)
        {
            return Departed == other.Departed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DepartureConnection) obj);
        }

        public override int GetHashCode()
        {
            return Departed.GetHashCode();
        }

        public static bool operator ==(DepartureConnection left, DepartureConnection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DepartureConnection left, DepartureConnection right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}