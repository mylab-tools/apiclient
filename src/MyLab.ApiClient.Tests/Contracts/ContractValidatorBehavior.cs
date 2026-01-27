using JetBrains.Annotations;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using System;
using System.Net.Http;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts;

[TestSubject(typeof(ContractValidator))]
public class ContractValidatorBehavior
{
    [Fact]
    public void ShouldThrowWhenContractIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ContractValidator.Validate(null));
    }

    [Fact]
    public void ShouldThrowWhenContractIsNotInterface()
    {
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(typeof(string)));
    }

    [Fact]
    public void ShouldThrowWhenContractContainsNonMethodMembers()
    {
        var contractType = typeof(IContractWithNonMethodMembers);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractContainsGenericMethods()
    {
        var contractType = typeof(IContractWithGenericMethods);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractMethodsAreNotDecoratedWithApiMethodAttribute()
    {
        var contractType = typeof(IContractWithoutApiMethodAttributes);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractMethodsHaveRefParameters()
    {
        var contractType = typeof(IContractWithRefParameters);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractMethodsHaveOutParameters()
    {
        var contractType = typeof(IContractWithOutParameters);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractMethodParametersAreNotDecoratedWithApiParameterAttribute()
    {
        var contractType = typeof(IContractWithParametersWithoutApiParameterAttributes);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldThrowWhenContractIsNotDecoratedWithApiContractAttribute()
    {
        var contractType = typeof(ICorrectWithoutContractAttribute);
        Assert.Throws<InvalidApiContractException>(() => ContractValidator.Validate(contractType));
    }

    [Fact]
    public void ShouldValidateCorrectContract()
    {
        var contractType = typeof(ICorrectContract);
        ContractValidator.Validate(contractType);
    }

    [ApiContract]
    interface IContractWithNonMethodMembers
    {
        int Property { get; }
    }

    [ApiContract]
    interface IContractWithGenericMethods
    {
        [Get]
        void GenericMethod<T>();
    }

    [ApiContract]
    interface IContractWithoutApiMethodAttributes
    {
        void MethodWithoutAttribute();
    }

    [ApiContract]
    interface IContractWithOutParameters
    {
        [Get]
        void MethodWithOutParameter(out int value);
    }

    [ApiContract]
    interface IContractWithRefParameters
    {
        [Get]
        void MethodWithRefParameter(ref int value);
    }

    [ApiContract]
    interface IContractWithParametersWithoutApiParameterAttributes
    {
        [Get]
        void MethodWithoutParameterAttributes(int param);
    }

    interface ICorrectWithoutContractAttribute
    {
        [Get]
        void ValidMethod([Query] int param);
    }

    [ApiContract]
    interface ICorrectContract
    {
        [Get]
        void ValidMethod([Query] int param);
    }
}