using System.Linq.Expressions;

namespace MyLab.ApiClient
{
    class ApiRequestParameter<TDEsc>
        where TDEsc : IRequestParameterDescription
    {
        public TDEsc Description { get; }
        public IApiRequestParameterValueProvider ValueProvider { get; }

        public ApiRequestParameter(
            TDEsc description, 
            IApiRequestParameterValueProvider valueProvider)
        {
            Description = description;
            ValueProvider = valueProvider;
        }
    }
}