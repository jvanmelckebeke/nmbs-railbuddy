using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Exceptions;
using Xamarin.Forms;

namespace Eindwerk.Views
{
    public abstract class EncapsulatedPage : ContentPage
    {
        protected void GoToNoNetworkPage()
        {
            Navigation.PushAsync(new NoNetworkPage());
        }

        protected T EncapsulateExceptions<T>(Func<T> functionToRun)
        {
            try
            {
                return functionToRun();
            }
            catch (NoNetworkException e)
            {
                Debug.WriteLine(e);
                GoToNoNetworkPage();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

            return default;
        }

        protected async Task EncapsulateExceptionsAsync(Func<Task> functionToRun)
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
                throw;
            }
        }
    }
}