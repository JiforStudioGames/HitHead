using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public class UINavigator : IInitializable, IDisposable
    {
        [Inject(Id = "HUD")] private ISimpleUIScreenPresenter _hud;

        [Inject] private DiContainer _di;

        public static UINavigator Instance { get; private set; }

        public DiContainer DiContainer => _di;

        public UINavigator()
        {
            Instance = this;
        }

        public ReactiveCommand OnHudShowed { get; } = new ReactiveCommand();
        public ReactiveCommand<IUIScreenPresenter> ScreenOpened { get; private set; }
        public ReactiveCommand<IUIScreenPresenter> ScreenClosed { get; private set; }

        private readonly List<IUIScreenPresenter> _screens = new();
        private readonly List<IUIScreenPresenter> _popups = new();
        private CompositeDisposable _disposables = new();

        public ISimpleUIScreenPresenter HUD => _hud;
        public IReadOnlyList<IUIScreenPresenter> OpenedScreens => _screens;
        public IReadOnlyList<IUIScreenPresenter> OpenedPopups => _popups;

        public T GetOrLoad<T>() where T : class, IUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            return presenter;
        }
        public IUIScreenPresenter GetOrLoad(Type type)
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<IUIScreenPresenter>(type, _di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {type.Name}: Unable to resolve");
                return null;
            }
            return presenter;
        }

        public void Perform<T>(Action<T> action) where T : class, IUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter != null)
                action(presenter);
            else
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
        }

        public void Perform<T>(Type type, Action<T> action) where T : class, IUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter != null)
                action(presenter);
            else
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
        }

        public T Show<T>(Action<T> afterShowCallback = null) where T : class, ISimpleUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return default;
            }

            if (afterShowCallback == null)
                presenter.Show().Forget(UnityEngine.Debug.LogException);
            else
                presenter.Show().ContinueWith(() => afterShowCallback(presenter))
                    .Forget(UnityEngine.Debug.LogException);

            return presenter;
        }

        public void Hide<T, TModel>()
            where T : class, IModelUIScreenPresenter<TModel>
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            presenter.Hide().Forget(UnityEngine.Debug.LogException);
        }

        public void Hide<T>()
            where T : class, ISimpleUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            presenter.Hide().Forget(UnityEngine.Debug.LogException);
        }

        public void Hide(Type type)
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<ISimpleUIScreenPresenter>(type, _di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {type.Name}: Unable to resolve");
                return;
            }

            presenter.Hide().Forget(UnityEngine.Debug.LogException);
        }

        public void Show(Type type)
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<ISimpleUIScreenPresenter>(type, _di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {type.Name}: Unable to resolve");
                return;
            }

            presenter.Show().Forget(UnityEngine.Debug.LogException);
        }

        public async UniTask ShowAsync<T>() where T : class, ISimpleUIScreenPresenter
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            await presenter.Show();
        }

        public async UniTask ShowAsync(Type type)
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<ISimpleUIScreenPresenter>(type, _di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {type.Name}: Unable to resolve");
                return;
            }

            await presenter.Show();
        }

        public void Show2<T, TModel>(TModel model, Action<T> afterShowCallback = null)
            where T : class, IModelUIScreenPresenter<TModel>
        {
            var presenter = _di.TryResolve<T>();
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            if (afterShowCallback == null)
                presenter.Show(model).Forget(UnityEngine.Debug.LogException);
            else
                presenter.Show(model).ContinueWith(() => afterShowCallback(presenter))
                    .Forget(UnityEngine.Debug.LogException);
        }

        public async UniTask ShowAsync<T, TModel>(TModel model, Action<T> afterShowCallback = null)
            where T : class, IModelUIScreenPresenter<TModel>
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            if (afterShowCallback == null)
                await presenter.Show(model);
            else
                await presenter.Show(model).ContinueWith(() => afterShowCallback(presenter));
        }

        public void Show<T, TModel>(TModel model, Action<T> afterShowCallback = null)
            where T : class, IModelUIScreenPresenter<TModel>
        {
            var presenter = PresenterViewBindingHelper.GetPresenter<T>(_di);
            if (presenter == null)
            {
                Debug.LogError($"[UI] Presenter {typeof(T).Name}: Unable to resolve");
                return;
            }

            if (afterShowCallback == null)
                presenter.Show(model).Forget(UnityEngine.Debug.LogException);
            else
                presenter.Show(model).ContinueWith(() => afterShowCallback(presenter))
                    .Forget(UnityEngine.Debug.LogException);
        }

        public void HideAll()
        {
            for (int i = _screens.Count - 1; i >= 0 && _screens.Count > 0; i--)
            {
                if (_screens[i] != _hud)
                {
                    _screens[i].Hide();
                }
            }
        }

        public bool HasOpenedScreens()
        {
            return !_screens.Contains(_hud);
        }

        public bool HasOpenedPopups()
        {
            return _popups.Count != 0;
        }

        public void HideAllPopups()
        {
            for (int i = _popups.Count - 1; i >= 0; i--)
                _popups[i].Hide();
        }

        public void HideAllExceptHUD()
        {
            //OnHide(_hud);
            HideAll();
            //_hud.Show();
        }

        public void HideAllWithHUD()
        {
            HideAll();
            _hud.Hide();
        }

        public void ShowHUD()
        {
            _hud.Show();
        }

        public bool IsScreenShown<T>() where T : class, IUIScreenPresenter
            => _screens.Any(screen => screen is T);

        public bool IsPopupShown<T>() where T : class, IUIScreenPresenter
            => _popups.Any(screen => screen is T);

        // This is used from guide ActionFlow in 'CloseScreen' action to check if screen is shown before hiding it
        public bool IsScreenShown(Type type)
            => _screens.Any(screen => ((object)screen).GetType() == type);

        public bool IsPopupShown(Type type)
            => _popups.Any(popup => ((object)popup).GetType() == type);

        void IInitializable.Initialize()
        {
            _disposables = new();

            ScreenOpened = new();
            ScreenClosed = new();
            ScreenOpened.AddTo(_disposables);
            ScreenClosed.AddTo(_disposables);

            IUIScreenPresenter.OnShow.Subscribe(OnShow).AddTo(_disposables);
            IUIScreenPresenter.OnHide.Subscribe(OnHide).AddTo(_disposables);
            _hud.Show();
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
        }

        private void OnShow(IUIScreenPresenter screen)
        {
            Debug.Log($"[OnShowScreen]: {screen.ScreenType} : {screen.GetType().Name}");
            if (screen.ScreenType is EScreenType.Screen or EScreenType.ScreenWithHud && !_screens.Contains(screen))
            {
                _screens.Add(screen);
            }
            else if (screen.ScreenType is EScreenType.Popup && !_popups.Contains(screen))
            {
                _popups.Add(screen);
            }

            ScreenOpened.Execute(screen);
        }

        private void OnHide(IUIScreenPresenter screen)
        {
            if (_screens.Count == 1 && screen == _hud)
            {
                return;
            }
            
            if (screen.ScreenType is EScreenType.Screen or EScreenType.ScreenWithHud && !_screens.Contains(screen))
            {
                _screens.Remove(screen);
            }
            else if (screen.ScreenType is EScreenType.Popup && !_popups.Contains(screen))
            {
                _popups.Remove(screen);
            }

            Debug.Log($"[OnHideScreen]: {screen.GetType().Name}");
            if (screen.ScreenType is EScreenType.Screen or EScreenType.ScreenWithHud)
                OnHide(screen, _screens);
            else if (screen.ScreenType == EScreenType.Popup)
                OnHide(screen, _popups);
        }

        private void OnHide(IUIScreenPresenter screen, List<IUIScreenPresenter> stack)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (!i.InBounds(stack))
                    break;
                var lastScreen = stack[i];

                ScreenClosed.Execute(lastScreen);

                stack.RemoveAt(i);

                if (lastScreen == screen || lastScreen == _hud)
                    break;
                lastScreen.Hide().Forget(UnityEngine.Debug.LogException);
            }
        }
    }
}