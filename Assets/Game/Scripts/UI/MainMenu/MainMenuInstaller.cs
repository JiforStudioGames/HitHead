using Game.Scripts.UI.Core;
using Game.Scripts.UI.MainMenu;
using Zenject;

namespace UI.Installers
{
    public class MainMenuInstaller : UIInstaller<MainMenuHUDView, MainMenuHUDPresenter>
    {
        protected override string HudPrefabPath => "UI/HUD/MainMenuHUD";

        protected override void InstallBindingsInternal()
        {
            
        }
    }
}