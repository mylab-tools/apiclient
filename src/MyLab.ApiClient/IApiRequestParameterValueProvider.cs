using System;
using System.Linq.Expressions;

namespace MyLab.ApiClient
{
    interface IApiRequestParameterValueProvider
    {
        object GetValue();
    }

    class DefaultApiRequestParameterValueProvider : IApiRequestParameterValueProvider
    {
        private readonly Expression _valueProviderExpression;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultApiRequestParameterValueProvider"/>
        /// </summary>
        public DefaultApiRequestParameterValueProvider(Expression valueProviderExpression)
        {
            _valueProviderExpression = valueProviderExpression;
        }

        public object GetValue()
        {
            throw new NotImplementedException();
        }
    }
}