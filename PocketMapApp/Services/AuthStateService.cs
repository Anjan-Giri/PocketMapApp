using PocketMapApp.Models;

namespace PocketMapApp.Services
{
    //for authentications
    public class AuthStateService
    {
        public User CurrentUser { get; private set; } //current user stores the currently authenticated user
        public event Action OnAuthStateChanged; //declaring onauthstatechanged event to trigger when the authentication state is changed

        //method to set the currentuser value to provided user
        public void SetUser(User user)
        {
            CurrentUser = user;
            NotifyAuthStateChanged(); //calling to notify the change in authstate
        }

        //logging out the user by setting currentuser to null
        public void Logout()
        {
            CurrentUser = null;
            NotifyAuthStateChanged();
        }

        //triggering onauthstatechanged event for state changes
        private void NotifyAuthStateChanged() => OnAuthStateChanged?.Invoke();
    }
}
