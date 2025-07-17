using ModestTree;
using System;
using Extensions;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    [NoReflectionBaking]
	public class SubPresenterBindInfo
	{
		public Action OpenAction { get; set; }
		public Type ParentPresenterType { get; set; }
		public Type PresenterType { get; set; }
		public Type BaseType { get; set; }
		public bool BindPresenterToAll { get; set; }
		public bool BindViewToAll { get; set; }
		public DiContainer DiContainer { get; set; }
		public bool AutoShow { get; set; }
	}

    [NoReflectionBaking]
	public class SubPresenterBindingFinalizer : IBindingFinalizer
	{
		public BindingInheritanceMethods BindingInheritanceMethod => BindingInheritanceMethods.None;
		public readonly SubPresenterBindInfo Info;

		public SubPresenterBindingFinalizer(SubPresenterBindInfo presenterBindInfo)
		{
			Info = presenterBindInfo;
		}

		public void FinalizeBinding(DiContainer container)
		{
			Assert.IsNotNull(Info.ParentPresenterType, $"Please, use ToParent to assign SubPresenter to ParentPresenter {Info.PresenterType.PrettyName()}");

			var opened = false;
			Info.OpenAction = () =>
			{
				if (opened)
				{
					return;
				}

				opened = true;
				RegisterView(container);
				RegisterPresenter(container);
			};
		}

		private void RegisterView(DiContainer container)
		{
			var viewType = Info.PresenterType.GetArgumentsOfInheritedOpenGenericClass(typeof(UIScreenSubPresenterBase<>))[0];
			var parentViewType = Info.ParentPresenterType.GetArgumentsOfInheritedOpenGenericClass(typeof(UIScreenPresenterBase<>))[0];

			container.RegisterProvider(new BindingId(viewType, null),
				(ic) =>
				{
					return true;
					if (Info.BindViewToAll)
					{
						return ic.ObjectType != null;
					}
					return ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(Info.PresenterType);
				},
				new GetFromGameObjectGetterComponentProvider(viewType, 
					(ic) =>
					{
						Assert.That(ic.Container.HasBinding(parentViewType), $"Parent View not found in DiContainer {parentViewType.PrettyName()}; {viewType.PrettyName()}");
						var parentResolve = ((MonoBehaviour)ic.Container.Resolve(parentViewType));
						var component = parentResolve.GetComponentInChildren(viewType, true);
						Assert.IsNotNull(component, $"Component {viewType.PrettyName()} not found in {parentViewType.PrettyName()}");
						component.gameObject.SetActive(Info.AutoShow);
						return component.gameObject;
					},
					true), true);
		}

		private void RegisterPresenter(DiContainer container)
		{
			var cached = new CachedProvider(new TransientProvider(Info.PresenterType, container, new TypeValuePair[0],
				$"Bind Presenter {Info.PresenterType.PrettyName()}", null, (ctx, obj) =>
				{
			
				}));
			container.RegisterProvider(new BindingId(Info.PresenterType, null),
				(ic) =>
				{
					return true;
					if (Info.BindPresenterToAll)
					{
						return ic.ObjectType != null;
					}
					return ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(Info.ParentPresenterType);
				},
				cached, true);
			container.RegisterProvider(new BindingId(Info.BaseType, null),
				(ic) => ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(Info.ParentPresenterType),
				cached, true);
		}
	}

    [NoReflectionBaking]
	public class SubPresenterBinderGeneric<TPresenter>
	{
		private BindStatement _bindStatement;
		private SubPresenterBindInfo _info;

		private Type presenterType;

		public SubPresenterBinderGeneric(BindStatement bindStatement, DiContainer diContainer, bool autoShow)
		{
			_bindStatement = bindStatement;
			_info = new SubPresenterBindInfo();
			_info.PresenterType = typeof(TPresenter);
			_info.DiContainer = diContainer;
			_info.AutoShow = autoShow;
			_bindStatement.SetFinalizer(new SubPresenterBindingFinalizer(_info));
			PresenterViewBindingHelper.AddSubPresenters(typeof(TPresenter), _info, diContainer);
		}

		public SubPresenterBinderGeneric<TPresenter> ToParent<TParentPresenter>()
		{
			presenterType = typeof(TPresenter);
			var parentPresenterType = typeof(TParentPresenter);
			var openSubScreenType = typeof(IUIScreenSubPresenter<>);
			var openParentScreenType = typeof(IModelUIScreenPresenter<>);

			if (presenterType.DerivesFrom(openSubScreenType))
			{
				Assert.That(!parentPresenterType.DerivesFrom(openParentScreenType),
					"SubPresenter doesn't match with Parent Presenter: " +
					$"{presenterType.PrettyName()} to {parentPresenterType.PrettyName()}");

				var subModelType = presenterType.GetArgumentsOfInheritedOpenGenericInterface(openSubScreenType)[0];
				var parentModelType = parentPresenterType.GetArgumentsOfInheritedOpenGenericInterface(openParentScreenType)[0];

				Assert.That(subModelType != parentModelType, "SubPresenter Model doesn't match with Parent Presenter Model: " +
					$"{presenterType.PrettyName()} to {parentPresenterType.PrettyName()}");

				_info.BaseType = presenterType.GetGenericBaseType(openSubScreenType);
			}
			else
			{
				Assert.That(!presenterType.DerivesFrom(typeof(UIScreenSubPresenterBase<>)),
					$"SubPresenter doesn't type UIScreenSubPresenter");
				_info.BaseType = typeof(IUIScreenSubPresenter);
			}
			_info.ParentPresenterType = parentPresenterType;
			return this;
		}

		public SubPresenterBinderGeneric<TPresenter> BindPresenterToAll()
		{
			_info.BindPresenterToAll = true;
			return this;
		}
		
		public SubPresenterBinderGeneric<TPresenter> BindPresenterViewToAll()
		{
			_info.BindViewToAll = true;
			return this;
		}
	}
}