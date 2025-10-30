using Game.Scripts.Services;
using Zenject;

namespace Game.Scripts.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            ConfigsInstaller.Install(Container);
            ModelsInstaller.Install(Container);
            ServicesInstaller.Install(Container);
            ControllersInstaller.Install(Container);
            UIInstaller.Install(Container);
        }
    }
}