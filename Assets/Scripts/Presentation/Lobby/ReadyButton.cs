using Presentation.Managers;
using Zenject;

namespace Presentation.Lobby
{
    public class ReadyButton : ButtonView
    {
        [Inject] private LobbyManager _manager;
        
        protected override void OnClick()
        {
            _manager.IsReady();
        }
    }
}