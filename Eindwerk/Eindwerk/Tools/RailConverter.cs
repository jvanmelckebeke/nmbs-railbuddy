using System;
using System.Linq;
using Newtonsoft.Json;

namespace Eindwerk.Tools
{
    public class RailConverter : JsonConverter
    {
        private readonly Type[] _canConvertTypes = {typeof(bool), typeof(DateTime), typeof(TimeSpan)};

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("this converter can not write json");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var other = reader.Value.ToString();
            // Debug.WriteLine($"value: {other} should become of type {existingValue.GetType()}");
            if (objectType == typeof(bool)) return other.Equals("1");

            if (objectType == typeof(DateTime))
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(long.Parse(other));

            if (objectType == typeof(TimeSpan)) return TimeSpan.FromSeconds(long.Parse(other));

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return _canConvertTypes.Contains(objectType);
        }
    }
}