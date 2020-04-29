using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ExpressionBasedApiRequestParameterValueProviderBehavior
    {

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void ShouldProvideValue(Expression expression, object expected)
        {
            //Arrange
            var provider = new ExpressionBasedApiRequestParameterValueProvider(expression);

            //Act
            var res = provider.GetValue();

            //Assert
            Assert.Equal(expected, res, new TestEqualityComparer());
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            var m = new TestModel("foo");

            yield return GetCase(() => 10);
            yield return GetCase(() => m.Value);
            yield return GetCase(() => m.GetValue());
            yield return GetCase(() => new TestModel("bar"));
            yield return GetCase(() => new TestModel("foo"){Value = "bar"});
            yield return GetCase(() => $"{10}-{m.Value}");
        }

        static object[] GetCase<T>(Expression<Func<T>> expr)
        {
            return new object[]
            {
                expr.Body,
                expr.Compile()()
            };
        }

        class TestModel
        {
            public string Value { get; set; }
            
            public TestModel(string value)
            {
                Value = value;
            }

            public string GetValue()
            {
                return Value;
            }
        }

        public class TestEqualityComparer : IEqualityComparer<object>
        {
            public bool Equals(object x, object y)
            {
                if(x is TestModel xM && y is TestModel yM)
                    return xM.Value.Equals(yM.Value);

                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                return (obj is TestModel objM ? objM.Value : obj).GetHashCode();
            }
        }
    }
}
