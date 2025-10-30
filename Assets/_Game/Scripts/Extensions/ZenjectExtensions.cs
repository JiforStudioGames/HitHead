using System;
using System.Text;
using UnityEngine;
using Zenject;

namespace Extensions
{
    public static class ZenjectExtensions
    {
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromRootComponent<T>(
            this ConcreteIdBinderGeneric<T> binder, MonoBehaviour target) =>
            binder.FromInstance(target.GetComponent<T>());

		public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromChildComponent<T>(
			this ConcreteIdBinderGeneric<T> binder, MonoBehaviour target) =>
			binder.FromInstance(target.GetComponentInChildren<T>(includeInactive: true));
		public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromChildComponent<T>(
			this FromBinderNonGeneric binder, MonoBehaviour target) =>
			binder.FromInstance(target.GetComponentInChildren<T>(includeInactive: true));

		public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromChildComponents<T>(
			this ConcreteIdBinderGeneric<T> binder, MonoBehaviour target) =>
			binder.FromMethodMultiple((ic) => target.GetComponentsInChildren<T>(includeInactive: true));

		public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromChildComponents<T>(
			this ConcreteBinderGeneric<T> binder, Func<InjectContext, MonoBehaviour> target) =>
			binder.FromMethodMultiple(ic => target(ic).GetComponentsInChildren<T>(includeInactive: true));

		public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromChildComponent<T>(
			this ConcreteBinderGeneric<T> binder, Func<InjectContext, MonoBehaviour> target) =>
			binder.FromMethod(ic => target(ic).GetComponentInChildren<T>(includeInactive: true));

		public static Type[] GetArgumentsOfInheritedOpenGenericClass(this Type type, Type openGenericType)
		{
			var currentType = type;
			while (currentType.BaseType != null)
			{
				currentType = currentType.BaseType;
				if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == openGenericType)
					return currentType.GetGenericArguments();
			}
			return new Type[0];
		}
		public static Type[] GetArgumentsOfInheritedOpenGenericInterface(this Type type, Type openGenericType)
		{
			foreach (var i in type.GetInterfaces())
				if (i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
					return i.GetGenericArguments();
			return new Type[0];
		}

		public static Type GetGenericBaseType(this Type type, Type openGenericType)
		{
			var nestedType = type.GetNestedType(openGenericType.Name);
			return nestedType;
		}
		
		public static string SplitPascalCase(this string str)
		{
			StringBuilder builder = new StringBuilder(str.Length);

			builder.Append(str[0]);
			for (int i = 1; i < str.Length; i++)
			{
				if (char.IsUpper(str[i]) && !char.IsUpper(str[i - 1]))
					builder.Append(" ");
				builder.Append(str[i]);
			}

			return builder.ToString();
		}
	}
}