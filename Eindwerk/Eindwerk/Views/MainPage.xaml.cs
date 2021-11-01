using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Views;
using Xamarin.Forms;

namespace Eindwerk
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Login(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LoginPage());
        }
    }
}