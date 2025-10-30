using Game.Scripts.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Installers
{
    public class InputInstaller : MonoInstaller
    {
        [SerializeField] 
        private InputActionAsset _inputActions;

        public override void InstallBindings()
        {
            Container.BindInstance(_inputActions)
                .AsSingle()
                .IfNotBound();
            
            Container.BindInterfacesTo<InputService>()
                .AsSingle()
                .NonLazy();
        }
    }
}