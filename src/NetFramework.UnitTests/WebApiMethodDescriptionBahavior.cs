using System.Reflection;
using NUnit.Framework;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiMethodDescriptionBahavior
    {
        [Test]
        public void ShouldThrowIfMethodIsNotEndpoint()
        {
            //Arrange
            var m = GetMethod(nameof(IContract.NonEndpointMethod));

            //Act
            //Assert
            Assert.Throws<WebApiContractException>(() => WebApiMethodDescription.Create(m));
        }

        [Test]
        [TestCase(nameof(IContract.GetResourceAction),    ContentType.Undefined,      TestName = "GET")]
        [TestCase(nameof(IContract.PostResourceAction),   ContentType.Json, TestName = "POST")]
        [TestCase(nameof(IContract.PutResourceAction),    ContentType.Json, TestName = "PUT")]
        [TestCase(nameof(IContract.PatchResourceAction),  ContentType.Json, TestName = "PATCH")]
        [TestCase(nameof(IContract.DeleteResourceAction), ContentType.Undefined, TestName = "DELETE")]
        public void ShouldDetermineDefaultContentTypeDependsOfHttpMethodForResourceMethods(string methodName, ContentType expectedContentType)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act
            var description = WebApiMethodDescription.Create(m);

            //Assert
            Assert.That(description.ContentType, Is.EqualTo(expectedContentType));
        }

        [Test]
        [TestCase(nameof(IContract.GetServiceEndpoint), ContentType.Undefined, TestName = "GET")]
        [TestCase(nameof(IContract.PostServiceEndpoint), ContentType.UrlEncodedForm, TestName = "POST")]
        [TestCase(nameof(IContract.PutServiceEndpoint), ContentType.UrlEncodedForm, TestName = "PUT")]
        [TestCase(nameof(IContract.PatchServiceEndpoint), ContentType.UrlEncodedForm, TestName = "PATCH")]
        [TestCase(nameof(IContract.DeleteServiceEndpoint), ContentType.Undefined, TestName = "DELETE")]
        public void ShouldDetermineDefaultContentTypeDependsOfHttpMethodForServiceMethods(string methodName, ContentType expectedContentType)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act
            var description = WebApiMethodDescription.Create(m);

            //Assert
            Assert.That(description.ContentType, Is.EqualTo(expectedContentType));
        }

        [Test]
        public void ShouldApplyContentType()
        {
            //Arrange
            var m = GetMethod(nameof(IContract.PostWithContentType));

            //Act
            var description = WebApiMethodDescription.Create(m);

            //Assert
            Assert.That(description.ContentType, Is.EqualTo(ContentType.Xml));
        }

        [Test]
        [TestCase(nameof(IContract.GetWithContentType), ContentType.Undefined, TestName = "for GET")]
        [TestCase(nameof(IContract.DeleteWithContentType), ContentType.Undefined, TestName = "for DELETE")]
        public void ShouldIgnoreContentType(string methodName, ContentType expextedContentType)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act
            var description = WebApiMethodDescription.Create(m);

            //Assert
            Assert.That(description.ContentType, Is.EqualTo(expextedContentType));
        }


        [Test]
        [TestCase(nameof(IContract.GetResourceAction), WebApiParameterType.Get, TestName = "'Get' for resource GET")]
        [TestCase(nameof(IContract.PostResourceAction), WebApiParameterType.Payload, TestName = "'Payload' for resource POST")]
        [TestCase(nameof(IContract.GetServiceEndpoint), WebApiParameterType.Get, TestName = "'Get' for service GET")]
        [TestCase(nameof(IContract.PostServiceEndpoint), WebApiParameterType.FormItem, TestName = "'Payload' for service POST")]
        public void ShouldDetermineTypeForUntypedParamtersWithoutContentSpecification(string methodName, WebApiParameterType expectedType)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act
            var description = WebApiMethodDescription.Create(m);
            var p = description.Parameters["p"];

            //Assert
            Assert.That(p.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void ShouldIgnoreContentSpecificationWhenHttpGetMethodAnfUntypedParameter()
        {
            //Arrange
            var m = GetMethod("GetWithContentTypeXmlAndParameter");

            //Act
            var description = WebApiMethodDescription.Create(m);
            var p = description.Parameters["p"];

            //Assert
            Assert.That(p.Type, Is.EqualTo(WebApiParameterType.Get));
        }

        [Test]
        [TestCase(nameof(IContract.PostWithContentTypeFormAndParameter), WebApiParameterType.FormItem, TestName = "'FormItem' for 'Form'")]
        [TestCase(nameof(IContract.PostWithContentTypeFormUrlEncAndParameter), WebApiParameterType.FormItem, TestName = "'FormItem' for 'Url encoded form'")]
        [TestCase(nameof(IContract.PostWithContentTypeHtmlAndParameter), WebApiParameterType.Payload, TestName = "'Payload' for 'Html'")]
        [TestCase(nameof(IContract.PostWithContentTypeJsAndParameter), WebApiParameterType.Payload, TestName = "'Payload' for 'JavaScript'")]
        [TestCase(nameof(IContract.PostWithContentTypeJsonAndParameter), WebApiParameterType.Payload, TestName = "'Payload' for 'JSON'")]
        [TestCase(nameof(IContract.PostWithContentTypeTextAndParameter), WebApiParameterType.Payload, TestName = "'Payload' for 'text'")]
        [TestCase(nameof(IContract.PostWithContentTypeXmlAndParameter), WebApiParameterType.Payload, TestName = "'Payload' for 'XML'")]
        public void ShouldDetermineUntypedParameterDependsOfCntentTypeInPostMethods(string methodName, WebApiParameterType expectedParameterType)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act
            var description = WebApiMethodDescription.Create(m);
            var p = description.Parameters["p"];

            //Assert
            Assert.That(p.Type, Is.EqualTo(expectedParameterType));
        }

        [Test]
        [TestCase(nameof(IContract.WithSeveralPayloadParams), TestName = "Several payload parameters")]
        [TestCase(nameof(IContract.WithPayloadAndFormParams), TestName = "Payload and form parameters")]
        public void ShouldThrowErrorWhenConflictParameters(string methodName)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act, Assert
            Assert.Throws<WebApiContractException>(() => WebApiMethodDescription.Create(m));
        }

        [Test]
        [TestCase(nameof(IContract.WithFormAndGetParams), TestName = "For payload")]
        [TestCase(nameof(IContract.WithPayloadAndGetParams), TestName = "For form")]
        public void ShouldNotThrowErrorWhenHasAdditionalGetParameters(string methodName)
        {
            //Arrange
            var m = GetMethod(methodName);

            //Act, Assert
            WebApiMethodDescription.Create(m);
        }
    }
}
