using System;
using System.Diagnostics;
using Acr.UserDialogs;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Tools;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileTapPage : LoggedInPage
    {
        public ProfileTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
            SetupListeners();
        }


        protected override void SetupVisual()
        {
            LblUser.Text = $"Hi, {Profile.Username}!";
            ImgAvatar.Source = ImageSource.FromUri(new Uri(Profile.AvatarUrl));

            ImgLogout.Source = AssetHelper.GetIcon("logout.png");
        }

        private void SetupListeners()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnLogoutClicked;

            FrLogout.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            ConfirmConfig confirmConfig = new ConfirmConfig()
            {
                Message = "Do you really want to logout",
                CancelText = "No",
                OkText = "Yes",
                OnAction = confirmed =>
                {
                    Debug.WriteLine($"result was {confirmed}");
                    if (!confirmed) return;
                    Debug.WriteLine("logging out");
                    AuthenticationService.Logout();
                    Navigation.PopToRootAsync();
                }
            };
            UserDialogs.Instance.Confirm(confirmConfig);
        }
    }
}