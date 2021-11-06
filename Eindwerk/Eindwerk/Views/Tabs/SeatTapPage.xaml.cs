using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeatTapPage : NetworkDependentPage
    {
        private Tokens _tokens;

        public SeatTapPage(Tokens tokens)
        {
            InitializeComponent();
            _tokens = tokens;
        }
    }
}