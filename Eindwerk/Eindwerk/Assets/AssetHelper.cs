using Xamarin.Forms;

namespace Eindwerk.Assets
{
    public static class AssetHelper
    {
        public const string SourcesPath = "Eindwerk.Assets.Sources";

        /**
         * <summary>loads an image from `Eindwerk/Assets/Icons/{the icon provided}`</summary>
         * <param name="icon">the relative path of the icon from `Eindwerk/Assets/Icons/` separated by `.`, extension included</param>
         */
        public static ImageSource GetIcon(string icon)
        {
            return ImageSource.FromResource($"{SourcesPath}.Icons.{icon}");
        }
    }
}