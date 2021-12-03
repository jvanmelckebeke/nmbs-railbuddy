using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Components;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

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
            Debug.WriteLine($"Profile in buddy tap page: {Profile}");
        }

        protected override void SetupVisual()
        {
            ImgQr.Source = ImageSource.FromUri(new Uri(Profile.QrCodeUrl));
            LstBuddies.ItemsSource = Profile.Friends;
            LstBuddyRequest.ItemsSource = Profile.FriendRequestsReceived;

            Debug.WriteLine(ImgQr.Source);
            Debug.WriteLine($"{Friends.Count} friends");
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

            if (action == "find by email")
            {
                Debug.WriteLine("not yet implemented");
                return;
            }

            if (action == "Scan a buddy QR code")
            {
                try
                {
                    MobileBarcodeScanningOptions options = new MobileBarcodeScanningOptions()
                    {
                        AutoRotate = false,
                        UseNativeScanning = true,
                        TryHarder = true
                    };

                    ZXingDefaultOverlay overlay = new ZXingDefaultOverlay()
                    {
                        TopText = "Buddy QR code scan",
                        BottomText = "Please Wait...."
                    };

                    ZXingScannerPage qrScanner = new ZXingScannerPage(options, overlay);

                    
                    await Navigation.PushModalAsync(qrScanner);

                    qrScanner.OnScanResult += (scanResult) =>
                    {
                        qrScanner.IsScanning = false;
                        AddFriend(scanResult.Text);
                        Device.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());
                    };
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        private async void AddFriend(string profileId)
        {
            Debug.WriteLine($"adding friend with profileId {profileId}");
            FriendRequest response = await UserService.AddFriendAsync(profileId);
            Debug.WriteLine(response);
        }
    }
}