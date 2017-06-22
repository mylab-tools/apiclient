using System;
using System.Dynamic;
using System.Reflection;

namespace RedCucumber.Wac
{
    class ServiceApiClientFactory<T>
        where T : class
    {
        private readonly WebApiServiceAttribute _serviceAttribute;

        public ServiceApiClientFactory(WebApiServiceAttribute serviceAttribute)
        {
            _serviceAttribute = serviceAttribute;
        }

        public T Create()
        {
            
            return null;
        }
    }
    
    class WebApiProxy : DynamicObject
    {
        private readonly WebApiDescription _webApiDescription;

        public WebApiProxy(WebApiDescription webApiDescription)
        {
            _webApiDescription = webApiDescription;
        }

        ///// <inheritdoc />
        //public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        //{
            
        //}
    }
}