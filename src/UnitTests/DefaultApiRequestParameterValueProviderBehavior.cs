using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class DefaultApiRequestParameterValueProviderBehavior
    {
        [Theory]
        [MemberData(nameof(GetExpressions))]
        public void ShouldProvideParameterValue(LambdaExpression expr)
        {
            //Arrange
            var provider = new ExpressionBasedApiRequestParameterValueProvider(expr.Body);

            //Act
            var value = provider.GetValue();

            //Assert
            Assert.IsType<string>(value);
            Assert.Equal("foo", value);
        }

        public static IEnumerable<object[]> GetExpressions()
        {
            Expression<Func<string>> getConst = () => "foo";

            var value = "f" + "oo";
            Expression<Func<string>> getVar = () => value;

            Expression<Func<string>> getFunc = () => GetFoo();


            return new List<object[]>
            {
                new object[]{ getConst },
                new object[]{ getVar },
                new object[]{ getFunc },
                new object[]{ getFunc },
            };
        }

        static string GetFoo() => "foo";
    }
}
