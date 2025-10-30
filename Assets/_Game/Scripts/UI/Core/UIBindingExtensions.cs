using Zenject;

namespace Game.Scripts.UI.Core
{
    public static class UIBindingExtensions
    {
        public static PresenterBinderGeneric<TPresenter> BindPresenter<TPresenter>(this DiContainer di)
            where TPresenter : IUIScreenPresenter
        {
            return new PresenterBinderGeneric<TPresenter>(di.StartBinding(), di);
        }

        public static SubPresenterBinderGeneric<TSubPresenter> BindSubPresenter<TSubPresenter>(this DiContainer di, bool autoShow = true)
            where TSubPresenter : IUIScreenSubPresenter
        {
            return new SubPresenterBinderGeneric<TSubPresenter>(di.StartBinding(), di, autoShow);
        }

        public static SubPresenterCollectionBinderGeneric<TParentPresenter> BindSubPresentersFor<TParentPresenter>(
            this DiContainer di)
        {
            return new SubPresenterCollectionBinderGeneric<TParentPresenter>(di.StartBinding());
        }
    }
}