using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public interface IUIScreenSubPresenter
    {
        UniTask BeforeShow(CompositeDisposable disposables);
        UniTask AfterShow(CompositeDisposable disposables);
        UniTask AfterHide();
    }

    public interface IUIScreenSubPresenter<TModel>
    {
        UniTask BeforeShow(TModel model, CompositeDisposable disposables);
        UniTask AfterShow(TModel model, CompositeDisposable disposables);
        UniTask AfterHide();
    }

    public abstract class UIScreenSubPresenterBase<TView> : ISimpleUIScreenPresenter
        where TView : Component
    {
        [Inject] protected TView _view;

        public TView View => _view;
		
        public virtual UniTask AfterHide() => UniTask.CompletedTask;
        public virtual void Dispose()
        {
			
        }

        public EScreenType ScreenType { get; }
        public UIScreenView BaseView { get; }

        public UniTask Hide()
        {
            _view.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public UniTask HideTarget()
        {
            return UniTask.CompletedTask;
        }

        public Component ViewObj => _view;
        public void SetSubPresenters(List<object> subPresenters)
        {
			
        }

        public virtual UniTask Show()
        {
            _view.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
    }

    public abstract class UIScreenSubPresenter<TView> : UIScreenSubPresenterBase<TView>, IUIScreenSubPresenter
        where TView : Component
    {
        public abstract UniTask BeforeShow(CompositeDisposable disposables);
        public virtual UniTask AfterShow(CompositeDisposable disposables) => UniTask.CompletedTask;
    }

    public abstract class UIScreenSubPresenter<TView, TModel> : UIScreenSubPresenterBase<TView>, IUIScreenSubPresenter<TModel>
        where TView : Component
    {
        public abstract UniTask BeforeShow(TModel model, CompositeDisposable disposables);
        public virtual UniTask AfterShow(TModel model, CompositeDisposable disposables) => UniTask.CompletedTask;
    }
}