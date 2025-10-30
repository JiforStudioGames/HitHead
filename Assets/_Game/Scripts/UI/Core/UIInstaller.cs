using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public abstract class UIInstaller<THudView, THudPresenter> : MonoInstaller
        where THudPresenter : UIScreenPresenter<THudView>, ISimpleUIScreenPresenter
        where THudView : UIScreenView
    {
        protected abstract string HudPrefabPath { get; }
        protected virtual string UIRootPrefabPath => "Systems/UI Root";

        public sealed override void InstallBindings()
        {
            InstallBindingsInternal();
            
            Container.Bind<UIRoot>()
                .FromComponentInNewPrefabResource(UIRootPrefabPath)
                .AsSingle();
            Container.BindInterfacesAndSelfTo<UINavigator>()
                .AsSingle();
            Container.BindInitializableExecutionOrder<UINavigator>(-200);
            Container.Bind<THudView>()
                .FromComponentInNewPrefabResource(HudPrefabPath)
                .WithGameObjectName("HUD")
                .UnderTransform(ic => ic.Container.Resolve<UIRoot>().Screens.transform)
                .AsSingle()
                .OnInstantiated<THudView>((ic, o) =>
                {
                    o.gameObject.SetActive(true);

                    var presenterBindInfo = new PresenterBindInfo
                    {
                        PresenterType = typeof(THudPresenter),
                        ScreenType = EScreenType.ScreenWithHud,
                        ViewFromResolve = true,
                        OpenAction = () => {},
                    };
                    PresenterViewBindingHelper.AddPresenters(typeof(THudPresenter), presenterBindInfo, Container);

                    var pres = Container.Resolve<THudPresenter>();
                    PresenterViewBindingHelper.GetPresenter<THudPresenter>(typeof(THudPresenter), Container, false, pres);
                });

            Container.Bind<THudPresenter>()
                .AsSingle()
                .WithArgumentsExplicit(new[] { new TypeValuePair(typeof(EScreenType), EScreenType.Screen) });
            Container.Bind<ISimpleUIScreenPresenter>()
                .WithId("HUD")
                .FromResolveGetter<THudPresenter>(t => t)
                .AsSingle();
            Container.BindInterfacesAndSelfTo<UIVisibilityChecker>().AsSingle();
            
            InstallBindingsLate();
        }

        protected virtual void InstallBindingsLate()
        {
        }

        protected abstract void InstallBindingsInternal();
    }
}