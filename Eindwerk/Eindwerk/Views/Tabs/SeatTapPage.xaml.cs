using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeatTapPage : LoggedInPage
    {
        public SeatTapPage(Tokens tokens) : base(tokens)
        {
            InitializeComponent();
        }

        
    }
}