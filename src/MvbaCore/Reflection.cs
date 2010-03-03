using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MvbaCore
{
	public class Reflection
	{
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
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				var unaryExpression = expression.Body as UnaryExpression;
				if (unaryExpression == null)
				{
					throw new ArgumentException(
						"expression must be in the form: (Thing instance) => instance.Property[.Optional.Other.Properties.In.Chain]");
				}
				memberExpression = unaryExpression.Operand as MemberExpression;
				if (memberExpression == null)
				{
					throw new ArgumentException(
						"expression must be in the form: (Thing instance) => instance.Property[.Optional.Other.Properties.In.Chain]");
				}
			}
			var names = GetNames(memberExpression);
			string name = names.Join(".");
			return name;
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

		/// <summary>
		///		http://stackoverflow.com/questions/340525/accessing-calling-object-from-methodcallexpression
		/// </summary>
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
	}
}