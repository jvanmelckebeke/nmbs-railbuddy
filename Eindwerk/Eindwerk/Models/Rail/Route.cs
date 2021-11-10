using System;
using System.Collections.Generic;
using Eindwerk.Models.Rail.Connections;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail
{
    public class Route
    {
        [JsonProperty("departure")] public DepartureConnection DepartureConnection { get; set; }

        [JsonProperty("arrival")] public ArrivalConnection ArrivalConnection { get; set; }

        [JsonProperty("duration")]
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan Duration { get; set; }

        [JsonProperty("vias")] private PackedVia _via;

        [JsonProperty("alerts")] private PackedAlerts _alerts;

        public List<ViaConnection> ViaConnections => _via.Vias;
        public List<Alert> Alerts => _alerts.Alerts;

        public DateTime DepartureTime => DepartureConnection.Time;

        public DateTime ArrivalTime => ArrivalConnection.Time;

        public TimeSpan DepartureDelay => DepartureConnection.Delay;

        public TimeSpan ArrivalDelay => ArrivalConnection.Delay;

        public override string ToString()
        {
            return
                $"Route[{nameof(ViaConnections)}: {(_via == null ? "no vias" : ViaConnections.Count + " via connections")}, " +
                $"{nameof(DepartureConnection)}: {DepartureConnection}, " +
                $"{nameof(ArrivalConnection)}: {ArrivalConnection}, " +
                $"{nameof(Duration)}: {Duration:g}, " +
                $"{nameof(Alerts)}: {(_alerts == null ? "no alerts" : Alerts.Count + " alerts")}, " +
                $"{nameof(DepartureTime)}: {DepartureTime:u}, " +
                $"{nameof(ArrivalTime)}: {ArrivalTime:u}, " +
                $"{nameof(DepartureDelay)}: {DepartureDelay:g}, " +
                $"{nameof(ArrivalDelay)}: {ArrivalDelay:g}]";
        }
    }

    internal class PackedAlerts
    {
        [JsonProperty("alert")] public List<Alert> Alerts;
    }

    internal class PackedVia
    {
        [JsonProperty("via")] public List<ViaConnection> Vias;
    }
}