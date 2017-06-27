using NUnit.Framework;
using DotAspectClient;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiParameterDescripitonBehavior
    {
        [Test]
        public void ShouldOverrideParamName()
        {
            //Arrange
            var p = GetParameter(nameof(IContract.Foo), "p");

            //Act
            var d = WebApiParameterDescription.Create(p);

            //Assert
            Assert.That(d.Name, Is.EqualTo("param"));
        }

        [Test]
        [TestCase(nameof(IContract.PayloadParam), WebApiParameterType.Payload, TestName = "Payload")]
        [TestCase(nameof(IContract.GetParam), WebApiParameterType.Get, TestName = "Get")]
        [TestCase(nameof(IContract.FormItemParam), WebApiParameterType.FormItem, TestName = "FormItem")]
        public void ShouldDetermineParamterType(string methodName, WebApiParameterType expectedType)
        {
            //Arrange
            var p = GetParameter(methodName, "p");

            //Act
            var d = WebApiParameterDescription.Create(p);

            //Assert
            Assert.That(d.Type, Is.EqualTo(expectedType));
        }
    }
}
