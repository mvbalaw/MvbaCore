//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using CodeQuery;

namespace MvbaCore
{
	public static class Reflection
	{
		public enum AccessorType
		{
			Field,
			Property
		}

		private static readonly LruCache<Type, Dictionary<string, GenericGetter>> _getters = new LruCache<Type, Dictionary<string, GenericGetter>>(50);
		private static readonly LruCache<Type, Dictionary<string, GenericSetter>> _setters = new LruCache<Type, Dictionary<string, GenericSetter>>(50);

		public static bool CouldBeNull(Type type)
		{
			if (!type.IsValueType)
			{
				return true;
			}

			if (IsNullableValueType(type))
			{
				// e.g. decimal?
				return true;
			}

			return false;
		}

		public static List<string> GetArguments<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression == null)
			{
				throw new ArgumentException("expression must be in the form: (Foo instance) => instance.Method");
			}
			var arguments = methodCallExpression.Arguments;
			return arguments.Select(p => Expression.Lambda(p).Compile().DynamicInvoke().ToString()).ToList();
		}

		public static T GetAttribute<T>(Assembly assembly)
		{
			return (T)assembly.GetCustomAttributes(typeof(T), false)[0];
		}

		public static string GetCamelCaseMultiLevelPropertyName(params string[] propertyNames)
		{
			return GetMultiLevelPropertyName(propertyNames).ToCamelCase();
		}

		public static string GetCamelCasePropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			return GetPropertyName(expression).ToCamelCase();
		}

		public static string GetCamelCasePropertyNameWithPrefix<T, TReturn>(Expression<Func<T, TReturn>> expression, string prefix)
		{
			return (prefix + GetPropertyName(expression)).ToCamelCase();
		}

		public static string GetCamelCasePropertyNameWithPrefix<T>(Expression<Func<T>> expression, string prefix)
		{
			return (String.Format("{0}.{1}", prefix, GetPropertyName(expression).ToCamelCase())).ToCamelCase();
		}

		public static string GetClassName<T>()
		{
			return typeof(T).Name;
		}

		public static string GetControllerName<TControllerType>()
		{
			string name = typeof(TControllerType).Name;
			return GetControllerName(name);
		}

		public static string GetControllerName(string name)
		{
			const string controller = "Controller";
			if (name.EndsWith(controller))
			{
				name = name.Substring(0, name.Length - controller.Length);
			}
			return name;
		}

		private static Dictionary<string, GenericSetter> GetDestinationAccessors(Type destinationType)
		{
			var destinationAccessors = _setters[destinationType];
			if (destinationAccessors == null)
			{
				var destinationFields = destinationType
					.GetFields()
					.Where(x=>!x.IsLiteral)
					.Select(x => new GenericSetter
					{
						Name = x.Name,
						StorageType = x.FieldType,
						SetValue = (Action<object, object>)(x.SetValue),
						AccessorType = AccessorType.Field
					});

				var destinationProperties = destinationType
					.GetProperties()
					.ThatHaveASetter()
					.Select(x => new GenericSetter
					{
						Name = x.Name,
						StorageType = x.PropertyType,
						SetValue = (Action<object, object>)((instance, value) => x.SetValue(instance, value, null)),
						AccessorType = AccessorType.Property
					});

				destinationAccessors = new Dictionary<string, GenericSetter>(100);
				foreach (var accessor in destinationProperties.Concat(destinationFields))
				{
					destinationAccessors.Add(accessor.Name.ToLower(), accessor);
				}
				_setters.Add(destinationType, destinationAccessors);
			}
			return destinationAccessors;
		}

		[DebuggerStepThrough]
		public static string GetFinalPropertyName<T>(Expression<Func<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("expression must be in the form: () => instance.Property");
			}
			var names = GetNames(memberExpression);
			return names.Last();
		}

		[DebuggerStepThrough]
		public static string GetFinalPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("expression must be in the form: () => instance.Property");
			}
			var names = GetNames(memberExpression);
			return names.Last();
		}

		public static IEnumerable<PropertyMappingInfo> GetMatchingFieldsAndProperties(Type sourceType, Type destinationType)
		{
			var sourceAccessors = GetSourceAccessors(sourceType);
			var destinationAccessors = GetDestinationAccessors(destinationType);

			return (from entry in destinationAccessors
			        where sourceAccessors.ContainsKey(entry.Key)
			        let sourceAccessor = sourceAccessors[entry.Key]
			        let destinationAccessor = entry.Value
			        select new PropertyMappingInfo
			        {
			        	Name = destinationAccessor.Name,
			        	SourcePropertyType = sourceAccessor.StorageType,
			        	DestinationPropertyType = destinationAccessor.StorageType,
			        	GetValueFromSource = sourceAccessor.GetValue,
			        	SetValueToDestination = destinationAccessor.SetValue
			        }).ToList();
		}

		public static IEnumerable<PropertyMappingInfo> GetMatchingProperties(Type sourceType, Type destinationType)
		{
			var sourceAccessors = GetSourceAccessors(sourceType);
			var destinationAccessors = GetDestinationAccessors(destinationType);

			return (from entry in destinationAccessors
			        where sourceAccessors.ContainsKey(entry.Key)
			        let sourceAccessor = sourceAccessors[entry.Key]
			        let destinationAccessor = entry.Value
			        where sourceAccessor.AccessorType == AccessorType.Property
			        where destinationAccessor.AccessorType == AccessorType.Property
			        select new PropertyMappingInfo
			        {
			        	Name = destinationAccessor.Name,
			        	SourcePropertyType = sourceAccessor.StorageType,
			        	DestinationPropertyType = destinationAccessor.StorageType,
			        	GetValueFromSource = sourceAccessor.GetValue,
			        	SetValueToDestination = destinationAccessor.SetValue
			        }).ToList();
		}

		public static MethodCallData GetMethodCallData<TClass>(Expression<Func<TClass, object>> methodCall) where TClass : class
		{
			string className = typeof(TClass).Name;
			string methodName = GetMethodName(methodCall);

			var expression = GetMethodCallExpression(methodCall);
			var parameters = expression.Method.GetParameters();
			var parameterDictionary = parameters.Select((x, i) => new
			{
				x.Name,
				Value = GetValueAsString(expression.Arguments[i])
			}
				).ToDictionary(x => x.Name, x => x.Value);

			return new MethodCallData
			{
				MethodName = methodName,
				ClassName = className,
				ParameterValues = parameterDictionary
			};
		}

		public static MethodCallExpression GetMethodCallExpression<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression == null)
			{
				var unaryExpression = expression.Body as UnaryExpression;
				if (unaryExpression == null)
				{
					throw new ArgumentException(
						"expression must be in the form: (Foo instance) => instance.Method()");
				}
				methodCallExpression = unaryExpression.Operand as MethodCallExpression;
				if (methodCallExpression == null)
				{
					throw new ArgumentException(
						"expression must be in the form: (Foo instance) => instance.Method()");
				}
			}
			return methodCallExpression;
		}

		[DebuggerStepThrough]
		public static string GetMethodName<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			var methodCallExpression = GetMethodCallExpression(expression);
			return methodCallExpression.Method.Name;
		}

		public static string GetMultiLevelPropertyName(params string[] propertyNames)
		{
			return propertyNames.Join(".");
		}

		private static List<string> GetNames(MemberExpression memberExpression)
		{
			var names = new List<string>
			{
				memberExpression.Member.Name
			};
			while (memberExpression.Expression as MemberExpression != null)
			{
				memberExpression = (MemberExpression)memberExpression.Expression;
				names.Insert(0, memberExpression.Member.Name);
			}
			return names;
		}

		[DebuggerStepThrough]
		public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
		{
			string names = GetPropertyName(expression.Body as MemberExpression);
			if (names != null)
			{
				return names;
			}
			names = GetPropertyName(expression.Body as UnaryExpression);
			if (names != null)
			{
				return names;
			}
			throw new ArgumentException("expression must be in the form: (Thing instance) => instance.Property[.Optional.Other.Properties.In.Chain]");
		}

		public static string GetPropertyName(MemberExpression memberExpression)
		{
			if (memberExpression == null)
			{
				return null;
			}
			var names = GetNames(memberExpression);
			string name = names.Join(".");
			return name;
		}

		public static string GetPropertyName(UnaryExpression unaryExpression)
		{
			if (unaryExpression == null)
			{
				return null;
			}
			var memberExpression = unaryExpression.Operand as MemberExpression;
			return GetPropertyName(memberExpression);
		}

		[DebuggerStepThrough]
		public static string GetPropertyName<T>(Expression<Func<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("expression must be in the form: () => instance.Property");
			}
			var names = GetNames(memberExpression);
			string name = names.Count > 1 ? names.Skip(1).Join(".") : names.Join(".");
			return name;
		}

		private static Dictionary<string, GenericGetter> GetSourceAccessors(Type sourceType)
		{
			var sourceAccessors = _getters[sourceType];
			if (sourceAccessors == null)
			{
				var sourceFields = sourceType
					.GetFields()
					.Select(x =>
					        new GenericGetter
					        {
					        	Name = x.Name,
					        	StorageType = x.FieldType,
					        	GetValue = x.GetValue,
					        	AccessorType = AccessorType.Field
					        }
					);
				var sourceProperties = sourceType
					.GetProperties()
					.ThatHaveAGetter()
					.Select(x => new GenericGetter
					{
						Name = x.Name,
						StorageType = x.PropertyType,
						GetValue = (Func<object, object>)(instance => x.GetValue(instance, null)),
						AccessorType = AccessorType.Property
					});

				sourceAccessors = new Dictionary<string, GenericGetter>(100);
				foreach (var accessor in sourceProperties.Concat(sourceFields))
				{
					sourceAccessors.Add(accessor.Name.ToLower(), accessor);
				}

				_getters.Add(sourceType, sourceAccessors);
			}
			return sourceAccessors;
		}

		///<summary>
		///    http://stackoverflow.com/questions/340525/accessing-calling-object-from-methodcallexpression
		///</summary>
		private static object GetValue(Expression expression)
		{
			var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)));
			var func = lambda.Compile();
			return func.Invoke();
		}

		public static string GetValueAsString(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Call:
					return GetValueAsString((MethodCallExpression)expression);
				case ExpressionType.Constant:
					return GetValueAsString((ConstantExpression)expression);
				case ExpressionType.MemberAccess:
					return GetValueAsString((MemberExpression)expression);
				default:
					throw new NotImplementedException(expression.GetType() + " with nodeType " + expression.NodeType);
			}
		}

		private static string GetValueAsString(ConstantExpression expression)
		{
			return expression.Value.ToString();
		}

		private static string GetValueAsString(MemberExpression expression)
		{
			var result = GetValue(expression);
			return result.ToString();
		}

		private static string GetValueAsString(MethodCallExpression expression)
		{
			var result = GetValue(expression);
			return result.ToString();
		}

		public static bool IsNullableValueType(Type type)
		{
			if (type.IsValueType)
			{
				if (type.IsGenericType &&
				    type.DeclaringType == null)
				{
					// e.g. decimal?
					return true;
				}
			}
			return false;
		}

		public static bool IsUserType(Type type)
		{
			var assembly = type.Assembly;
			string company = GetAttribute<AssemblyCompanyAttribute>(assembly).Company;
			return company != "Microsoft Corporation";
		}

		private class GenericGetter
		{
			public AccessorType AccessorType { get; set; }
			public Func<object, object> GetValue { get; set; }
			public string Name { get; set; }
			public Type StorageType { get; set; }
		}

		private class GenericSetter
		{
			public AccessorType AccessorType { get; set; }
			public string Name { get; set; }
			public Action<object, object> SetValue { get; set; }
			public Type StorageType { get; set; }
		}
	}
}