using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models;
using Eindwerk.Services;

namespace Eindwerk.Views
{
    public class LoggedInPage : NetworkDependentPage
    {
        protected readonly Tokens Tokens;
        protected AuthenticationService AuthenticationService;
        protected UserService UserService;
        protected UserProfile Profile;
        
        protected LoggedInPage(Tokens tokens)
        {
            Tokens = tokens;

            SetupProfileSafe();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetupProfileSafe();
        }

        private async void SetupProfileSafe()
        {
            await HandleNetworkAsync(SetupProfile);
            SetupVisual();
        }

        private async Task SetupProfile()
        {
            AuthenticationService = new AuthenticationService(Tokens);
            UserService = AuthenticationService.CreateWithTokens<UserService>();

            Profile = await UserService.GetUserProfileAsync();
        }

        protected virtual void SetupVisual()
        {
        }
    }
}