using System;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Tools;
using Eindwerk.Views.RouteViews;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeTabPage : LoggedInPage
    {
        public HomeTabPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
            SetupListeners();
        }

        protected override void SetupVisual()
        {
            #region static images

            // ImgAddFavorite.Source = AssetHelper.GetIcon("plus.png");

            #endregion

            #region user text and images

            #endregion
        }


        private void SetupListeners()
        {
            BtnStartTravel.Clicked += OnStartTravelClicked;
        }

        #region event handlers

        private void OnStartTravelClicked(object sender, EventArgs e)
        {
            GoToPrepareRoute();
        }

        private void LogoutTapped(object sender, EventArgs e)
        {
            AuthenticationService.Logout();
            Navigation.PopToRootAsync();
        }

        #endregion

        #region navigation

        private void GoToPrepareRoute()
        {
            Navigation.PushAsync(new PrepareRoutePage(Tokens), true);
        }

        #endregion
    }
}