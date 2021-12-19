using System;
using System.Collections.Generic;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class BaseConnection
    {
        [JsonProperty("direction")] private PackedDirection _packedDirection;

        [JsonProperty("stops")] private PackedStop _packedStops;

        [JsonProperty(PropertyName = "delay")]
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan Delay { get; set; }

        [JsonProperty(PropertyName = "time")]
        [JsonConverter(typeof(RailConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("vehicleinfo")] public Vehicle Vehicle { get; set; }

        [JsonProperty("platforminfo")] public Platform Platform { get; set; }

        [JsonProperty("canceled")]
        [JsonConverter(typeof(RailConverter))]
        public bool Canceled { get; set; }

        public List<StationStop> Stops => _packedStops?.Stops;

        public bool HasStops => _packedStops != null;
        public string StopsText => HasStops ? $"{Stops.Count} stop{(Stops.Count > 1 ? "s" : "")}" : "no stops";

        public string Direction => _packedDirection?.DirectionName.Split('/')[0];


        public string Name => $"{Vehicle.FormattedName} to {Direction}";


        public override string ToString()
        {
            return
                $"BaseConnection[{nameof(Delay)}: {Delay:g}, " +
                $"{nameof(Time)}: {Time:g}, " +
                $"{nameof(Vehicle)}: {Vehicle}, " +
                $"{nameof(Platform)}: {Platform}, " +
                $"{nameof(Canceled)}: {Canceled}, " +
                $"{nameof(Stops)}: {(_packedStops == null ? "no stops" : $"{Stops.Count} stops")}, " +
                $"{nameof(Direction)}: {Direction}]";
        }

        #region equality

        protected bool Equals(BaseConnection other)
        {
            return Delay.Equals(other.Delay) && Time.Equals(other.Time) && Equals(Vehicle, other.Vehicle) &&
                   Equals(Platform, other.Platform) && Canceled == other.Canceled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseConnection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Delay.GetHashCode();
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                hashCode = (hashCode * 397) ^ (Vehicle != null ? Vehicle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Platform != null ? Platform.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Canceled.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BaseConnection left, BaseConnection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseConnection left, BaseConnection right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    internal class PackedDirection
    {
        [JsonProperty("name")] public string DirectionName { get; set; }
    }

    internal class PackedStop
    {
        [JsonProperty("stop")] public List<StationStop> Stops;
    }
}