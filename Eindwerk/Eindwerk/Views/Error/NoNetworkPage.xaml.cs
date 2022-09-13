using System;
using System.Net.Http;
using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Error
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoNetworkPage : ContentPage
    {
        public NoNetworkPage()
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnPageTapped;

            GrPage.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private async void OnPageTapped(object sender, EventArgs e)
        {
            // check network connection

            using (var client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync("https://ifconfig.co");
                    Console.WriteLine($"ifconfig.co response: {response}");

                    UserDialogs.Instance.Toast("Network restored");

                    await Navigation.PopModalAsync();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("still no network");

                    UserDialogs.Instance.Toast("Sorry, no network connection");
                }
            }
        }
    }
}