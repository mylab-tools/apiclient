using System.Reflection;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    public partial class WebApiMethodDescriptionBahavior
    {
        private interface IContract
        {
            void NonEndpointMethod();

            [ResourceAction(HttpMethod.Get)]
            void GetResourceAction(int p);

            [ResourceAction(HttpMethod.Delete)]
            void DeleteResourceAction();

            [ResourceAction(HttpMethod.Patch)]
            void PatchResourceAction();

            [ResourceAction(HttpMethod.Post)]
            void PostResourceAction(int p);

            [ResourceAction(HttpMethod.Put)]
            void PutResourceAction();

            [ServiceEndpoint(HttpMethod.Get, "")]
            void GetServiceEndpoint(int p);

            [ServiceEndpoint(HttpMethod.Delete, "")]
            void DeleteServiceEndpoint();

            [ServiceEndpoint(HttpMethod.Patch, "")]
            void PatchServiceEndpoint();

            [ServiceEndpoint(HttpMethod.Post, "")]
            void PostServiceEndpoint(int p);

            [ServiceEndpoint(HttpMethod.Put, "")]
            void PutServiceEndpoint();

            [ResourceAction(HttpMethod.Get), ContentType(ContentType.Xml)]
            void GetWithContentType();

            [ResourceAction(HttpMethod.Delete), ContentType(ContentType.Xml)]
            void DeleteWithContentType();

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Xml)]
            void PostWithContentType();

            [ResourceAction(HttpMethod.Get), ContentType(ContentType.Xml)]
            void GetWithContentTypeXmlAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Json)]
            void PostWithContentTypeJsonAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.UrlEncodedForm)]
            void PostWithContentTypeFormUrlEncAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.FromData)]
            void PostWithContentTypeFormAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Html)]
            void PostWithContentTypeHtmlAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Xml)]
            void PostWithContentTypeXmlAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Javascript)]
            void PostWithContentTypeJsAndParameter(int p);

            [ResourceAction(HttpMethod.Post), ContentType(ContentType.Text)]
            void PostWithContentTypeTextAndParameter(int p);

            [ResourceAction(HttpMethod.Post)]
            void WithSeveralPayloadParams([Payload] int p1, [Payload] int p2);

            [ResourceAction(HttpMethod.Post)]
            void WithPayloadAndFormParams([Payload] int p1, [FormItem] int p2);

            [ResourceAction(HttpMethod.Post)]
            void WithPayloadAndGetParams([Payload] int p1, [GetParam] int p2, [GetParam] int p3);

            [ResourceAction(HttpMethod.Post)]
            void WithFormAndGetParams([FormItem] int p0, [FormItem] int p1, [GetParam] int p2, [GetParam] int p3);
        }

        private MethodInfo GetMethod(string name)
        {
            return typeof(IContract).GetMethod(name);
        }
    }
}