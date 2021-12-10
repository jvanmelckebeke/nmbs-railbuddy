using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Tools;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Error
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage()
        {
            InitializeComponent();

            TapGestureRecognizer pageTapGesture = new TapGestureRecognizer();
            pageTapGesture.Tapped += OnPageTapped;

            GrPage.GestureRecognizers.Add(pageTapGesture);
        }

        private void OnPageTapped(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
        }
    }
}