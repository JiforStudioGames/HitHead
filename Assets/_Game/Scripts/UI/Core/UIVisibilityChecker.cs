using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Camera;
using Game.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public class UIVisibilityChecker : IInitializable
    {
        private readonly LayerMask _blockingMask = LayerMask.GetMask("UI");
        
        [Inject] private UINavigator _uiNavigator;
        [Inject] private UIRoot _uiRoot;
        [Inject] private Cameras _cameras;
        
        private UnityEngine.Camera _uiCamera;
        private PointerEventData _pointerEventData;
        private Transform _hudTransform;

        private Transform GetHUDTransform()
        {
            if (_hudTransform != null)
                return _hudTransform;
            
            var cityBuilderHUD = _uiNavigator.GetOrLoad<MainMenuHUDPresenter>();
            if (cityBuilderHUD != null)
            {
                _hudTransform = cityBuilderHUD.View.transform;
                return _hudTransform;
            }

            throw new Exception($"[{nameof(UIVisibilityChecker)}] There is no HUD on the scene");
        }

        public void Initialize()
        {
            _uiCamera = _cameras.ScreenCamera;
        }

        public bool IsOnlyHUDActive()
        {
            return IsOnlyScreenHUDActive() && !IsAnyPopupActive();
        }

        public bool IsOnlyScreenHUDActive()
        {
            return AreAllSiblingsBelowInactive(GetHUDTransform());
        }

        public bool IsAnyPopupActive()
        {
            return IsAnyChildrenActive(_uiRoot.Popups.transform);
        }
        
        public bool IsHUDBlocked()
        {
            if (_pointerEventData == null)
            {
                CreatePointerEventData();
            }

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(_pointerEventData, results);

            return results.Any(result =>
                _blockingMask == (_blockingMask | (1 << result.gameObject.layer)) &&
                result.gameObject.GetComponent<RectTransform>() != null);
        }
        
        private void CreatePointerEventData()
        {
            var hudRect = GetHUDTransform() as RectTransform;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, hudRect.position);

            _pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = screenPoint
            };
        }

        private bool AreAllSiblingsBelowInactive(Transform childTransform)
        {
            Transform parent = childTransform.parent;
            if (parent == null)
                return true;

            int currentChildIndex = childTransform.GetSiblingIndex();

            for (var i = currentChildIndex + 1; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling.gameObject.activeSelf)
                    return false;
            }

            return true;
        }

        private bool IsAnyChildrenActive(Transform parent)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling.gameObject.activeSelf && sibling.GetComponent<UIAntiClickView>() == null)
                    return true;
            }

            return false;
        }
    }
}