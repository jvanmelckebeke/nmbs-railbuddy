using System.Collections.Generic;
using Eindwerk.Models;
using Eindwerk.Models.Rail;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsResultPage : NetworkDependentPage
    {
        private List<Route> _connections;
        
        public ConnectionsResultPage(List<Route> connections)
        {
            InitializeComponent();
            _connections = connections;
        }
    }
}