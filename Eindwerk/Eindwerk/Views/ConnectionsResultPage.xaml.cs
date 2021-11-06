using System.Collections.Generic;
using Eindwerk.Models;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsResultPage : NetworkDependentPage
    {
        private List<Connection> _connections;
        
        public ConnectionsResultPage(List<Connection> connections)
        {
            InitializeComponent();
            _connections = connections;
        }
    }
}