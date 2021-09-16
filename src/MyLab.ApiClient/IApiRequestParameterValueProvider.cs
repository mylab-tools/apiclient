using System.Linq.Expressions;
using MyLab.ExpressionTools;

namespace MyLab.ApiClient
{
    interface IApiRequestParameterValueProvider
    {
        object GetValue();
    }

    class SimpleApiRequestParameterValueProvider : IApiRequestParameterValueProvider
    {
        private readonly object _value;

        public SimpleApiRequestParameterValueProvider(object value)
        {
            _value = value;
        }
        public object GetValue()
        {
            return _value;
        }
    }

    class ExpressionBasedApiRequestParameterValueProvider : IApiRequestParameterValueProvider
    {
        private readonly Expression _valueProviderExpression;

        /// <summary>
        /// Initializes a new instance of <see cref="ExpressionBasedApiRequestParameterValueProvider"/>
        /// </summary>
        public ExpressionBasedApiRequestParameterValueProvider(Expression valueProviderExpression)
        {
            _valueProviderExpression = valueProviderExpression;
        }

        public object GetValue()
        {
            return _valueProviderExpression.GetValue<object>();
        }
    }
}