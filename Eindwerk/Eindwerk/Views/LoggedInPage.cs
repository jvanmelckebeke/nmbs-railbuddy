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
            SetupProfile();
        }

        private async void SetupProfile()
        {
            AuthenticationService = new AuthenticationService(Tokens);
            UserService = AuthenticationService.CreateWithTokens<UserService>();

            await HandleApi(async () => { Profile = await UserService.GetOwnUserProfileAsync(); }, "loading profile");
            
            SetupVisual();
        }

        protected virtual void SetupVisual()
        {
        }
    }
}