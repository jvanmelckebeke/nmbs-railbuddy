using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeatTapPage : LoggedInPage
    {
        private SeatTapPageViewModel _viewModel;

        public SeatTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
            _viewModel = new SeatTapPageViewModel();
        }

        protected override async Task SetupData()
        {
            SeatRegistration ownRegistration = await UserService.GetOwnSeatRegistrationAsync();

            if (ownRegistration == null)
            {
                _viewModel = new SeatTapPageViewModel();
                return;
            }

            var service = new RailService();

            _viewModel.CurrentSeat = ownRegistration;
            _viewModel.CurrentVehicle = await service.GetVehicle(ownRegistration.VehicleName);
            _viewModel.CurrentComposition = await UserService.GetTrainCompositionAsync(ownRegistration.VehicleName);
        }

        protected override void SetupVisual()
        {
            if (_viewModel.HasCurrentSeat)
            {
                LblCurrentlyNotOnTrain.IsVisible = false;
                BtnScanWagon.IsVisible = false;

                StTrainComposition.IsVisible = true;
                GrWagonActiveButtons.IsVisible = true;

                LblCurrentVehicleName.Text = _viewModel.CurrentVehicle.FormattedName;
                BvDivider.BackgroundColor = Color.FromHex(_viewModel.CurrentVehicle.VehicleColor);
                ColTrainComposition.ItemsSource = _viewModel.CurrentComposition;
            }
            else
            {
                LblCurrentlyNotOnTrain.IsVisible = true;
                BtnScanWagon.IsVisible = true;

                StTrainComposition.IsVisible = false;
                GrWagonActiveButtons.IsVisible = false;
            }
        }


        private async void ScanSeat(object sender, EventArgs e)
        {
            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = false,
                UseNativeScanning = true,
                TryHarder = true
            };

            var overlay = new ZXingDefaultOverlay
            {
                TopText = "Wagon QR code scan",
                BottomText = "Please Wait...."
            };

            var qrScanner = new ZXingScannerPage(options, overlay);


            await Navigation.PushModalAsync(qrScanner);

            async void OnScan(Result scanResult)
            {
                qrScanner.IsScanning = false;
                Debug.WriteLine($"recording seat {scanResult.Text}");
                Device.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());

                var seat = JsonConvert.DeserializeObject<SeatRegistration>(scanResult.Text);


                async Task RegisterCall()
                {
                    await UserService.RegisterSeat(seat);
                }

                await HandleApi(RegisterCall, "registering wagon");

                UserDialogs.Instance.Toast("scanned seat");
                await SetupDataSafe();
            }

            qrScanner.OnScanResult += OnScan;
        }

        private async void RemoveSeat(object sender, EventArgs e)
        {
            await HandleApi(async () => await UserService.RemoveSeat());
            await SetupDataSafe();
            SetupVisual();
        }
    }

    public class SeatTapPageViewModel
    {
        public Vehicle CurrentVehicle { get; set; }

        public SeatRegistration CurrentSeat { get; set; }

        public List<Wagon> CurrentComposition { get; set; }

        public bool HasCurrentSeat => CurrentSeat != null;
    }
}