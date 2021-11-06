using Eindwerk.Models;
using Eindwerk.Tools;
using Eindwerk.Views.Tabs;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using TabbedPage = Xamarin.Forms.TabbedPage;

namespace Eindwerk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage(Tokens tokens)
        {
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            UnselectedTabColor = Color.FromHex("#0169b2").AddLuminosity(-0.5);
            SelectedTabColor = Color.FromHex("#0169b2");
            ;

            Children.Add(new NavigationPage(new HomeTabPage(tokens))
                {Title = "Home", IconImageSource = AssetHelper.GetIcon("train.png")});
            Children.Add(new NavigationPage(new BuddiesTapPage(tokens))
                {Title = "Buddies", IconImageSource = AssetHelper.GetIcon("buddies.png")});
            Children.Add(new NavigationPage(new SeatTapPage(tokens))
                {Title = "Seat", IconImageSource = AssetHelper.GetIcon("seat.png")});
            Children.Add(new ProfileTapPage(tokens)
                {Title = "Profile", IconImageSource = AssetHelper.GetIcon("account.png")});

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}