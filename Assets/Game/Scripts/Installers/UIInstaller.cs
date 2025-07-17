using Game.Scripts.UI.Core;
using UI.Installers;
using Zenject;

namespace Game.Scripts.Installers
{
    public class UIInstaller : Installer<UIInstaller>
    {
        private readonly string SHOP_SCREEN_PATH = UIConstants.Screens(@"Shop/ShopScreen");

        public override void InstallBindings()
        {
            ShopInstaller.Install(Container);
        }
    }
}