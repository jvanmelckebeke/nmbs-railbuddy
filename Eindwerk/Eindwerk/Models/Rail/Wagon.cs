using System;
using System.Collections.Generic;
using Eindwerk.Models.BuddyApi.Friends;
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

        public List<Friend> FriendsInWagon { get; set; } = new List<Friend>();

        public override string ToString()
        {
            return $"{nameof(ModelName)}: {ModelName}, {nameof(ModelUrl)}: {ModelUrl}";
        }
    }
}