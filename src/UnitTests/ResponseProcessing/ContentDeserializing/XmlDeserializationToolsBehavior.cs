using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(XmlDeserializationTools))]
public class XmlDeserializationToolsBehavior
{
    [Fact]
    public async Task ShouldDeserializeSimpleXmlObject()
    {
        //Arrange
        var tools = new XmlDeserializationTools();
        var content = new StringContent(
            """<Item><Name>foo</Name></Item>""",
            Encoding.UTF8,
            "application/xml");

        //Act
        var result = await tools.ReadObjectXml(content, typeof(Item));

        //Assert
        var item = Assert.IsType<Item>(result);
        Assert.Equal("foo", item.Name);
    }

    [Fact]
    public async Task ShouldProhibitDtdWhenReadingRootElement()
    {
        //Arrange
        var tools = new XmlDeserializationTools();
        var content = CreateXmlContent("""
                                       <?xml version="1.0"?>
                                       <!DOCTYPE Item [
                                         <!ENTITY xxe SYSTEM "file:///etc/passwd">
                                       ]>
                                       <Item><Name>&xxe;</Name></Item>
                                       """);

        //Act
        var exception = await Record.ExceptionAsync(() => tools.ReadObjectXml(content, typeof(Item)));

        //Assert
        AssertDtdProhibited(exception);
    }

    [Fact]
    public async Task ShouldProhibitDtdWhenDeserializingWithExplicitXmlRoot()
    {
        //Arrange
        var tools = new XmlDeserializationTools();
        var content = CreateXmlContent("""
                                       <?xml version="1.0"?>
                                       <!DOCTYPE RootItem [
                                         <!ENTITY xxe SYSTEM "file:///etc/passwd">
                                       ]>
                                       <RootItem><Name>&xxe;</Name></RootItem>
                                       """);

        //Act
        var exception = await Record.ExceptionAsync(() => tools.ReadObjectXml(content, typeof(RootItem)));

        //Assert
        AssertDtdProhibited(exception);
    }

    static StringContent CreateXmlContent(string xml) =>
        new(xml, Encoding.UTF8, "application/xml");

    static void AssertDtdProhibited(Exception? exception)
    {
        Assert.NotNull(exception);

        var xmlException = exception as XmlException
                           ?? exception.InnerException as XmlException;

        Assert.NotNull(xmlException);
        Assert.Contains("DTD is prohibited", xmlException.Message, StringComparison.OrdinalIgnoreCase);
    }

    public class Item
    {
        public string Name { get; set; } = string.Empty;
    }

    [System.Xml.Serialization.XmlRoot("RootItem")]
    public class RootItem
    {
        public string Name { get; set; } = string.Empty;
    }
}
