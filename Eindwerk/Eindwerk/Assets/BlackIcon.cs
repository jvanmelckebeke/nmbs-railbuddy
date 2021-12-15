﻿using Xamarin.Forms;
using static Eindwerk.Tools.AssetHelper;

namespace Eindwerk.Assets
{
    public static class BlackIcon
    {
        private static ImageSource GetBlackIcon(string icon)
        {
            return GetIcon($"Black.{icon}");
        }
        
        public static ImageSource Account => GetBlackIcon("account.png");
        public static ImageSource Buddies => GetBlackIcon("buddies.png");

        public static ImageSource ChevronLeft => GetBlackIcon("chevron-left.png");
        public static ImageSource ChevronRight => GetBlackIcon("chevron-right.png");

        public static ImageSource Plus => GetBlackIcon("plus.png");

        public static ImageSource Seat => GetBlackIcon("seat.png");
        public static ImageSource Train => GetBlackIcon("train.png");

        public static ImageSource Star => GetBlackIcon("star.png");
        public static ImageSource StarOutline => GetBlackIcon("star-outline.png");

        public static ImageSource DotsVertical => GetBlackIcon("dots-vertical.png");
        public static ImageSource TransitConnection => GetBlackIcon("transit-connection.png");
    }
}