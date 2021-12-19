using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail.Requests;
using Eindwerk.Repository;
using Eindwerk.Views.RouteViews;
using Xamarin.Forms;
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
            BindingContext = new HomeTabPageViewModel
            {
                Favorites = FavoriteRepository.GetFavorites()
            };
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

        #endregion

        #region navigation

        private void GoToPrepareRoute()
        {
            Navigation.PushAsync(new PrepareRoutePage(Tokens), true);
        }

        #endregion

        private void OnDeleteFavorite(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;
            var ctx = (BaseRouteRequest) button.BindingContext;

            void ConfirmDelete(bool acc)
            {
                if (!acc) return;
                FavoriteRepository.RemoveFromFavorites(ctx.RouteHash);
                UserDialogs.Instance.Toast($"removed {ctx.Name} from favorites");
                SetupVisual();
            }

            var config = new ConfirmConfig
            {
                Title = "Remove favorite?",
                Message = $"Are you sure you want to remove {ctx.Name} from your favorites?",
                OnAction = ConfirmDelete,
                OkText = "Yes",
                CancelText = "No"
            };

            UserDialogs.Instance.Confirm(config);
        }

        private void OnGoFavorite(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;
            var ctx = (BaseRouteRequest) button.BindingContext;

            Navigation.PushAsync(new PrepareRoutePage(Tokens, ctx), true);
        }
    }

    internal class HomeTabPageViewModel
    {
        public List<BaseRouteRequest> Favorites { get; set; }
    }
}