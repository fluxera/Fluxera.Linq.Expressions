namespace Fluxera.Linq.Expressions.UnitTests
{
	using FluentAssertions;
	using NUnit.Framework;
	using System;
	using System.Linq.Expressions;
	using Fluxera.Linq.Expressions;

	[TestFixture]
	public class ExpressionExtensionsTests
	{
		[Test]
		public void ShouldCheckForEmpty()
		{
			// Arrange
			Expression<Func<Person, string>> expression = x => x.Name;

			// Act
			string? str = expression.ToExpressionString();

			// Assert
			str.Should().NotBeNullOrWhiteSpace();
		}
	}
}
