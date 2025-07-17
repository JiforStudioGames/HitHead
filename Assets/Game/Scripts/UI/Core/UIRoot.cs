using System;
using Game.Scripts.Camera;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public class UIRoot : MonoBehaviour
    {
        [Inject] private DiContainer _di;

        public Canvas Popups => _popups;
        public Canvas Screens => _screens;
        public Canvas LayerUnderScreens => _layerUnderScreens;
        public Canvas LayerOverScreens => _layerOverScreens;
        
        [SerializeField] private Canvas _popups;
        [SerializeField] private Canvas _screens;
        [SerializeField] private Canvas _layerUnderScreens;
        [SerializeField] private Canvas _layerOverScreens;

        [Inject]
        private void Construct(Cameras cameras)
        {
            _screens.worldCamera           = cameras.ScreenCamera;
            _layerUnderScreens.worldCamera = cameras.ScreenCamera;
            _layerOverScreens.worldCamera  = cameras.ScreenCamera;
            _popups.worldCamera            = cameras.ScreenCamera;
        }

        public void MoveToLayer<T>(EScreenType type) where T : class, IUIScreenPresenter
        {
            var presenter = _di.TryResolve<T>();
            if (presenter != null)
            {
                if (presenter.ScreenType == type) return;
                presenter.BaseView.transform.SetParent(GetRoot(type).transform);
            }
            else
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
        }

        private Canvas GetRoot(EScreenType type) => type switch
        {
            EScreenType.Popup => _popups,
            EScreenType.ScreenUnderPopup => _layerUnderScreens,
            EScreenType.Screen or EScreenType.ScreenWithHud => _screens,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}