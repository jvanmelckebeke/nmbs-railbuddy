using System;
using Xamarin.Forms;

namespace Eindwerk.Models.Rail
{
    public class Wagon
    {
        public string ModelName { get; set; }

        public string ModelUrl { get; set; }

        public ImageSource Preview
        {
            get
            {
                var uri = new Uri($"https://www.beluxtrains.net{ModelUrl}");
                return ImageSource.FromUri(uri);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ModelName)}: {ModelName}, {nameof(ModelUrl)}: {ModelUrl}";
        }
    }
}