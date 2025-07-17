using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Scripts.Camera
{
    public class Cameras
    {
        public UnityEngine.Camera MainCamera => _mainCamera;
        public UnityEngine.Camera ScreenCamera => _screenCamera;
        
        private readonly UnityEngine.Camera _mainCamera;
        private readonly UnityEngine.Camera _screenCamera;

        public Cameras(UnityEngine.Camera mainCamera, UnityEngine.Camera screenCamera)
        {
            _mainCamera = mainCamera;
            _screenCamera = screenCamera;
        }
    }
    
    public class CameraControlInstaller : MonoInstaller<CameraControlInstaller>
    {
        [SerializeField] private UnityEngine.Camera _mainCamera;
        [SerializeField] private UnityEngine.Camera _screenCamera;
        
        public sealed override void InstallBindings()
        {
            Debug.Log($"1 [CHECK]: CameraControlInstaller");
            //TODO: next there
            InstallCameraController();
            Debug.Log($"2 [CHECK]: CameraControlInstaller");

            Container.Bind<Cameras>()
                .FromMethod(() => new Cameras(_mainCamera, _screenCamera))
                .AsSingle();
        }

        public void SetContainer(DiContainer container) => Container = container;

        protected virtual void InstallCameraController()
        {
            //TODO: Bind controllers
        }
    }
}