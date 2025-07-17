using ModestTree;
using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    [NoReflectionBaking]
	public class PresenterBindInfo
	{
		public Action OpenAction { get; set; }
		public Type PresenterType { get; set; }

		public string PrefabPath { get; set; } = "";
		public Func<UIRoot, Transform> ParentTransformGetter { get; set; }
		public EScreenType ScreenType { get; set; }
		public bool ViewFromResolve { get; set; } = false;
	}

    [NoReflectionBaking]
	public class PresenterBindingFinalizer : IBindingFinalizer
	{
		public BindingInheritanceMethods BindingInheritanceMethod => BindingInheritanceMethods.None;
		public readonly PresenterBindInfo Info;

		public PresenterBindingFinalizer(PresenterBindInfo presenterBindInfo)
		{
			Info = presenterBindInfo;
		}

		public void FinalizeBinding(DiContainer container)
		{
			Assert.That(!string.IsNullOrEmpty(Info.PrefabPath) || Info.ViewFromResolve, $"Please, choose path to view prefab: {Info.PresenterType.PrettyName()}");
			Assert.That(Info.ParentTransformGetter != null, $"Please, use AsScreen, AsPopup or AsCameraScreen: {Info.PresenterType.PrettyName()}");

			var opened = false;
			Info.OpenAction = () =>
			{
				if (opened)
				{
					return;
				}

				opened = true;
				if (!Info.ViewFromResolve)
					RegisterViewPrefab(container);
				RegisterPresenter(container);
			};
		}

		private void RegisterViewPrefab(DiContainer container)
		{
			var viewType = Info.PresenterType.GetArgumentsOfInheritedOpenGenericClass(typeof(UIScreenPresenterBase<>))[0];
			var gameObjectBindInfo = new GameObjectCreationParameters()
			{
				ParentTransformGetter = ic => Info.ParentTransformGetter(ic.Container.Resolve<UIRoot>()),
				Name = viewType.Name.Substring(2).SplitPascalCase(),
			};

			var prefabCreator = new PrefabInstantiatorCached(
				new PrefabInstantiator(
					container,
					gameObjectBindInfo,
					viewType,
					new List<Type>() { viewType },
					new List<TypeValuePair>(),
					new PrefabProviderResource(Info.PrefabPath),
					(ic, o) =>
					{
						var go = ((MonoBehaviour)o).gameObject;
						go.SetActive(false);
					}));

			container.RegisterProvider(new BindingId(viewType, null),
				null, new GetFromPrefabComponentProvider(viewType, prefabCreator, true), true);
		}

		private void RegisterPresenter(DiContainer container)
		{
			var cached = new CachedProvider(new TransientProvider(Info.PresenterType, container, 
				new [] { new TypeValuePair(typeof(EScreenType), Info.ScreenType) },
				$"Bind Presenter {Info.PresenterType.PrettyName()}", null, null));
			container.RegisterProvider(new BindingId(Info.PresenterType, null),
				null, cached, true);
			container.RegisterProvider(new BindingId(typeof(IDisposable), null),
				null, cached, true);
		}
	}

    [NoReflectionBaking]
	public class PresenterBinderGeneric<TPresenter>
	{
		private BindStatement _bindStatement;
		protected PresenterBindInfo _info;

		public PresenterBinderGeneric(BindStatement bindStatement, DiContainer diContainer)
		{
			_info = new PresenterBindInfo();
			_info.PresenterType = typeof(TPresenter);
			bindStatement.SetFinalizer(new PresenterBindingFinalizer(_info));
			PresenterViewBindingHelper.AddPresenters(typeof(TPresenter), _info, diContainer);
		}

		public PresenterViewBinderGeneric WithViewFromPrefab(string prefabPath)
		{
			_info.PrefabPath = prefabPath;
			return new PresenterViewBinderGeneric(_info);
		}
		
		public PresenterViewBinderGeneric WithViewFromResolve()
		{
			_info.ViewFromResolve = true;
			return new PresenterViewBinderGeneric(_info);
		}
	}

	[NoReflectionBaking]
	public class PresenterViewBinderGeneric
	{
		private PresenterBindInfo _info;

		public PresenterViewBinderGeneric(PresenterBindInfo info) =>
			_info = info;

		public void AsPopup()
		{
			_info.ScreenType = EScreenType.Popup;
			_info.ParentTransformGetter = uiRoot => uiRoot.Popups.transform;
		}
		
		public PresenterWithHUDBinderGeneric AsScreen()
		{
			_info.ScreenType = EScreenType.Screen;
			_info.ParentTransformGetter = uiRoot => uiRoot.Screens.transform;
			return new PresenterWithHUDBinderGeneric(_info);
		}
		
		public PresenterWithHUDBinderGeneric AsLayerUnderScreens()
		{
			_info.ScreenType = EScreenType.Screen;
			_info.ParentTransformGetter = uiRoot => uiRoot.LayerUnderScreens.transform;
			return new PresenterWithHUDBinderGeneric(_info);
		}
		
		public PresenterWithHUDBinderGeneric AsLayerOverScreens()
		{
			_info.ScreenType = EScreenType.Screen;
			_info.ParentTransformGetter = uiRoot => uiRoot.LayerOverScreens.transform;
			return new PresenterWithHUDBinderGeneric(_info);
		}
	}

	[NoReflectionBaking]
	public class PresenterWithHUDBinderGeneric
	{
		private PresenterBindInfo _info;

		public PresenterWithHUDBinderGeneric(PresenterBindInfo info) =>
			_info = info;

		public void WithHUD() =>
			_info.ScreenType = EScreenType.ScreenWithHud;
	}
}