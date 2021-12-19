using System;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail
{
    public class Alert
    {
        [JsonProperty("endTime")] [JsonConverter(typeof(RailConverter))]
        public DateTime EndTime;

        [JsonProperty("startTime")] [JsonConverter(typeof(RailConverter))]
        public DateTime StartTime;

        [JsonProperty("header")] public string Header { get; set; }
        [JsonProperty("description")] public string Description { get; set; }

        public override string ToString()
        {
            return $"Alert[StartTime: {StartTime:u}, " +
                   $"EndTime: {EndTime:u}, " +
                   $"Header: {Header}," +
                   $" Description: {Description}]";
        }
    }
}