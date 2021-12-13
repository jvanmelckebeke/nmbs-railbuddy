using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Components;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Services;
using Eindwerk.Tools;
using Eindwerk.Views.Buddies;
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

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnGoToFriendRequestsTapped;

            FrFriendRequests.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnGoToFriendRequestsTapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FriendRequestOverviewPage(Tokens), true);
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
            UserProfile profile = await UserService.GetUserProfileAsync(profileId);

            if (profile == null)
                return;

            async void ConfirmFriend(bool confirmed)
            {
                if (confirmed)
                {
                    Debug.WriteLine($"adding friend with profileId {profileId}");
                    FriendRequest response = await UserService.RequestFriendAsync(profileId);
                    Debug.WriteLine(response);
                    UserDialogs.Instance.Toast($"friend request sent to {profile.Username}");
                }
            }

            ConfirmConfig config = new ConfirmConfig()
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
            Button btn = (Button) sender;

            Friend friendCtx = (Friend) btn.BindingContext;

            async void ConfirmRemoveFriend(bool confirm)
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
            }

            ConfirmConfig config = new ConfirmConfig()
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