using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Game.Scripts.UI.Core
{
    public abstract class UIScreenPresenter<TView> : UIScreenPresenterBase<TView>, ISimpleUIScreenPresenter
        where TView : UIScreenView
    {
        private List<IUIScreenSubPresenter> _subPresenters;

        private CompositeDisposable _disposables;
        private CancellationTokenSource _cancellationTokenSource;
        public CompositeDisposable Disposables => _disposables;

        public override void SetSubPresenters(List<object> subPresenters)
        {
            _subPresenters = subPresenters.Select(sp => (IUIScreenSubPresenter)sp).ToList();
        }

        public override async UniTask Show()
        {
            if (_disposables != null && !_disposables.IsDisposed)
                return;

            _disposables?.Dispose();
            _disposables = new();
            _cancellationTokenSource = new();

            Disposable.Create(() =>
            {
                _cancellationTokenSource?.Cancel(throwOnFirstException: false);
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }).AddTo(_disposables);

            await BeforeShow(_disposables);

            if (_subPresenters != null && ShowSubPresenter)
            {
                foreach (var subPresenter in _subPresenters)
                {
                    try
                    {
                        await subPresenter.BeforeShow(_disposables);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            await _view.Show();
            await AfterShow(_disposables);

            if (_subPresenters != null && ShowSubPresenter)
            {
                foreach (var subPresenter in _subPresenters)
                {
                    try
                    {
                        await subPresenter.AfterShow(_disposables);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            IUIScreenPresenter.OnShow.Execute(this);
        }

        public void ShowAndForget() => Show().Forget();

        protected virtual void OnDispose()
        {
        }

        public sealed override void Dispose()
        {
            OnDispose();
            _disposables?.Dispose();
            _disposables = null;
        }

        protected abstract UniTask BeforeShow(CompositeDisposable disposables);
        protected virtual UniTask AfterShow(CompositeDisposable disposables) => UniTask.CompletedTask;

        protected override async UniTask AfterHide()
        {
            if (_subPresenters != null)
            {
                foreach (var presenter in _subPresenters)
                    await presenter.AfterHide().AttachExternalCancellation(_cancellationTokenSource.Token);
            }
        }
    }

    public abstract class UIScreenPresenter<TView, TModel> : UIScreenPresenterBase<TView>,
        IModelUIScreenPresenter<TModel>
        where TView : UIScreenView
    {
        private List<IUIScreenSubPresenter> _subPresenters;
        private List<IUIScreenSubPresenter<TModel>> _subPresentersArgs;

        public override void SetSubPresenters(List<object> subPresenters)
        {
            _subPresenters = subPresenters.Select(sp => sp as IUIScreenSubPresenter)
                .Where(sp => sp != null)
                .ToList();
            _subPresentersArgs = subPresenters.Select(sp => sp as IUIScreenSubPresenter<TModel>)
                .Where(sp => sp != null)
                .ToList();
        }

        private CompositeDisposable _disposables;
        private CancellationTokenSource _cancellationTokenSource;
        protected CompositeDisposable Disposables => _disposables;
        protected CancellationToken ScreenCancellationToken { get; private set; } = CancellationToken.None;

        protected Action _initiatorBackAction;

        public async UniTask Show(TModel model)
        {
            if (_disposables != null && !_disposables.IsDisposed)
                return;

            _disposables?.Dispose();
            _disposables = new();
            _cancellationTokenSource = new();
            ScreenCancellationToken = _cancellationTokenSource.Token;

            Disposable.Create(() =>
            {
                _cancellationTokenSource?.Cancel(throwOnFirstException: false);
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }).AddTo(_disposables);

            await BeforeShow(model, _disposables).AttachExternalCancellation(_cancellationTokenSource.Token);

            foreach (IUIScreenSubPresenter subPresenter in _subPresenters)
            {
                if (ShowSubPresenter)
                {
                    await subPresenter.BeforeShow(_disposables)
                        .AttachExternalCancellation(_cancellationTokenSource.Token);
                }
            }

            foreach (var subPresenter in _subPresentersArgs)
            {
                if (ShowSubPresenter)
                {
                    await subPresenter.BeforeShow(model, _disposables)
                        .AttachExternalCancellation(_cancellationTokenSource.Token);
                }
            }

            await ShowView().AttachExternalCancellation(_cancellationTokenSource.Token);

            await AfterShow(model, _disposables).AttachExternalCancellation(_cancellationTokenSource.Token);

            foreach (var subPresenter in _subPresenters)
            {
                if (ShowSubPresenter)
                {
                    await subPresenter.AfterShow(_disposables)
                        .AttachExternalCancellation(_cancellationTokenSource.Token);
                }
            }

            foreach (var subPresenter in _subPresentersArgs)
            {
                if (ShowSubPresenter)
                {
                    await subPresenter.AfterShow(model, _disposables)
                        .AttachExternalCancellation(_cancellationTokenSource.Token);
                }
            }

            IUIScreenPresenter.OnShow.Execute(this);
        }

        protected virtual async UniTask ShowView()
        {
            await _view.Show();
        }

        public void ShowAndForget(TModel model, Action initiatorBackAction = null)
        {
            _initiatorBackAction = initiatorBackAction;
            Show(model).Forget();
        }

        public override void Dispose()
        {
            if (_disposables == null)
                return;
            _disposables.Dispose();
            _disposables = null;
        }

        protected virtual UniTask AfterShow(TModel model, CompositeDisposable disposables) => UniTask.CompletedTask;

        protected abstract UniTask BeforeShow(TModel researchModel, CompositeDisposable disposables);

        protected override async UniTask AfterHide()
        {
            foreach (var presenter in _subPresenters)
                await presenter.AfterHide().AttachExternalCancellation(ScreenCancellationToken);
            foreach (var presenter in _subPresentersArgs)
                await presenter.AfterHide().AttachExternalCancellation(ScreenCancellationToken);
        }
    }
}