using System.Linq;
using System.Reflection;
using DotApiClient;

namespace NetFramework.UnitTests
{
    public partial class WebApiParameterDescripitonBehavior
    {
        private interface IContract
        {
            void Foo([Name("param")] int p);

            void PayloadParam([Payload] int p);
            void GetParam([UrlParam] int p);
            void FormItemParam([FormItem] int p);
        }

        private ParameterInfo GetParameter(string methodName, string paramName)
        {
            return typeof(IContract).GetMethod(methodName).GetParameters().Single(p => p.Name == paramName);
        }
    }
}