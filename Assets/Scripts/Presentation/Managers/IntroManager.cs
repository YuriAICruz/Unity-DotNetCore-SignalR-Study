using Controllers;
using UnityEngine.SceneManagement;

namespace Presentation.Managers
{
    public class IntroManager
    {
        private readonly AuthenticationController _auth;

        public IntroManager(AuthenticationController auth)
        {
            _auth = auth;

            _auth.UserLoggedIn += NextWindow;
            _auth.UserSignedIn += NextWindow;

            _auth.UserSignedOut += ReturnToLogin;
        }

        private void ReturnToLogin()
        {
            SceneManager.LoadScene(0);
        }

        private void NextWindow()
        {
            SceneManager.LoadScene(1);
        }
    }
}