using System;
using System.Threading.Tasks;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Options;

namespace UnitTests.Usage;

public partial class ApiClientProxyBehavior
{
    readonly ApiClientOptions _defaultOptions = new();

    interface IContract
    {
        [Post("void")]
        public Task PerformVoidAsync([JsonContent] int parameter);

        [Post("get")]
        public Task<string> GetAsync();

        [Get("newtonjson")]
        public Task<NewtonJsonModel> GetNewtonJsonModel();

        [Get("microsoft")]
        public Task<MicrosoftModel> GetMicrosoftModel();

        [Post("send/{value}")]
        public Task SendInt([Path] int value);

        [Post("send/{value}")]
        public Task SendGuid([Path("value")] Guid id);

        [Post("send-obj")]
        public Task SendObj([JsonContent] NewtonJsonModel obj);
    }

    class NewtonJsonModel
    {
        public const string ValuePropertyName = "nj_val";

        [Newtonsoft.Json.JsonProperty(ValuePropertyName)]
        public string? Value { get; set; }
    }

    class MicrosoftModel
    {
        public const string ValuePropertyName = "ms_val";

        [System.Text.Json.Serialization.JsonPropertyName(ValuePropertyName)]
        public string? Value { get; set; }
    }
}