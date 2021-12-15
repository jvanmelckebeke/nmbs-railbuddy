using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteOverviewPage : LoggedInPage
    {
        private Route _route;

        public RouteOverviewPage(Tokens tokens, Route route) : base(tokens)
        {
            InitializeComponent();
            _route = route;
            BindingContext = _route;

            Debug.WriteLine(JsonConvert.SerializeObject(_route));
        }
    }
}