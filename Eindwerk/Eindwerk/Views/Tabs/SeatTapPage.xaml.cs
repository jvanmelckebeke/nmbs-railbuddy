using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeatTapPage : LoggedInPage
    {
        public SeatTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
        }


        private async void ScanSeat(object sender, EventArgs e)
        {
            var options = new MobileBarcodeScanningOptions()
            {
                AutoRotate = false,
                UseNativeScanning = true,
                TryHarder = true
            };

            var overlay = new ZXingDefaultOverlay()
            {
                TopText = "Wagon QR code scan",
                BottomText = "Please Wait...."
            };

            var qrScanner = new ZXingScannerPage(options, overlay);


            await Navigation.PushModalAsync(qrScanner);

            qrScanner.OnScanResult += (scanResult) =>
            {
                qrScanner.IsScanning = false;
                Debug.WriteLine($"recording seat {scanResult.Text}");
                Device.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());

                var seat = JsonConvert.DeserializeObject<SeatRegistration>(scanResult.Text);

                UserService.RegisterSeat(seat);
                
                UserDialogs.Instance.Toast("scanned seat");
            };
        }
    }
}