using DotApiClient;

namespace NetFramework.UnitTests
{
    public partial class WebApiClientFctoryBehavior
    {
        [RestApi]
        private interface IResource
        {
        }

        [WebApi]
        private interface IService
        {
            [ServiceEndpoint(HttpMethod.Get)]
            void Foo();
        }

        private interface IContractWithoutSpecialAttributes
        {
        }
    }
}