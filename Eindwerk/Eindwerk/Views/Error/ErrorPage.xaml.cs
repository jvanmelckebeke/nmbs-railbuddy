using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Error
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage()
        {
            InitializeComponent();

            var pageTapGesture = new TapGestureRecognizer();
            pageTapGesture.Tapped += OnPageTapped;

            GrPage.GestureRecognizers.Add(pageTapGesture);
        }

        private void OnPageTapped(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
        }
    }
}