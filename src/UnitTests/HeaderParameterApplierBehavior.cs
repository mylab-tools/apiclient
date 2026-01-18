using System;
using System.Net.Http;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class HeaderParameterApplierBehavior
    {
        private readonly ITestOutputHelper _output;

        public HeaderParameterApplierBehavior(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void ShouldApplyDateTimeForIfModifiedSince()
        {
            //Arrange
            var dt =DateTime.Now;
            var desc = new HeaderRequestParameterDescription(0, "If-Modified-Since");
            var valueProvider = new SimpleApiRequestParameterValueProvider(dt);
            var applier = new HeaderParameterApplier(desc, valueProvider);

            var msg = new HttpRequestMessage();

            var v =valueProvider.GetValue();
            _output.WriteLine("TYPE: " + v.GetType().FullName);

            //Act
            applier.Apply(msg);

            var dtOffset = msg.Headers.IfModifiedSince;

            //Assert
            Assert.Equal(new DateTimeOffset(dt), dtOffset);
        }

        [Fact]
        public void ShouldApplyDateTimeOffsetForIfModifiedSince()
        {
            //Arrange
            var dt = new DateTimeOffset(DateTime.Now);
            var desc = new HeaderRequestParameterDescription(0, "If-Modified-Since");
            var valueProvider = new SimpleApiRequestParameterValueProvider(dt);
            var applier = new HeaderParameterApplier(desc, valueProvider);

            var msg = new HttpRequestMessage();

            //Act
            applier.Apply(msg);

            var dtOffset = msg.Headers.IfModifiedSince;

            //Assert
            Assert.Equal(dt, dtOffset);
        }
    }
}
