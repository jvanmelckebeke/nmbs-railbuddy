using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Tools;
using Eindwerk.ViewModels;
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

            // this is a bit stupid
            var viewFriendRequests =
                friendRequests.Select(friendRequest =>
                    new FriendRequestViewModel(friendRequest));

            LstBuddyRequest.ItemsSource = viewFriendRequests;

            SetupHandlers();
        }

        private void SetupHandlers()
        {
            BtnGoBack.Clicked += OnGoBackClick;
        }

        private EventHandler CreateAcceptHandler(Friend friendRequest)
        {
            return delegate(object sender, EventArgs args)
            {
                Debug.WriteLine($"accepting friend request {friendRequest}");

                ConfirmConfig config = new ConfirmConfig()
                {
                    Title = $"Add {friendRequest.Username}?",
                    Message =
                        $"Are you sure you want to add {friendRequest.Username} ({friendRequest.Email}) to your buddies?",
                    CancelText = "No",
                    OkText = "Sure",
                    OnAction = delegate(bool result) { Debug.WriteLine($"result is {result}"); }
                };

                UserDialogs.Instance.Confirm(config);
            };
        }


        private void OnGoBackClick(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void HandleAccept(object sender, EventArgs e)
        {
            ImageButton imageButton = (ImageButton) sender;
            FriendRequest friendRequest = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"accepting friend request {friendRequest}");

            ConfirmConfig config = new ConfirmConfig()
            {
                Title = $"Add {friendRequest.Username}?",
                Message =
                    $"Are you sure you want to add {friendRequest.Username} ({friendRequest.Email}) to your buddies?",
                CancelText = "No",
                OkText = "Sure",
                OnAction = HandleConfirmAccept
            };

            UserDialogs.Instance.Confirm(config);
        }

        private void HandleConfirmAccept(bool confirm)
        {
            
        }

        private void HandleDeny(object sender, EventArgs e)
        {
            ImageButton imageButton = (ImageButton) sender;
            FriendRequest friendRequest = (FriendRequest) imageButton.BindingContext;

            Debug.WriteLine($"ignoring friend request {friendRequest}");

            var config = new ConfirmConfig()
            {
                Title = $"Ignore {friendRequest.Username}?",
                Message =
                    $"Are you sure you want to ignore {friendRequest.Username} ({friendRequest.Email})?",
                CancelText = "No",
                OkText = "Yes",
                OnAction = HandleConfirmDeny
            };

            UserDialogs.Instance.Confirm(config);
        }

        private void HandleConfirmDeny(bool confirm)
        {
        }
    }
}