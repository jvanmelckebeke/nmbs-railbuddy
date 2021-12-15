using System.Collections.Generic;
using System.Diagnostics;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Requests;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsResultPage : LoggedInPage
    {
        private readonly List<Route>         _connections;
        private readonly SearchRoutesRequest _originalRoutesRequest;

        public ConnectionsResultPage(Tokens tokens, SearchRoutesRequest originalRoutesRequest, List<Route> connections)
            : base(tokens)
        {
            InitializeComponent();
            _connections = connections;
            _originalRoutesRequest = originalRoutesRequest;
        }

        protected override void SetupVisual()
        {
            LblFromStation.Text = _originalRoutesRequest.FromStation.StandardName;
            LblToStation.Text = _originalRoutesRequest.ToStation.StandardName;

            LblTimeSel.Text = _originalRoutesRequest.TimeSelection == TimeSelection.Arrival ? "arrival" : "departure";
            LblDate.Text = _originalRoutesRequest.Time.ToString("dd MMMM yyyy");
            LblTime.Text = _originalRoutesRequest.Time.ToString("H:mm");

            LstRoutes.ItemsSource = _connections;

            LstRoutes.ItemSelected += OnRouteSelected;
        }

        private async void OnRouteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Route r = (Route) LstRoutes.SelectedItem;

            if (r != null)
            {
                await Navigation.PushAsync(new RouteOverviewPage(Tokens, r), true);
                Debug.WriteLine("opening new page");
            }

            LstRoutes.SelectedItem = null;
        }
    }
}