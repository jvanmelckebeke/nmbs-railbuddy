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

        protected override async void OnAppearing()
        {
            CheckNetwork();
            await SetupDataSafe();
            base.OnAppearing();
        }

        protected void CheckNetwork()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) GoToNoNetworkPage();
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CheckNetwork();
        }

        protected async Task HandleApi(Func<Task> apiCall, string loadingText = null)
        {
            IProgressDialog loadingDialog = null;
            if (loadingText != null) loadingDialog = UserDialogs.Instance.Loading(loadingText);

            try
            {
                Debug.WriteLine("start handling api call");
                await apiCall();
                Debug.WriteLine("end handling api call");

                loadingDialog?.Hide();
            }
            catch (NoNetworkException e)
            {
                loadingDialog?.Hide();
                Debug.WriteLine("START OF NO NETWORK EXCEPTION");
                Debug.WriteLine(e);
                Debug.WriteLine("END OF NO NETWORK EXCEPTION");
                GoToNoNetworkPage();
            }
            catch (Exception e)
            {
                loadingDialog?.Hide();
                Debug.WriteLine("START OF REAL EXCEPTION");
                Debug.WriteLine(e);
                Debug.WriteLine("END OF REAL EXCEPTION");
                GoToNoNetworkPage();
            }
        }

        protected async Task SetupDataSafe()
        {
            Debug.WriteLine("=== setting up data safe");
            await HandleApi(SetupData, "loading data");
            Debug.WriteLine("=== end setting up data safe");
        }

        protected virtual Task SetupData()
        {
            return Task.CompletedTask;
        }

        #region navigation

        private void GoToNoNetworkPage()
        {
            Debug.WriteLine("going to no network page");
            Navigation.PushModalAsync(new NoNetworkPage(), false);
        }

        private void GoToGenericErrorPage()
        {
            Navigation.PushModalAsync(new ErrorPage(), false);
        }

        #endregion
    }
}