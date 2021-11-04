using System;
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
                GoToNoNetworkPage();
            }

            return default;
        }
    }
}