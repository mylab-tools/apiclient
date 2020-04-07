using System;
using System.Linq.Expressions;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class MemberInitExpressionValueProviderBehavior
    {
        readonly MemberInitExpression _expr;

        /// <summary>
        /// Initializes a new instance of <see cref="MemberInitExpressionValueProviderBehavior"/>
        /// </summary>
        public MemberInitExpressionValueProviderBehavior()
        {
            Expression<Func<A>> func = () => new A { B = "foo" };
            _expr = (MemberInitExpression)func.Body;
        }

        [Fact]
        public void ShouldApplyBindings()
        {
            //Arrange
            var provider = new MemberInitExpressionValueProvider();

            //Act
            var val = (A)provider.GetValue(_expr);

            //Assert
            Assert.Equal("foo", val.B);
        }

        class A
        {
            public string B { get; set; }
        }
    }
}