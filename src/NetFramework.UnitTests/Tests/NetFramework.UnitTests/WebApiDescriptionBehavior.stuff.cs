using DotAspectClient;

namespace NetFramework.UnitTests
{
    public partial class WebApiDescriptionBehavior
    {
        [RestApi]
        private interface IContractWithForgottenMethod
        {
            void Foo(int i);
        }

        [WebApi]
        private interface IServiceContractWithWronMarkedMethod
        {
            [RestAction(HttpMethod.Get)]
            void WithWrongAttribute();
        }

        [RestApi]
        private interface IResourceContractWithWrongMarkedMethod
        {
            [ServiceEndpoint(HttpMethod.Get)]
            void WithWrongAttribute();
        }
    }
}