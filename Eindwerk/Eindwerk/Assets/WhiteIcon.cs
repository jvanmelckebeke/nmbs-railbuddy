using Xamarin.Forms;
using static Eindwerk.Tools.AssetHelper;

namespace Eindwerk.Assets
{
    public static class WhiteIcon
    {
        public static ImageSource GetWhiteIcon(string icon)
        {
            return GetIcon($"White.{icon}");
        }


        public static ImageSource Check => GetWhiteIcon("check.png");

        public static ImageSource ChevronRight => GetWhiteIcon("chevron-right.png");

        public static ImageSource Close => GetWhiteIcon("close.png");

        public static ImageSource Logout => GetWhiteIcon("logout.png");

        public static ImageSource Delete => GetWhiteIcon("delete.png");
        public static ImageSource Navigation => GetWhiteIcon("navigation.png");
    }
}