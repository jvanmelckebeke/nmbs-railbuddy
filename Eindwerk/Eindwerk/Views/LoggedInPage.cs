using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;

namespace Eindwerk.Views
{
    public class LoggedInPage : NetworkDependentPage
    {
        protected AuthenticationService AuthenticationService;
        protected UserProfile Profile;
        protected Tokens Tokens;
        protected UserService UserService;

        protected LoggedInPage(Tokens tokens)
        {
            Tokens = tokens;
        }

        protected override void OnAppearing()
        {
            if (Profile == null)
                RefreshProfile();
            else
                QuietRefreshProfile();

            base.OnAppearing();
        }

        private async void QuietRefreshProfile()
        {
            await SetupProfileSafe();
        }

        protected async Task SetupProfileSafe()
        {
            await HandleApi(SetupProfile, "loading profile");
        }

        private async Task SetupProfile()
        {
            AuthenticationService = new AuthenticationService(Tokens);
            UserService = AuthenticationService.CreateWithTokens<UserService>();
            CheckNetwork();
            Profile = await UserService.GetOwnUserProfileAsync();

            Debug.WriteLine($"profile: {Profile}");

            if (Profile == null)
            {
                Tokens newTokens = await AuthenticationService.TryRefreshTokensAsync();

                if (newTokens == null)
                {
                    await UserDialogs.Instance.AlertAsync("the session has expired", "Invalid profile", "Ok");
                    AuthenticationService.Logout();
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    Tokens = newTokens;
                    UserService = AuthenticationService.CreateWithTokens<UserService>();
                    Profile = await UserService.GetOwnUserProfileAsync();
                }
            }
            else
            {
                await SetupDataSafe();
                SetupVisual();
            }
        }

        protected async void RefreshProfile()
        {
            await SetupProfileSafe();
        }

        protected virtual void SetupVisual() { }
    }
}