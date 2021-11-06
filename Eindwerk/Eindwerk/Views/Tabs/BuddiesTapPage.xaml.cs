using Eindwerk.Models;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuddiesTapPage : NetworkDependentPage
    {
        private Tokens _tokens;

        public BuddiesTapPage(Tokens tokens)
        {
            InitializeComponent();
            _tokens = tokens;
        }
    }
}