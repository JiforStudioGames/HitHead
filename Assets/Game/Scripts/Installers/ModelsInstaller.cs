using Game.Scripts.Domain.Models;
using Game.Scripts.Services;
using Zenject;

namespace Game.Scripts.Installers
{
    public class ModelsInstaller : Installer<ModelsInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CurrencyModel>()
                .AsSingle();
            Container.Bind<InventoryModel>()
                .AsSingle();
            Container.Bind<LevelProgressModel>()
                .AsSingle();
        }
    }
}