using System.Net.Http;
using JetBrains.Annotations;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Models;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Models
{
    [TestSubject(typeof(ServiceModel))]
    public class ServiceModelBehavior
    {
        [Fact]
        public void ShouldFailIfContractIsNotInterface()
        {
            //Arrange
            var contract = typeof(ClassContract);

            //Act & Assert
            var e = Assert.Throws<InvalidApiContractException>(() => ServiceModel.FromContract(contract, null));
            Assert.Contains("is not an interface", e.Message);
        }

        [Fact]
        public void ShouldFailIfContractWithoutContractAttribute()
        {
            //Arrange
            var contract = typeof(IApiContractWithoutContractAttribute);

            //Act & Assert
            var e = Assert.Throws<InvalidApiContractException>(() => ServiceModel.FromContract(contract));
            Assert.Contains("ApiContract", e.Message);
        }

        [Fact]
        public void ShouldProvideServiceDescription()
        {
            //Arrange
            var contract = typeof(IApiContract);
            var methodId = contract.GetMethod(nameof(IApiContract.Method))!.MetadataToken;

            //Act
            var desc = ServiceModel.FromContract(contract);

            //Assert
            Assert.NotNull(desc);
            Assert.Equal("/path", desc.Url);
            Assert.NotNull(desc.Endpoints);
            Assert.Single(desc.Endpoints);
            Assert.Contains(desc.Endpoints, e => e.Key == methodId && e.Value.HttpMethod == HttpMethod.Post);
            Assert.NotNull(desc.Bindings);
            Assert.Equal(3, desc.Bindings.Count);
            Assert.Contains("test", desc.Bindings);
            Assert.Contains(contract.Name, desc.Bindings);
            Assert.Contains(contract.FullName, desc.Bindings);
        }

        [ApiContract(Url = "/path", Binding = "test")]
        interface IApiContract
        {
            [Post]
            void Method();
        }

        interface IApiContractWithoutContractAttribute
        {

        }

        class ClassContract
        {
            
        }
    }
}
