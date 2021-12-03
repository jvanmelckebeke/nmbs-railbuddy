using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models.BuddyApi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Buddies
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendRequestOverviewPage : LoggedInPage
    {
        public FriendRequestOverviewPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
        }

        protected override void SetupVisual()
        {
            base.SetupVisual();
            LstBuddyRequest.ItemsSource = Profile.FriendRequestsReceived;
        }
    }
}