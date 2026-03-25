using System.Net;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using TestServer;

namespace E2eTests
{
    [ApiContract(Url="test")]
    public interface ITestServerContract
    {
        [Get("status")]
        Task GetStatusAsync([StringContent] HttpStatusCode codeForReceive);

        [Post("dto/xml")]
        Task<TestDto> GetXmlDto([JsonContent]TestDto dto);
        
        [Post("dto/json")]
        Task<TestDto> GetJsonDto([JsonContent] TestDto dto);
    }

    
}
