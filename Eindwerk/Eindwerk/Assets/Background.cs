using Xamarin.Forms;
using static Eindwerk.Assets.AssetHelper;

namespace Eindwerk.Assets
{
    public static class Background
    {
        public static ImageSource Error => GetBackground("fail.png");

        public static ImageSource NoConnection => GetBackground("no-connection.gif");

        public static ImageSource GetBackground(string background)
        {
            return ImageSource.FromResource($"{SourcesPath}.Background.{background}");
        }
    }
}