using PocketMapApp.Models;

namespace PocketMapApp.Services
{
    public class AuthStateService
    {
        public User CurrentUser { get; private set; }
        public event Action OnAuthStateChanged;

        public void SetUser(User user)
        {
            CurrentUser = user;
            NotifyAuthStateChanged();
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyAuthStateChanged();
        }

        private void NotifyAuthStateChanged() => OnAuthStateChanged?.Invoke();
    }
}
