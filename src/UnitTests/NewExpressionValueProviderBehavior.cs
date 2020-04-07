using System;
using System.Linq.Expressions;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class NewExpressionValueProviderBehavior
    {
        readonly NewExpression _expr;

        /// <summary>
        /// Initializes a new instance of <see cref="NewExpressionValueProviderBehavior"/>
        /// </summary>
        public NewExpressionValueProviderBehavior()
        {
            Expression<Func<A>> func = () => new A(10);
            _expr = (NewExpression) func.Body;
        }

        [Fact]
        public void ShouldProvideNewObject()
        {
            //Arrange
            var provider = new NewExpressionValueProvider();

            //Act
            var val = (A) provider.GetValue(_expr);

            //Assert
            Assert.NotNull(val);
        }

        [Fact]
        public void ShouldUseCtorArgs()
        {
            //Arrange
            var provider = new NewExpressionValueProvider();

            //Act
            var val = (A)provider.GetValue(_expr);

            //Assert
            Assert.Equal(10, val.B);
        }
        
        class A
        {
            public A(int i)
            {
                B = i;
            }
            public int B { get; }
        }
    }
}