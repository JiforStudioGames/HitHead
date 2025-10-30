using Game.Scripts.Camera;
using UnityEngine;
using Zenject;

namespace UI.Installers
{
    public class CameraControlInstaller : MonoInstaller
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private UnityEngine.Camera _screenCamera;

        public override void InstallBindings()
        {
            Container.BindInstance(new Cameras(_camera, _screenCamera))
                .AsSingle();
        }
    }
}
