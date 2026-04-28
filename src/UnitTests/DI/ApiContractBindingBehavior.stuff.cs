using MyLab.ApiClient.Contracts.Attributes.ForContract;

namespace UnitTests.DI;

public partial class ApiContractBindingBehavior
{
    interface ITestContract
    {
    }

    [ApiContract(Binding = "CustomBinding")]
    interface ITestContractWithCustomBinding
    {
    }

    interface ITestContractWithFullName
    {
    }

    public static object[][] GetBindingKeysCases()
    {
        return new[]
        {
            new[] { "CustomBinding" },
            new[] { nameof(ITestContractWithCustomBinding) },
            new[] { typeof(ITestContractWithCustomBinding).FullName },
        };
    }
}