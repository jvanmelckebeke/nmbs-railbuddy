using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Views.Buddies;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuddiesTapPage : LoggedInPage
    {
        public BuddiesTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
            SetupListeners();
        }

        protected override void SetupVisual()
        {
            ImgQr.Source = ImageSource.FromUri(new Uri(Profile.QrCodeUrl));
            LstBuddies.ItemsSource = Profile.Friends;

            LblFriendRequests.Text = $"{Profile.FriendRequestsReceived.Count} Friend requests";
            FrFriendRequests.IsVisible = Profile.FriendRequestsReceived.Count > 0;
        }

        private void SetupListeners()
        {
            BtnAddBuddy.Clicked += OnAddBuddyClick;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnGoToFriendRequestsTapped;

            FrFriendRequests.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnGoToFriendRequestsTapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FriendRequestOverviewPage(Tokens), true);
        }

        private async void OnAddBuddyClick(object sender, EventArgs e)
        {
            try
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseNativeScanning = true,
                    TryHarder = true
                };

                var overlay = new ZXingDefaultOverlay
                {
                    TopText = "Buddy QR code scan",
                    BottomText = "Please Wait...."
                };

                var qrScanner = new ZXingScannerPage(options, overlay);


                await Navigation.PushModalAsync(qrScanner);

                qrScanner.OnScanResult += async scanResult =>
                {
                    qrScanner.IsScanning = false;
                    await HandleApi(async () => await AddFriend(scanResult.Text));
                    Device.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());
                };
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task AddFriend(string profileId)
        {
            UserProfile profile = await UserService.GetUserProfileAsync(profileId);

            if (profile == null)
                return;

            async void ConfirmFriend(bool confirmed)
            {
                await HandleApi(async () =>
                {
                    if (confirmed)
                    {
                        Debug.WriteLine($"adding friend with profileId {profileId}");
                        BasicFriendRequestStatus response = await UserService.RequestFriendAsync(profileId);
                        Debug.WriteLine(response);
                        UserDialogs.Instance.Toast($"friend request sent to {profile.Username}");
                    }
                });
            }

            var config = new ConfirmConfig
            {
                Title = $"Add {profile.Username}?",
                Message = $"Are you sure you want to add {profile.Username} ({profile.Email})?",
                OkText = "Yes",
                CancelText = "No",
                OnAction = ConfirmFriend
            };

            UserDialogs.Instance.Confirm(config);
        }

        private void OnDeleteHandler(object sender, EventArgs e)
        {
            var btn = (Button) sender;

            var friendCtx = (Friend) btn.BindingContext;

            async void ConfirmRemoveFriend(bool confirm)
            {
                await HandleApi(async () =>
                {
                    if (confirm)
                    {
                        Debug.WriteLine($"removing friend {friendCtx.Username} ({friendCtx.Email})");
                        await UserService.DeleteFriendAsync(friendCtx.UserId.ToString());
                        UserDialogs.Instance.Toast($"removed friend {friendCtx.Username}");
                        RefreshProfile();
                    }
                    else
                    {
                        Debug.WriteLine($"not removing friend {friendCtx.Username} ({friendCtx.Email})");
                    }
                });
            }

            var config = new ConfirmConfig
            {
                Title = $"Remove {friendCtx.Username}?",
                Message = $"Are you sure you want to remove {friendCtx.Username}?",
                OkText = "Yes",
                CancelText = "No",
                OnAction = ConfirmRemoveFriend
            };

            UserDialogs.Instance.Confirm(config);
        }
    }
}