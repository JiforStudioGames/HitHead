using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
	public interface IUIScreenPresenter : IDisposable
	{
		public static ReactiveCommand<IUIScreenPresenter> OnShow { get; } = new();
		public static ReactiveCommand<IUIScreenPresenter> OnHide { get; } = new();

		EScreenType ScreenType { get; }
		UIScreenView BaseView { get; }
		UniTask Hide();
		UniTask HideTarget();
		public Component ViewObj { get; }
		void SetSubPresenters(List<object> subPresenters);
	}

	public interface ISimpleUIScreenPresenter : IUIScreenPresenter
	{
		UniTask Show();
	}

	public interface IModelUIScreenPresenter<TModel> : IUIScreenPresenter
	{
		UniTask Show(TModel model);
	}

	public abstract class UIScreenPresenterBase<TView> : IUIScreenPresenter, ISimpleUIScreenPresenter
		where TView : UIScreenView
	{
		
		public EScreenType ScreenType => _screenType;
		public UIScreenView BaseView => _view;

		[Inject] private EScreenType _screenType;	
		[Inject] protected TView _view;
		
		protected UniTask Completed => UniTask.CompletedTask;
		public TView View => _view;
		public Component ViewObj => _view;
		public virtual bool ShowSubPresenter { get; } = true;
		public virtual UniTask Show()
		{
			return UniTask.CompletedTask;
		}

		public virtual void SetSubPresenters(List<object> subPresenters)
		{
			
		}

		public async UniTask Hide()
		{
			if (_view && _screenType != EScreenType.ScreenWithHud)
			{
				await HideView();
				await AfterHide();
                Dispose();
                IUIScreenPresenter.OnHide.Execute(this);
            }
		}
		
		protected virtual async UniTask HideView()
		{
			await _view.Hide();
		}
		
		public async UniTask HideTarget()
		{
			if (_view && _screenType != EScreenType.ScreenWithHud)
			{
				await _view.Hide();
				await AfterHide();
                Dispose();
            }
        }

		public void HideAndForget() => Hide().Forget(UnityEngine.Debug.LogException);

		public abstract void Dispose();
		protected virtual UniTask AfterHide() => UniTask.CompletedTask;
	}
}