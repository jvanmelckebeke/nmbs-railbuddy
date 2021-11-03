using Xamarin.Forms;

namespace Eindwerk.Tools
{
    public static class AssetHelper
    {
        /**
         * <summary>loads an image from `Eindwerk/Assets/Icons/{the icon provided}`</summary>
         *
         * <param name="icon">the name of the icon, extension included</param>
         */
        public static ImageSource GetIcon(string icon)
        {
            return ImageSource.FromResource($"Eindwerk.Assets.Icons.{icon}");
        }
    }
}