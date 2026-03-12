using System.Linq;
using System.Reflection;
using System.Threading;

namespace MyLab.ApiClient.Tools
{
    static class CancellationTokenExtractor
    {
        public static CancellationToken FromMethodInvocation(MethodInfo? targetMethod, object?[]? args)
        {
            if(targetMethod == null || args == null)
            {
                return CancellationToken.None;
            }

            return targetMethod.GetParameters()
                .Where(p => p.ParameterType == typeof(CancellationToken))
                .Select((p, i) => args[i])
                .Cast<CancellationToken>()
                .FirstOrDefault(CancellationToken.None);
        }
    }
}
