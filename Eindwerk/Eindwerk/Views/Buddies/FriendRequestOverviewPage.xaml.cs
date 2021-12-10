﻿using System;
using System.Diagnostics;
using Acr.UserDialogs;
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
            var friendRequests = Profile.FriendRequestsReceived;

            LstBuddyRequest.ItemsSource = friendRequests;

            SetupHandlers();
        }

        private void SetupHandlers()
        {
            BtnGoBack.Clicked += OnGoBackClick;
        }


        private void OnGoBackClick(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void HandleAccept(object sender, EventArgs e)
        {
            ImageButton imageButton = (ImageButton) sender;
            FriendRequest request = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"accepting friend request {request}");

            async void ConfirmAction(bool confirm)
            {
                if (!confirm) return;

                var req = await UserService.AcceptFriendAsync(request.UserId.ToString());

                UserDialogs.Instance.Toast($"added {req.Username}");

                RefreshProfile();
            }

            var config = new ConfirmConfig()
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
            ImageButton imageButton = (ImageButton) sender;
            FriendRequest request = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"ignoring friend request {request}");

            async void ConfirmAction(bool confirm)
            {
                if (!confirm) return;

                var req = await UserService.DeleteFriendAsync(request.UserId.ToString());

                if (req == null)
                {
                    UserDialogs.Instance.Alert("an error has occured");
                }
                else
                {
                    UserDialogs.Instance.Toast($"ignored {req.Username}");
                }

                RefreshProfile();
            }

            var config = new ConfirmConfig()
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