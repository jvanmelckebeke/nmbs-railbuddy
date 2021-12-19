using System;
using System.Diagnostics;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
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
            ImgAvatar.Source = Profile.Avatar;
        }

        private void SetupListeners()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnLogoutClicked;

            FrLogout.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            var confirmConfig = new ConfirmConfig
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