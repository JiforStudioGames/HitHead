using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Core
{
    public class PresenterViewBindingHelper
    {
        public static Dictionary<DiContainer, ByContainer> ByContainers = new();
        private static Subject<MonoBehaviour> _resolveViewSubject = new();

        public static IObservable<MonoBehaviour> OnResolveView => _resolveViewSubject;

        public static void AddPresentersOpened(Type presenterType, object presenter, DiContainer diContainer)
        {
            if (ByContainers.TryGetValue(diContainer, out var byContainer))
            {
                byContainer.PresentersOpened.Add(presenterType, presenter);
            }
        }

        public static void ClearByContainer(DiContainer diContainer)
        {
            if (ByContainers.TryGetValue(diContainer, out var byContainer))
            {
                ByContainers.Remove(diContainer);
                diContainer.ResolveAll<IDisposable>().ForEach(p =>
                {
                    try
                    {
                        p?.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"[PresenterViewBindingHelper]: Error = {e}");
                    }
                });
            }
        }

        public static void Clear()
        {
            var array = PresenterViewBindingHelper.ByContainers.ToArray();
            foreach (var itemPair in array)
            {
                ClearByContainer(itemPair.Key);
            }
        }

        public static T GetPresenter<T>(DiContainer diContainer)
            where T : class
        {
            return (T)GetPresenterInternal(typeof(T), diContainer);
        }

        public static T GetPresenter<T>(Type t, DiContainer diContainer, bool ignoreChildren = false, object fromInstance = null)
            where T : class
        {
            return (T)GetPresenterInternal(t, diContainer, ignoreChildren, false, fromInstance);
        }

        private static T GetOrLoad<T>(Type t)
            where T : class
        {
            foreach (var itemPair in PresenterViewBindingHelper.ByContainers)
            {
                foreach (var itemPair2 in itemPair.Value.Presenters)
                {
                    if (itemPair2.Value.PresenterType == t)
                    {
                        return (T)itemPair.Key.TryResolve(t);
                    }
                }
            }

            return default;
        }

        public static void AddSubPresenters(Type type, SubPresenterBindInfo info, DiContainer diContainer)
        {
            if (!PresenterViewBindingHelper.ByContainers.TryGetValue(diContainer, out var byContainer))
            {
                byContainer = new ByContainer();
                PresenterViewBindingHelper.ByContainers.Add(diContainer, byContainer);
            }

            if (!byContainer.SubPresenters.TryGetValue(type, out var list))
            {
                list = new List<SubPresenterBindInfo>();
                byContainer.SubPresenters.Add(type, list);
            }

            var found = list.FirstOrDefault(item => item.PresenterType == type);
            if (found == null)
            {
                list.Add(info);
            }
        }

        public static void AddPresenters(Type type, PresenterBindInfo info, DiContainer diContainer)
        {
            if (!PresenterViewBindingHelper.ByContainers.TryGetValue(diContainer, out var byContainer))
            {
                byContainer = new ByContainer();
                PresenterViewBindingHelper.ByContainers.Add(diContainer, byContainer);
            }

            if (!byContainer.Presenters.TryGetValue(type, out var _))
            {
                byContainer.Presenters.Add(type, info);
            }
        }

        private static object GetPresenterInternal(Type t, DiContainer diContainer, bool ignoreChildren = false, bool callFromChild = false, object fromInstance = null)
        {
            if (!PresenterViewBindingHelper.ByContainers.TryGetValue(diContainer, out var byContainer))
            {
                byContainer = new ByContainer();
                PresenterViewBindingHelper.ByContainers.Add(diContainer, byContainer);
            }

            var isFirstOpen = false;
            var isParent = false;

            if (!byContainer.PresentersOpened.TryGetValue(t, out var presenter))
            {
                isFirstOpen = true;
                if (byContainer.SubPresenters.TryGetValue(t, out var subPresentersList))
                {
                    var subPresenterObj = subPresentersList[0];

                    if (!callFromChild && !byContainer.PresentersOpened.TryGetValue(subPresenterObj.ParentPresenterType, out var parentPresenter))
                    {
                        GetPresenterInternal(subPresenterObj.ParentPresenterType, diContainer, false, true);
                    }
                    
                    subPresenterObj.OpenAction();
                    presenter = diContainer.TryResolve(t);
                }
                else if (byContainer.Presenters.TryGetValue(t, out var presenterObj))
                {
                    isParent = true;

                    if (fromInstance != null)
                    {
                        presenter = fromInstance;
                    }
                    else
                    {
                        presenterObj.OpenAction();
                        presenter = diContainer.TryResolve(t);

                        if (presenter == null)
                        {
                            presenter = GetOrLoad<object>(t);
                        }
                    }
                }

                if (presenter != null)
                {
                    if (presenter is IUIScreenPresenter { ViewObj: MonoBehaviour view })
                    {
                        _resolveViewSubject?.OnNext(view);
                    }
                    
                    if (byContainer.PresentersOpened.TryGetValue(t, out var _))
                    {
                        byContainer.PresentersOpened[t] = presenter;
                    }
                    else
                    {
                        byContainer.PresentersOpened.Add(t, presenter);
                    }
                }
            }

            var subs = byContainer.SubPresenters
                .Where(item => item.Value.FirstOrDefault(p => p.ParentPresenterType == t) != null)
                .Select(item => item.Value.FirstOrDefault(p => p.ParentPresenterType == t))
                .ToArray();

            if (!ignoreChildren)
            {
                foreach (var sub in subs)
                {
                    PresenterViewBindingHelper.GetPresenterInternal(sub.PresenterType, diContainer, false, callFromChild);
                }
            }
            

            if (isFirstOpen && isParent)
            {
                var list = new List<object>();
                foreach (var byContainerSubPresenter in byContainer.SubPresenters)
                {
                    var subPresenterObj = byContainerSubPresenter.Value[0];
                    if (subPresenterObj.ParentPresenterType == t)
                    {
                        subPresenterObj.OpenAction();
                        var subPresenter = diContainer.TryResolve(subPresenterObj.PresenterType);
                        list.Add(subPresenter);
                    }
                }
                // if (byContainer.SubPresenters.TryGetValue(t, out var subPresentersList2))
                // {
                //     var subPresenterObj = subPresentersList2[0];
                //
                //     if (!callFromChild &&
                //         !byContainer.PresentersOpened.TryGetValue(subPresenterObj.ParentPresenterType,
                //             out var parentPresenter))
                //     {
                //         GetPresenterInternal(subPresenterObj.ParentPresenterType, diContainer, false, true);
                //     }
                //
                //     subPresenterObj.OpenAction();
                //     var subPresenter = diContainer.TryResolve(t);
                //     list.Add((IUIScreenSubPresenter)subPresenter);
                // }

                ((IUIScreenPresenter)presenter).SetSubPresenters(list);
            }

            return presenter;
        }

        public class ByContainer
        {
            public Dictionary<Type, List<SubPresenterBindInfo>> SubPresenters = new();
            public Dictionary<Type, PresenterBindInfo> Presenters = new();
            public Dictionary<Type, object> PresentersOpened = new();
        }
    }
}