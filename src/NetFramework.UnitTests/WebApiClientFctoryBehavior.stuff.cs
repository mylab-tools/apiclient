using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    public partial class WebApiClientFctoryBehavior
    {
        [WebApiResource]
        private interface IResource
        {
        }

        [WebApiService]
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