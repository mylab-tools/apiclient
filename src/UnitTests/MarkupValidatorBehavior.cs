using System;
using System.Linq;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class MarkupValidatorBehavior
    {
        [Fact]
        public void ShouldPassValidContract()
        {
            //Arrange
            

            //Act
            var valRes = new ApiContractValidator().Validate(typeof(IRightContract));
            
            //Assert
            Assert.Empty(valRes);
        }

        [Theory]
        [InlineData(typeof(IContractWithMethodWithWrongUrl))]
        [InlineData(typeof(IContractWithMethodWithoutAttr))]
        [InlineData(typeof(IContractWithParameterWithoutAttr))]
        [InlineData(typeof(IContractWithSeveralContentParams))]
        [InlineData(typeof(IContractWithWrongUrl))]
        [InlineData(typeof(IContractWithoutApiAttr))]
        [InlineData(typeof(IContractWithWrongBinParam))]
        public void ShouldDetectCriticalWrongContract(Type contractType)
        {
            //Arrange


            //Act
            var valRes = new ApiContractValidator().Validate(contractType);

            _output.WriteLine(valRes.ToString());

            //Assert
            Assert.Contains(valRes, iss => iss.Critical);
        }

        [Theory]
        [InlineData(typeof(IContractWithEmptyUrl))]
        [InlineData(typeof(IContractWithMethodWithEmptyUrl))]
        public void ShouldDetectWarningWrongContract(Type contractType)
        {
            //Arrange


            //Act
            var valRes = new ApiContractValidator().Validate(contractType);

            _output.WriteLine(valRes.ToString());

            //Assert
            Assert.True(valRes.All(iss => !iss.Critical));
        }
    }
}
