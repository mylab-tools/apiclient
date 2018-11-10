using System;
using System.Net.Http;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Testing
{
    class TestConsoleWriterHttpMessageListener : IHttpMessagesListener
    {
        private readonly ITestOutputHelper _output;

        public TestConsoleWriterHttpMessageListener(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Notify(HttpRequestMessage request, HttpResponseMessage response)
        {
            _output.WriteLine("========================================");
            _output.WriteLine("HTTP Request:");
            _output.WriteLine("========================================");
            _output.WriteLine(request.Method + " " + request.RequestUri);
            foreach (var header in request.Headers)
                _output.WriteLine(header.Key + ": " + string.Join(", ", header.Value));
            if (request.Content != null)
                foreach (var header in request.Content.Headers)
                    _output.WriteLine(header.Key + ": " + string.Join(", ", header.Value));

            _output.WriteLine("");

            if (request.Content != null)
            {
                try
                {
                    _output.WriteLine(request.Content.ReadAsStringAsync().Result);
                }
                catch (Exception e)
                {
                    _output.WriteLine("Error: " + e.Message);
                }
            }

            _output.WriteLine("");
            _output.WriteLine("========================================");
            _output.WriteLine("HTTP Response:");
            _output.WriteLine("========================================");
            _output.WriteLine((int)response.StatusCode + " " + response.StatusCode);
            foreach (var header in response.Headers)
                _output.WriteLine(header.Key + ": " + header.Value);
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                    _output.WriteLine(header.Key + ": " + header.Value);
                _output.WriteLine("");
                try
                {
                    _output.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception e)
                {
                    _output.WriteLine("Error: " + e.Message);
                }
            }
        }
    }
}
