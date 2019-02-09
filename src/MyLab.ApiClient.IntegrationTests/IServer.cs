using System.Threading.Tasks;
using TestServer.Controllers;

namespace MyLab.ApiClient.IntegrationTests
{
    [Api("api/test")]
    public interface IServer
    {
        [ApiGet]
        Task<string> RootGet();
        
        [ApiGet(RelPath = "foo/bar")]
        Task<string> GetWithPath();

        [ApiGet(RelPath = "foo/bar/{id}")]
        Task<string> GetWithParametrizedPath([PathParam] string id);

        [ApiGet(RelPath = "foo/bar/q")]
        Task<string> GetWithQuery([QueryParam] string id);

        [ApiPost(RelPath = "post/bin")]
        Task<byte[]> PostBinary([BinaryBody]byte[] bin);

        [ApiPost(RelPath = "post/xml-object")]
        Task<TestObject> PostXmlObj([XmlBody]TestObject testObject);
        
        [ApiPost(RelPath = "post/bin-xml")]
        Task<byte[]> PostBinaryXml([XmlBody]byte[] binXml);

        [ApiPost(RelPath = "post/str-xml")]
        Task<string> PostStringXml([XmlBody]string strXml);


        [ApiPost(RelPath = "post/json-object")]
        Task<TestObject> PostJsonObj([JsonBody]TestObject testObject);

        [ApiPost(RelPath = "post/form-object")]
        Task<TestObject> PostFormObj([FormBody]TestObject testObject);

        [ApiPost(RelPath = "post/json-object")]
        WebApiCall<TestObject> GetObjUsingCall([JsonBody]TestObject testObject);

        [ApiPost(RelPath = "post/header")]
        Task<string> PostHeaderDirect([Header]string superheader);

        [ApiPost(RelPath = "post/header")]
        Task<string> PostHeaderDirectWithParameterRenaming([Header(Name = "superheader")]string header);

        [ApiPost(RelPath = "post/header")]
        Task<string> PostHeaderWithFactory();

        [ApiGet(RelPath = "get/code")]
        Task<CodeResult> GetCode([QueryParam]int code, [QueryParam]string msg);
    }
}