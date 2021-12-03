using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuddiesTapPage : LoggedInPage
    {
        private List<UserProfile> Friends = new List<UserProfile>();

        public BuddiesTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
            SetupListeners();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(SetupFriendsAsync).Wait();
            Debug.WriteLine($"Profile in buddy tap page: {Profile}");
        }

        protected override void SetupVisual()
        {
            ImgQr.Source = ImageSource.FromUri(new Uri(Profile.QrCodeUrl));
            LstBuddies.ItemsSource = Friends;

            Debug.WriteLine(ImgQr.Source);
            Debug.WriteLine($"{Friends.Count} friends");
        }

        private async void SetupFriendsAsync()
        {
            Friends = await UserService.GetFriendsAsync();
        }

        private void SetupListeners()
        {
            BtnAddBuddy.Clicked += OnAddBuddyClick;
        }

        private async void OnAddBuddyClick(object sender, EventArgs e)
        {
            var action =
                await DisplayActionSheet("Add a buddy", "Cancel", null,
                    "Find by email",
                    "Scan a buddy QR code");

            // todo
        }
    }
}