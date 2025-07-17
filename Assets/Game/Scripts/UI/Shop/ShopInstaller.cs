using Game.Scripts.Domain.Models;
using Game.Scripts.UI.Core;
using Game.Scripts.UI.Shop;
using Zenject;

namespace UI.Installers
{
    public class ShopInstaller : Installer<ShopInstaller>
    {
        private readonly string SHOP_SCREEN_PATH = UIConstants.Screens(@"Shop/ShopScreen");

        public override void InstallBindings()
        {
            Container.BindPresenter<ShopPresenter>()
                .WithViewFromPrefab(SHOP_SCREEN_PATH)
                .AsScreen();
        }
    }
}