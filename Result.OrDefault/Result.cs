﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Result.OrDefault
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
					if (memberAccess != null)
					{
						var propertyInvocation = (PropertyInfo)memberAccess.Member;
						var newInstance = propertyInvocation.GetValue(instance);
						return newInstance;
					}
					else
					{
						var methodInvocation = methodCall.Method;
						var newInstance = methodInvocation.Invoke(instance, new object[0]);
						return newInstance;
					}
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
					if (memberAccess != null)
					{
						var propertyInvocation = (PropertyInfo)memberAccess.Member;
						var newInstance = propertyInvocation.GetValue(instance);
						return newInstance;
					}
					else
					{
						var methodInvocation = methodCall.Method;
						var newInstance = methodInvocation.Invoke(instance, new object[0]);
						return newInstance;
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				if (memberAccess != null)
				{
					var constantExpression = (ConstantExpression)memberAccess.Expression;
					var closureField = memberAccess.Member as FieldInfo;
					if (closureField != null)
					{
						var instance = closureField.GetValue(constantExpression.Value);
						return instance;
					}
					else
					{
						var closureProperty = memberAccess.Member as PropertyInfo;
						var instace = closureProperty.GetValue(constantExpression.Value);
						return instace;
					}
				}
				else
				{
					var constantExpression = (ConstantExpression)methodCall.Object;
					var closureMethod = methodCall.Method;
					var instance = closureMethod.Invoke(constantExpression.Value, new object[0]);
					return instance;
				}
			}
		}
	}
}