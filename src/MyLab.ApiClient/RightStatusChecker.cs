using System.Net.Http;

namespace MyLab.ApiClient
{
    static class RightStatusChecker
    {
        public static void Check(HttpRequestMessage req, HttpResponseMessage resp)
        {
            if (!resp.IsSuccessStatusCode)
            {
                throw new WrongResponseException(req, resp);
            }
        }
    }
}