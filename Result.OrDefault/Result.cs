using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ResultOrDefault
{
	public class Result
	{
		public static T OrDefault<T>(Expression<Func<T>> selector, T defaultValue = default(T))
		{
			try
			{
				var result = GetRecursive(
					memberAccess: selector.Body as MemberExpression,
					methodCall: selector.Body as MethodCallExpression);
				return (T)(result ?? defaultValue);
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			};
		}

		private static object GetRecursive(MemberExpression memberAccess, MethodCallExpression methodCall)
		{
			var nextExpression = memberAccess != null ? memberAccess.Expression : methodCall.Object;
			var nextMemberAccess = nextExpression as MemberExpression;
			var nextMethodCall = nextExpression as MethodCallExpression;

			if (nextMemberAccess != null)
			{
				var instance = GetRecursive(nextMemberAccess, null);
				if (instance != null)
				{
					return GetOrInvoke(instance, memberAccess, methodCall);
				}
				else
				{
					return null;
				}
			}
			if (nextMethodCall != null)
			{
				var instance = GetRecursive(null, nextMethodCall);
				if (instance != null)
				{
					return GetOrInvoke(instance, memberAccess, methodCall);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return GetRoot(memberAccess, methodCall);
			}
		}

		private static object GetOrInvoke(object instance, MemberExpression memberAccess, MethodCallExpression methodCall)
		{
			if (memberAccess != null)
			{
				if (memberAccess.Member is PropertyInfo)
				{
					var property = (PropertyInfo)memberAccess.Member;
					return property.GetValue(instance, new object[0]);
				}
				else if (memberAccess.Member is FieldInfo)
				{
					var field = (FieldInfo)memberAccess.Member;
					return field.GetValue(instance);
				}
				else
				{
					throw new NotSupportedException();
				}
			}
			else
			{
				var methodInvocation = methodCall.Method;
				var newInstance = methodInvocation.Invoke(instance, GetArguments(methodCall.Arguments));
				return newInstance;
			}
		}

		private static object GetRoot(MemberExpression memberAccess, MethodCallExpression methodCall)
		{
			if (memberAccess != null)
			{
				if (memberAccess.Expression == null)
				{
					return GetOrInvoke(null, memberAccess, null);
				}
				else
				{
					if (memberAccess.Expression is ConstantExpression)
					{
						var constantExpression = (ConstantExpression)memberAccess.Expression;
						return constantExpression.Value != null ? GetOrInvoke(constantExpression.Value, memberAccess, null) : null;
					}
					else
					{
						var instance = Expression.Lambda<Func<object>>(memberAccess.Expression).Compile()();
						var property = (PropertyInfo)memberAccess.Member;
						return instance != null ? property.GetValue(instance, new object[0]) : null;
					}
				}
			}
			else
			{
				var method = methodCall.Method;
				var arguments = GetArguments(methodCall.Arguments);

				if (methodCall.Object == null)
				{
					return method.Invoke(null, arguments);
				}
				else
				{
					if (methodCall.Object is ConstantExpression)
					{
						var constantExpression = (ConstantExpression)methodCall.Object;
						return constantExpression.Value != null ? method.Invoke(constantExpression.Value, arguments) : null;
					}
					else
					{
						var instance = Expression.Lambda<Func<object>>(methodCall.Object).Compile()();
						return instance != null ? method.Invoke(instance, arguments) : null;
					}
				}
			}
		}

		private static object[] GetArguments(IEnumerable<Expression> arguments)
		{
			return arguments
				.Select(a => Expression.Lambda<Func<object>>(a).Compile()())
				.ToArray<object>();
		}
	}
}
