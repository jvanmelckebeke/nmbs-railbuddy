using System;
using System.Diagnostics;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Tools;
using Xamarin.Forms;


namespace Eindwerk.ViewModels
{
    public class FriendRequestViewModel : FriendRequest
    {
        public FriendRequestViewModel()
        {
        }

        #region copy constructors

        public FriendRequestViewModel(FriendRequest friendRequest) :
            base(friendRequest)
        {
        }

        public FriendRequestViewModel(Friend friend) : base(friend)
        {
        }

        public FriendRequestViewModel(FriendRequestViewModel friendRequestViewModel) : base(friendRequestViewModel)
        {
        }

        #endregion
    }
}