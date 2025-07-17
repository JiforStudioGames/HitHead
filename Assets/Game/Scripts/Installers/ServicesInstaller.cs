using Game.Scripts.Domain.Models;
using Game.Scripts.Services;
using Zenject;

namespace Game.Scripts.Installers
{
    public class ServicesInstaller: Installer<ServicesInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelProgressService>()
                .AsSingle();
            Container.BindInterfacesTo<InventoryService>()
                .AsSingle();
            Container.BindInterfacesTo<CurrencyService>()
                .AsSingle();
            Container.BindInterfacesTo<RewardService>()
                .AsSingle();
        }
    }
}