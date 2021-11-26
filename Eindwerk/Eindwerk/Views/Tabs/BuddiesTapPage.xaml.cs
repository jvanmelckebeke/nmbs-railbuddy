using System;
using System.Diagnostics;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("PROFILEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            Debug.WriteLine(Profile);
            Debug.WriteLine("PROFILEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
        }

        protected override void SetupVisual()
        {
            ImgQr.Source = ImageSource.FromUri(new Uri(Profile.QrCodeUrl));

            Debug.WriteLine(ImgQr.Source);
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
            
            // todo
        }
    }
}