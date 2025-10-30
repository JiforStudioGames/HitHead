using System;
using Game.Scripts.Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using Zenject;

namespace Game.Scripts.Input
{
    public class InputService : IInitializable, IDisposable, IInputService
    {
        [Inject] private readonly Cameras _cameras;
        
        private readonly Subject<Ray> _onTap = new Subject<Ray>();
        private readonly Subject<Ray> _onDrag = new Subject<Ray>();
        private readonly Subject<Ray> _onRelease = new Subject<Ray>();
        
        private bool _isPressing;
        private readonly InputAction _pressAction;
        private readonly InputAction _positionAction;

        public IObservable<Ray> OnTap => _onTap;
        public IObservable<Ray> OnDrag => _onDrag;
        public IObservable<Ray> OnRelease => _onRelease;
        
        public InputService(InputActionAsset actions)
        {
            var map = actions.FindActionMap("Touch") ??
                      throw new ArgumentException("Not instance Action Map 'Touch' in InputActionAsset");
            _pressAction = map.FindAction("Press") ??
                           throw new ArgumentException("Not instance Action 'Press' in Touch");
            _positionAction = map.FindAction("Position") ??
                              throw new ArgumentException("Not instance Action 'Position' in Touch");
        }

        public void Initialize()
        {
            _pressAction.started += OnPressStarted;
            _pressAction.canceled += OnPressCanceled;
            _positionAction.performed += OnPositionPerformed;

            _pressAction.Enable();
            _positionAction.Enable();
        }
        
        private void OnPressStarted(InputAction.CallbackContext ctx)
        {
            _isPressing = true;
            var screenPos = _positionAction.ReadValue<Vector2>();
            Ray ray = _cameras.MainCamera.ScreenPointToRay(screenPos);
            _onTap.OnNext(ray);
        }

        private void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            _isPressing = false;
            var screenPos = _positionAction.ReadValue<Vector2>();
            Ray ray = _cameras.MainCamera.ScreenPointToRay(screenPos);
            _onRelease.OnNext(ray);
        }

        private void OnPositionPerformed(InputAction.CallbackContext ctx)
        {
            if (!_isPressing) return;
            var screenPos = ctx.ReadValue<Vector2>();
            Ray ray = _cameras.MainCamera.ScreenPointToRay(screenPos);
            _onDrag.OnNext(ray);
        }
        
        private Vector2 ScreenToWorld(Vector2 screenPos)
        {
            var world3 = _cameras.MainCamera.ScreenToWorldPoint(screenPos);
            return new Vector2(world3.x, world3.y);
        }

        public void Dispose()
        {
            _pressAction.started -= OnPressStarted;
            _pressAction.canceled -= OnPressCanceled;
            _positionAction.performed -= OnPositionPerformed;

            _pressAction.Disable();
            _positionAction.Disable();

            _onTap.Dispose();
            _onDrag.Dispose();
            _onRelease.Dispose();
        }
    }
}