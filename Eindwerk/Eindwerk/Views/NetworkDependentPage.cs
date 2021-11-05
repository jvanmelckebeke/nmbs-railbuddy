using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Exceptions;
using Eindwerk.Views.Error;
using Xamarin.Forms;

namespace Eindwerk.Views
{
    public abstract class NetworkDependentPage : ContentPage
    {
        protected async Task HandleNetworkAsync(Func<Task> functionToRun)
        {
            try
            {
                await functionToRun();
            }
            catch (NoNetworkException e)
            {
                Debug.WriteLine(e);
                GoToNoNetworkPage();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                GoToGenericErrorPage();
                throw;
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