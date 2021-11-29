using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
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

            SetupProfile();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(SetupProfile).Wait();
        }

        private async Task SetupProfile()
        {
            AuthenticationService = new AuthenticationService(Tokens);
            UserService = AuthenticationService.CreateWithTokens<UserService>();


            await HandleApi(async () => { Profile = await UserService.GetOwnUserProfileAsync(); });

            Debug.WriteLine($"profile: {Profile}");

            if (Profile == null)
            {
                UserDialogs.Instance.Alert("the session has expired", "Invalid profile", "Ok");
                AuthenticationService.Logout();
            }
            else
            {
                await HandleApi(SetupVisualTask);
            }
        }

        protected virtual void SetupVisual()
        {
        }

        private Task SetupVisualTask()
        {
            SetupVisual();
            return Task.FromResult("whatever");
        }
    }
}