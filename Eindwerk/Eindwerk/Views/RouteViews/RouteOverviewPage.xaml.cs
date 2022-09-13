using System;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Connections;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteOverviewPage : LoggedInPage
    {
        private readonly Route _route;

        public RouteOverviewPage(Tokens tokens, Route route) : base(tokens)
        {
            InitializeComponent();
            _route = route;
            BindingContext = _route;
        }


        private async void OpenDepartingTrainComposition(object sender, EventArgs e)
        {
            Vehicle vehicle = _route.DepartureConnection.Vehicle;

            await Navigation.PushModalAsync(new VehicleCompositionModal(Tokens, vehicle), true);
        }

        private async void OpenViaTrainComposition(object sender, EventArgs e)
        {
            var b = (Button) sender;

            var ctx = (ViaConnection) b.BindingContext;

            await Navigation.PushModalAsync(new VehicleCompositionModal(Tokens, ctx.Departure.Vehicle), true);
        }
    }
}