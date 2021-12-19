using System;
using System.Collections.Generic;
using System.Diagnostics;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
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
            List<FriendRequest> friendRequests = Profile.FriendRequestsReceived;

            LstBuddyRequest.ItemsSource = friendRequests;
        }


        private void OnGoBackClick(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void HandleAccept(object sender, EventArgs e)
        {
            var imageButton = (ImageButton) sender;
            var request = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"accepting friend request {request}");

            async void ConfirmAction(bool confirm)
            {
                if (!confirm) return;

                await HandleApi(async () => await UserService.AcceptFriendAsync(request.UserId.ToString()));

                UserDialogs.Instance.Toast($"added {request.Username}");

                RefreshProfile();
            }

            var config = new ConfirmConfig
            {
                Title = $"Add {request.Username}?",
                Message =
                    $"Are you sure you want to add {request.Username} ({request.Email}) to your buddies?",
                CancelText = "No",
                OkText = "Yes",
                OnAction = ConfirmAction
            };

            UserDialogs.Instance.Confirm(config);
        }

        private void HandleDeny(object sender, EventArgs e)
        {
            var imageButton = (ImageButton) sender;
            var request = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"ignoring friend request {request}");

            async void ConfirmAction(bool confirm)
            {
                if (!confirm) return;

                await HandleApi(async () =>
                {
                    BasicFriendRequestStatus req = await UserService.DeleteFriendAsync(request.UserId.ToString());

                    if (req == null)
                        UserDialogs.Instance.Alert("an error has occured");
                    else
                        UserDialogs.Instance.Toast($"ignored {request.Username}");
                }, "ignoring request");

                RefreshProfile();
            }

            var config = new ConfirmConfig
            {
                Title = $"Ignore {request.Username}?",
                Message =
                    $"Are you sure you want to ignore {request.Username} ({request.Email})?",
                CancelText = "No",
                OkText = "Yes",
                OnAction = ConfirmAction
            };

            UserDialogs.Instance.Confirm(config);
        }
    }
}