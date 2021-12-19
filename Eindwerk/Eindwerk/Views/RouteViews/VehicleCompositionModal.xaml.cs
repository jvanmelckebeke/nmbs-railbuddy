using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Models.Rail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VehicleCompositionModal : LoggedInPage
    {
        private Vehicle _vehicle;

        private List<Wagon> _wagons;

        public VehicleCompositionModal(Tokens tokens, Vehicle vehicle) : base(tokens)
        {
            InitializeComponent();
            _vehicle = vehicle;
            BindingContext = _vehicle;
        }

        protected override async Task SetupData()
        {
            _wagons = await UserService.GetTrainCompositionAsync(_vehicle.VehicleNumber);
            ColWagons.ItemsSource = _wagons;
        }

        private async void GoBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}