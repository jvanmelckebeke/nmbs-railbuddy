using System.Collections.Generic;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Requests;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsResultPage : LoggedInPage
    {
        private readonly List<Route> _connections;
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
            LblTime.Text = _originalRoutesRequest.Time.ToString("hh:mm");

            LstRoutes.ItemsSource = _connections;
        }
    }
}