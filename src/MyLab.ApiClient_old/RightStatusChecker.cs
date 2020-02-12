using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    static class RightStatusChecker
    {
        public static async Task Check(Func<Task<HttpRequestMessage>> reqProvider, HttpResponseMessage resp)
        {
            if (!resp.IsSuccessStatusCode)
            {
                throw new WrongResponseException(await reqProvider(), resp);
            }
        }
    }
}