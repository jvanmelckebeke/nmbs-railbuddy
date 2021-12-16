using System;
using System.Collections.Generic;
using System.Linq;
using Eindwerk.Assets;
using Eindwerk.Models.Rail.Connections;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Tools;
using Newtonsoft.Json;
using Xamarin.Forms;

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

        public List<ViaConnection> ViaConnections => _via?.Vias;
        public List<Alert>         Alerts         => _alerts?.Alerts;

        public bool HasVias   => ViaConnections != null;
        public bool HasNoVias => ViaConnections == null;

        public DateTime DepartureTime => DepartureConnection.Time;

        public DateTime ArrivalTime => ArrivalConnection.Time;

        public TimeSpan DepartureDelay => DepartureConnection.Delay;

        public TimeSpan ArrivalDelay => ArrivalConnection.Delay;

        public string Name => $"{DepartureConnection.Station.StandardName} - {ArrivalConnection.Station.StandardName}";

        /// <summary>
        /// property that displays the duration of travel as `x hours y minutes`
        /// including removing hours if the travel lasts less than 1 hour,
        /// removing minutes if the travel lasts exactly x hours,
        /// and dealing with the s in 1 hour / 2 hours / 1 minute / 2 minutes 
        /// </summary>
        public string DurationText
        {
            get
            {
                var ret = "duration: ";
                if (Duration.Hours > 0)
                {
                    ret += $"{Duration.Hours} hour";
                    if (Duration.Hours > 1)
                    {
                        ret += "s";
                    }

                    ret += " ";
                }

                if (Duration.Minutes > 0)
                {
                    ret += $"{Duration.Minutes} minute";
                    if (Duration.Minutes > 0)
                    {
                        ret += "s";
                    }
                }

                return ret;
            }
        }

        public string NumberOfViaText => ViaConnections == null
            ? "direct"
            : $"{ViaConnections.Count} change{(ViaConnections.Count > 1 ? "s" : "")}";

        /// <summary>
        /// generates a color of the route based on:
        /// <list type="bullet">
        /// <item><description>the departing vehicle type</description></item>
        /// <item><description>the arriving vehicle type</description></item>
        /// <item><description>the duration of the travel</description></item>
        /// <item><description>the number of changes</description></item>
        /// <item><description>the departure minute</description></item>
        /// <item><description>the arriving minute</description></item>
        /// </list>
        /// </summary>
        public string RouteColor
        {
            get
            {
                // I love this technique
                string mdHash =
                    Crypto.ComputeMd5(
                        $"{Name} {DepartureConnection.Vehicle.VehicleType} {ArrivalConnection.Vehicle.VehicleType} {Duration:c} {NumberOfViaText} {DepartureTime.Minute} {ArrivalTime.Minute}");

                return "#" + mdHash.Substring(0, 6);
            }
        }

        public bool Favorite { get; set; } = false;

        public ImageSource FavoriteImageSource => Favorite ? BlackIcon.Star : BlackIcon.StarOutline;


        public override string ToString()
        {
            return
                $"Route[{nameof(Name)}: {Name}, " +
                $"{nameof(ViaConnections)}: {(_via == null ? "no vias" : ViaConnections.Count + " via connections")}, " +
                $"{nameof(RouteColor)}: {RouteColor}, " +
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