using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Exceptions;
using Eindwerk.Views.Error;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Eindwerk.Views
{
    public abstract class NetworkDependentPage : ContentPage
    {
        public NetworkDependentPage()
        {
            Connectivity.ConnectivityChanged += ConnectivityChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckNetwork();
        }

        private void CheckNetwork()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                GoToNoNetworkPage();
            }
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CheckNetwork();
        }

        protected async Task HandleApi(Func<Task> apiCall, string loadingText = null)
        {
            IProgressDialog loadingDialog = null;
            if (loadingText != null)
            {
                loadingDialog = UserDialogs.Instance.Loading(loadingText);
            }

            try
            {
                await apiCall();

                loadingDialog?.Hide();
            }
            catch (Exception e)
            {
                loadingDialog?.Hide();
                Debug.WriteLine(e);
                GoToGenericErrorPage();
            }
        }

        #region navigation

        private void GoToNoNetworkPage()
        {
            Navigation.PushAsync(new NoNetworkPage());
        }

        private void GoToGenericErrorPage()
        {
            Navigation.PushAsync(new ErrorPage());
        }

        #endregion
    }
}