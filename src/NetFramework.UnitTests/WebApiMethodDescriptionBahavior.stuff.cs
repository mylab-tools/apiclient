using System;
using System.Reflection;
using DotApiClient;

namespace NetFramework.UnitTests
{
    public partial class WebApiMethodDescriptionBahavior
    {
        private interface IContract
        {
            void NonEndpointMethod();

            [RestAction(HttpMethod.Get)]
            void GetResourceAction(int p);

            [RestAction(HttpMethod.Delete)]
            void DeleteResourceAction();

            [RestAction(HttpMethod.Patch)]
            void PatchResourceAction();

            [RestAction(HttpMethod.Post)]
            void PostResourceAction(int p);

            [RestAction(HttpMethod.Put)]
            void PutResourceAction();

            [ServiceEndpoint(HttpMethod.Get)]
            void GetServiceEndpoint(int p);

            [ServiceEndpoint(HttpMethod.Delete)]
            void DeleteServiceEndpoint();

            [ServiceEndpoint(HttpMethod.Patch)]
            void PatchServiceEndpoint();

            [ServiceEndpoint(HttpMethod.Post)]
            void PostServiceEndpoint(int p);

            [ServiceEndpoint(HttpMethod.Put)]
            void PutServiceEndpoint();

            [RestAction(HttpMethod.Get), ContentType(ContentType.Xml)]
            void GetWithContentType();

            [RestAction(HttpMethod.Delete), ContentType(ContentType.Xml)]
            void DeleteWithContentType();

            [RestAction(HttpMethod.Post), ContentType(ContentType.Xml)]
            void PostWithContentType();

            [RestAction(HttpMethod.Get), ContentType(ContentType.Xml)]
            void GetWithContentTypeXmlAndParameter(int p);

            [RestAction(HttpMethod.Post), ContentType(ContentType.Json)]
            void PostWithContentTypeJsonAndParameter(int p);

            [RestAction(HttpMethod.Post), ContentType(ContentType.UrlEncodedForm)]
            void PostWithContentTypeFormUrlEncAndParameter(int p);
            
            [RestAction(HttpMethod.Post), ContentType(ContentType.Html)]
            void PostWithContentTypeHtmlAndParameter(int p);

            [RestAction(HttpMethod.Post), ContentType(ContentType.Xml)]
            void PostWithContentTypeXmlAndParameter(int p);

            [RestAction(HttpMethod.Post), ContentType(ContentType.Javascript)]
            void PostWithContentTypeJsAndParameter(int p);

            [RestAction(HttpMethod.Post), ContentType(ContentType.Text)]
            void PostWithContentTypeTextAndParameter(int p);

            [RestAction(HttpMethod.Post)]
            void WithSeveralPayloadParams([Payload] int p1, [Payload] int p2);

            [RestAction(HttpMethod.Post)]
            void WithPayloadAndFormParams([Payload] int p1, [FormItem] int p2);

            [RestAction(HttpMethod.Post)]
            void WithPayloadAndGetParams([Payload] int p1, [UrlParam] int p2, [UrlParam] int p3);

            [RestAction(HttpMethod.Post)]
            void WithFormAndGetParams([FormItem] int p0, [FormItem] int p1, [UrlParam] int p2, [UrlParam] int p3);

            [RestAction(HttpMethod.Post)]
            void PostFile(WebApiFile file);

            [RestAction(HttpMethod.Post)]
            void MethodWithHeader([Header("Content-Type")]string header);

            [RestAction(HttpMethod.Post)]
            void MethodWithRestId([RestId]string id);
        }

        private MethodInfo GetMethod(string name)
        {
            return typeof(IContract).GetMethod(name);
        }
    }
}