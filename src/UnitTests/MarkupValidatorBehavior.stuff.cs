using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class MarkupValidatorBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="MarkupValidatorBehavior"/>
        /// </summary>
        public MarkupValidatorBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private void WriteLog(MarkupValidationIssuer[] valRes)
        {
            _output.WriteLine("===== Validation Result =====");

            if (valRes.Length == 0)
            {
                _output.WriteLine("<empty>");
            }
            else
            {
                foreach (var i in valRes)
                {
                    _output.WriteLine("");
                    _output.WriteLine(i.Reason + (i.Critical ? " (critical)" : " (warning)"));
                    _output.WriteLine("\t" + i.ServiceContract.FullName);
                    if (i.Method != null)
                        _output.WriteLine("\t\t" + i.Method.Name);
                    if (i.Parameter != null)
                        _output.WriteLine("\t\t" + i.Parameter.Name);
                }
            }
        }

        [Api("api")]
        private interface IRightContract
        {
            [Post("test")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        private interface IContractWithoutApiAttr
        {
            [Post("test")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("")]
        private interface IContractWithEmptyUrl
        {
            [Post("test")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("http://http://")]
        private interface IContractWithWrongUrl
        {
            [Post("test")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithMethodWithoutAttr
        {
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithMethodWithEmptyUrl
        {
            [Post("")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithMethodWithWrongUrl
        {
            [Post("http://http://")]
            Task Post([Query] int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithParameterWithoutAttr
        {
            [Post("test")]
            Task Post(int a1, [Path] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithSeveralContentParams
        {
            [Post("test")]
            Task Post([Query] int a1, [JsonContent] int a2, [Header("a3")] int a3, [StringContent] int a4);
        }

        [Api("api")]
        private interface IContractWithWrongBinParam
        {
            [Post("test")]
            Task Post([BinContent] int a1);
        }
    }
}