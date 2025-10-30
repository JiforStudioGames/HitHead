using Game.Scripts.Domain.Controllers;
using Game.Scripts.Services;
using Zenject;

namespace Game.Scripts.Installers
{
    public class ControllersInstaller : Installer<ControllersInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EffectController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<EnemyController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<WeaponController>().AsSingle().NonLazy();
        }
    }
}