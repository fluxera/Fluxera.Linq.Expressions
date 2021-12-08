﻿namespace Fluxera.Linq.Expressions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using JetBrains.Annotations;

	[PublicAPI]
	public static class ExpressionExtensions
	{
		/// <summary>
		///		Converts the given <see cref="Expression"/> to a string representation.
		/// </summary>
		/// <remarks>
		///		See Pete Montgomery's post here: http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string? ToExpressionString<T, TResult>(this Expression<Func<T, TResult>>? expression)
		{
			if(expression is null)
			{
				return null;
			}

			Expression expr = expression;

			// Locally evaluate as much of the query as possible.
			expr = Evaluator.PartialEval(expr);

			// Support local collections.
			expr = LocalCollectionExpander.Rewrite(expr);

			// Use the string representation of the expression for the cache key.
			return expr.ToString();
		}

		/// <summary>
		///		Creates a <see cref="Expression"/> that represents an bitwise AND operation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.And);
		}

		/// <summary>
		///		Creates a <see cref="Expression"/> that represents a conditional AND operation that evaluates the second operand only if it has to.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.AndAlso);
		}

		/// <summary>
		///		Creates a <see cref="Expression"/> that represents an bitwise OR operation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.Or);
		}

		/// <summary>
		///		Creates a <see cref="Expression"/> that represents a conditional OR operation that evaluates the second operand only if it has to.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.OrElse);
		}

		private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
		{
			// Build parameter map (from parameters of second to parameters of first).
			Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

			// Replace parameters in the second lambda expression with parameters from the first.
			Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

			// Apply composition of lambda expression bodies to parameters from the first expression .
			return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
		}
	}
}