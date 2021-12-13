using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;

namespace Eindwerk.Views
{
    public class LoggedInPage : NetworkDependentPage
    {
        protected readonly Tokens                Tokens;
        protected          AuthenticationService AuthenticationService;
        protected          UserService           UserService;
        protected          UserProfile           Profile;

        protected LoggedInPage(Tokens tokens)
        {
            Tokens = tokens;

            // SetupProfile();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Profile == null)
            {
                RefreshProfile();
            }
            else
            {
                QuietRefreshProfile();
            }
        }

        private async void QuietRefreshProfile()
        {
            await SetupProfile();
        }

        private async Task SetupProfile()
        {
            AuthenticationService = new AuthenticationService(Tokens);
            UserService = AuthenticationService.CreateWithTokens<UserService>();
            Profile = await UserService.GetOwnUserProfileAsync();

            Debug.WriteLine($"profile: {Profile}");

            if (Profile == null)
            {
                UserDialogs.Instance.Alert("the session has expired", "Invalid profile", "Ok");
                AuthenticationService.Logout();
                Navigation.PopToRootAsync();
            }
            else
            {
                await SetupData();
                SetupVisual();
            }
        }

        protected async void RefreshProfile()
        {
            var loader = UserDialogs.Instance.Loading("Loading...");
            loader.Show();
            await SetupProfile();
            loader.Hide();
        }

        protected virtual void SetupVisual() { }

        protected virtual async Task SetupData() { }
    }
}