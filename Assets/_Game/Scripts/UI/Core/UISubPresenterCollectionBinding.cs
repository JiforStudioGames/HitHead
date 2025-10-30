using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using ModestTree;
using Zenject;

namespace Game.Scripts.UI.Core
{
    [NoReflectionBaking]
    public class SubPresenterCollectionBindInfo
    {
        public Type ParentPresenterType { get; set; }
        public List<Type> PresenterTypes { get; set; } = new();
        public List<Type> BaseTypes { get; set; } = new();
    }
    
    [NoReflectionBaking]
	public class SubPresenterCollectionBindingFinalizer : IBindingFinalizer
	{
		public BindingInheritanceMethods BindingInheritanceMethod => BindingInheritanceMethods.None;
		public readonly SubPresenterCollectionBindInfo Info;

		public SubPresenterCollectionBindingFinalizer(SubPresenterCollectionBindInfo presenterBindInfo)
		{
			Info = presenterBindInfo;
		}

		public void FinalizeBinding(DiContainer container)
		{
			RegisterView(container);
			RegisterPresenter(container);
		}

		private void RegisterView(DiContainer container)
		{
			foreach (var presenterType in Info.PresenterTypes)
			{
				var viewType = presenterType.GetArgumentsOfInheritedOpenGenericClass(typeof(UIScreenSubPresenterBase<>))[0];
				var parentViewType = Info.ParentPresenterType.GetArgumentsOfInheritedOpenGenericClass(typeof(UIScreenPresenterBase<>))[0];
				
				container.RegisterProvider(new BindingId(viewType, null),
					(ic) => ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(presenterType),
					new GetFromGameObjectGetterComponentProvider(viewType, 
						(ic) =>
						{
							Assert.That(ic.Container.HasBinding(parentViewType), 
								$"Parent View not found in DiContainer {parentViewType.PrettyName()} for {presenterType.PrettyName()}");
							var parentResolve = ((MonoBehaviour)ic.Container.Resolve(parentViewType));
							var component = parentResolve.GetComponentInChildren(viewType, true);
							Assert.IsNotNull(component, 
								$"Component {viewType.PrettyName()} not found in {parentViewType.PrettyName()}");
							return component.gameObject;
						},
						true), false);
			}
		}

		private void RegisterPresenter(DiContainer container)
		{
			for (int i = 0; i < Info.PresenterTypes.Count; i++)
			{
				var presenterType = Info.PresenterTypes[i];
				var baseType = Info.BaseTypes[i];
				var cached = new CachedProvider(new TransientProvider(presenterType, container, new TypeValuePair[0],
					$"Bind Presenter {presenterType.PrettyName()}", null, null));
				container.RegisterProvider(new BindingId(presenterType, null),
					(ic) => ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(Info.ParentPresenterType),
					cached, false);
				container.RegisterProvider(new BindingId(baseType, null),
					(ic) => ic.ObjectType != null && ic.ObjectType.DerivesFromOrEqual(Info.ParentPresenterType),
					cached, false);
			}
		}
	}

    [NoReflectionBaking]
	public class SubPresenterCollectionBinderGeneric<TPresenter>
	{
		private BindStatement _bindStatement;
		private SubPresenterCollectionBindInfo _info;

		public SubPresenterCollectionBinderGeneric(BindStatement bindStatement)
		{
			_bindStatement = bindStatement;
			_info = new SubPresenterCollectionBindInfo();
			_info.ParentPresenterType = typeof(TPresenter);
			_bindStatement.SetFinalizer(new SubPresenterCollectionBindingFinalizer(_info));
		}

		public SubPresenterCollectionBinderGeneric<TPresenter> To<TSubPresenter>()
		{
			var subPresenterType = typeof(TSubPresenter);
			var parentPresenterType = typeof(TPresenter);
			var openSubScreenType = typeof(IUIScreenSubPresenter<>);
			var openParentScreenType = typeof(IModelUIScreenPresenter<>);
			
			Assert.That(!subPresenterType.DerivesFrom(typeof(UIScreenSubPresenterBase<>)),
				$"SubPresenter doesn't type UIScreenSubPresenter");

			if (subPresenterType.DerivesFrom(openSubScreenType))
			{
				Assert.That(!parentPresenterType.DerivesFrom(openParentScreenType),
					"SubPresenter doesn't match with Parent Presenter: " +
					$"{subPresenterType.PrettyName()} to {parentPresenterType.PrettyName()}");

				var subModelType = subPresenterType.GetArgumentsOfInheritedOpenGenericClass(openSubScreenType)[0];
				var parentModelType = parentPresenterType.GetArgumentsOfInheritedOpenGenericClass(openParentScreenType)[0];

				Assert.That(subModelType != parentModelType, 
					"SubPresenter Model doesn't match with Parent Presenter Model: " + 
					$"{subPresenterType.PrettyName()} to {parentPresenterType.PrettyName()}");

				_info.BaseTypes.Add(subPresenterType.GetGenericBaseType(openSubScreenType));
			}
			else
			{
				_info.BaseTypes.Add(typeof(IUIScreenSubPresenter));
			}

			_info.PresenterTypes.Add(subPresenterType);
			return this;
		}
	}
}