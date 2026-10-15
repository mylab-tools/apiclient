using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Generator;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Usage;
using Newtonsoft.Json;

namespace GeneratorTests;

public sealed class ApiSpecBehavior : IClassFixture<ApiSpecFixture>
{
    readonly ApiSpecFixture _fixture;

    public ApiSpecBehavior(ApiSpecFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void GeneratedSourcesCompileToContractsAndData()
    {
        Assert.Equal("ТестовыйAPI", _fixture.Document.Namespace);
        Assert.NotNull(_fixture.Require("IЗаказыApi"));
        Assert.NotNull(_fixture.Require("IПользователиApi"));
        Assert.NotNull(_fixture.Require("DocumentShort"));
        Assert.NotNull(_fixture.Require("OrderList"));
        Assert.NotNull(_fixture.Require("OrderDetails"));
        Assert.NotNull(_fixture.Require("OrderComment"));
        Assert.NotNull(_fixture.Require("UserList"));
        Assert.NotNull(_fixture.Require("UserShort"));
    }

    [Fact]
    public void OrdersContractMatchesSpecification()
    {
        var contract = _fixture.Require("IЗаказыApi");
        var binding = contract.GetCustomAttribute<ApiContractAttribute>();
        Assert.NotNull(binding);
        Assert.Equal("Тестовый API", binding.Binding);
        Assert.Null(binding.Url);

        var orders = RequireMethod(contract, "GetOrders");
        AssertMethod(orders, HttpMethod.Get, "orders", "OrderList");
        AssertQuery(orders.GetParameters()[0], "user_id", "userId", typeof(string), optional: true);
        AssertQuery(orders.GetParameters()[1], "page", "page", typeof(int), optional: true);

        var details = RequireMethod(contract, "GetOrdersById");
        AssertMethod(details, HttpMethod.Get, "orders/{id}", "OrderDetails");
        AssertPath(details.GetParameters().Single(), "id", typeof(int));

        var comment = RequireMethod(contract, "PutOrdersByIdComment");
        Assert.Equal(typeof(Task), comment.ReturnType);
        Assert.Equal(HttpMethod.Put, comment.GetCustomAttribute<PutAttribute>()!.HttpMethod);
        Assert.Equal("orders/{id}/comment", comment.GetCustomAttribute<PutAttribute>()!.Url);
        AssertPath(comment.GetParameters()[0], "id", typeof(int));
        var body = comment.GetParameters()[1];
        Assert.Equal("orderComment", body.Name);
        Assert.NotNull(body.GetCustomAttribute<JsonContentAttribute>());
        Assert.Equal("OrderComment", body.ParameterType.Name);
        Assert.True(body.HasDefaultValue);
        Assert.Null(body.DefaultValue);

        var source = _fixture.Document.Files.Single(file => file.Name == "IЗаказыApi.cs").Code;
        Assert.Contains("foo-dev.ru/v1", source, StringComparison.Ordinal);
        Assert.Contains("foo.ru/v1", source, StringComparison.Ordinal);
        Assert.Contains("Все заказы", source, StringComparison.Ordinal);
    }

    [Fact]
    public void UsersContractMatchesSpecification()
    {
        var contract = _fixture.Require("IПользователиApi");
        var users = RequireMethod(contract, "GetUsers");
        AssertMethod(users, HttpMethod.Get, "users", "UserList");
        AssertQuery(users.GetParameters().Single(), "page", "page", typeof(int), optional: true);
    }

    [Fact]
    public void DocumentShortRoundTripsJsonNames()
    {
        var type = _fixture.Require("DocumentShort");
        var value = Activator.CreateInstance(type)!;
        type.GetProperty("Id")!.SetValue(value, 1);
        type.GetProperty("Date")!.SetValue(value, new DateOnly(2026, 5, 4));
        type.GetProperty("NotaryId")!.SetValue(value, "12345");
        type.GetProperty("Name")!.SetValue(value, "Договор купли-продажи");
        type.GetProperty("ClientName")!.SetValue(value, "Петров П.П.");

        var json = JsonConvert.SerializeObject(value);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(1, document.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("2026-05-04", document.RootElement.GetProperty("date").GetString());
        Assert.Equal("12345", document.RootElement.GetProperty("notaryId").GetString());
        Assert.Equal("Договор купли-продажи", document.RootElement.GetProperty("name").GetString());
        Assert.Equal("Петров П.П.", document.RootElement.GetProperty("clientName").GetString());

        var restored = JsonConvert.DeserializeObject(json, type)!;
        Assert.Equal(1, type.GetProperty("Id")!.GetValue(restored));
        Assert.Equal(new DateOnly(2026, 5, 4), type.GetProperty("Date")!.GetValue(restored));
        Assert.Equal("12345", type.GetProperty("NotaryId")!.GetValue(restored));
    }

    [Fact]
    public void DataModelsReflectSchemaNullability()
    {
        var details = _fixture.Require("OrderDetails");
        Assert.Equal(typeof(DateOnly), details.GetProperty("Date")!.PropertyType);
        Assert.Equal(typeof(string), details.GetProperty("UserId")!.PropertyType);
        Assert.Equal(NullabilityState.Nullable, new NullabilityInfoContext().Create(details.GetProperty("UserId")!).WriteState);
        Assert.Equal("OrderComment", details.GetProperty("Comment")!.PropertyType.Name);
        Assert.Equal(NullabilityState.Nullable, new NullabilityInfoContext().Create(details.GetProperty("Comment")!).WriteState);

        var comment = _fixture.Require("OrderComment");
        Assert.Equal(typeof(string), comment.GetProperty("Text")!.PropertyType);
        Assert.Equal(typeof(int), Nullable.GetUnderlyingType(comment.GetProperty("Level")!.PropertyType));

        var orders = _fixture.Require("OrderList");
        var items = orders.GetProperty("Items")!;
        Assert.Equal(typeof(List<>), items.PropertyType.GetGenericTypeDefinition());
        Assert.Equal("DocumentShort", items.PropertyType.GenericTypeArguments[0].Name);
        Assert.Equal(typeof(bool), orders.GetProperty("HasNext")!.PropertyType);

        var users = _fixture.Require("UserList");
        Assert.Equal("UserShort", users.GetProperty("Items")!.PropertyType.GenericTypeArguments[0].Name);
    }

    [Fact]
    public async Task GeneratedOrdersContractPerformsRequests()
    {
        var contract = _fixture.Require("IЗаказыApi");
        var processor = new CapturingProcessor(() => JsonResponse(
            """
            {"items":[{"id":1,"date":"2026-05-04","notaryId":"n-1","name":"doc","clientName":"client"}],"hasNext":true}
            """));
        var proxy = CreateProxy(contract, processor);

        var getOrders = contract.GetMethod("GetOrders")!;
        var getTask = (Task)getOrders.Invoke(proxy, ["12345", 2])!;
        await getTask;
        var orders = getTask.GetType().GetProperty("Result")!.GetValue(getTask)!;

        Assert.Equal("GET", processor.Method);
        Assert.Equal("orders?user_id=12345&page=2", processor.Uri);
        Assert.Equal(true, orders.GetType().GetProperty("HasNext")!.GetValue(orders));
        var items = (System.Collections.IEnumerable)orders.GetType().GetProperty("Items")!.GetValue(orders)!;
        var item = items.Cast<object>().Single();
        Assert.Equal("n-1", item.GetType().GetProperty("NotaryId")!.GetValue(item));
        Assert.Equal(new DateOnly(2026, 5, 4), item.GetType().GetProperty("Date")!.GetValue(item));

        var commentType = _fixture.Require("OrderComment");
        var comment = Activator.CreateInstance(commentType)!;
        commentType.GetProperty("Text")!.SetValue(comment, "Поспешить!");
        commentType.GetProperty("Level")!.SetValue(comment, 3);

        processor.Respond = () => new HttpResponseMessage(HttpStatusCode.OK);
        var put = contract.GetMethod("PutOrdersByIdComment")!;
        await (Task)put.Invoke(proxy, [7, comment])!;

        Assert.Equal("PUT", processor.Method);
        Assert.Equal("orders/7/comment", processor.Uri);
        Assert.Equal("application/json", processor.ContentType);
        using var body = JsonDocument.Parse(processor.Body!);
        Assert.Equal("Поспешить!", body.RootElement.GetProperty("text").GetString());
        Assert.Equal(3, body.RootElement.GetProperty("level").GetInt32());
    }

    static MethodInfo RequireMethod(Type contract, string name)
    {
        return contract.GetMethod(name) ?? throw new InvalidOperationException("Method " + name + " was not generated.");
    }

    static void AssertMethod(MethodInfo method, HttpMethod httpMethod, string url, string resultName)
    {
        var attribute = method.GetCustomAttribute<ApiMethodAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(httpMethod, attribute.HttpMethod);
        Assert.Equal(url, attribute.Url);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(resultName, method.ReturnType.GenericTypeArguments[0].Name);
    }

    static void AssertQuery(ParameterInfo parameter, string wireName, string parameterName, Type valueType, bool optional)
    {
        Assert.Equal(parameterName, parameter.Name);
        var attribute = parameter.GetCustomAttribute<QueryAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(wireName, attribute.Name);
        AssertOptional(parameter, valueType, optional);
    }

    static void AssertPath(ParameterInfo parameter, string wireName, Type valueType)
    {
        var attribute = parameter.GetCustomAttribute<PathAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(wireName, attribute.Name);
        Assert.Equal(valueType, parameter.ParameterType);
        Assert.False(parameter.HasDefaultValue);
    }

    static void AssertOptional(ParameterInfo parameter, Type valueType, bool optional)
    {
        if (valueType.IsValueType)
            Assert.Equal(valueType, Nullable.GetUnderlyingType(parameter.ParameterType));
        else
            Assert.Equal(valueType, parameter.ParameterType);

        Assert.Equal(optional, parameter.HasDefaultValue);
        if (optional)
            Assert.Null(parameter.DefaultValue);
    }

    static object CreateProxy(Type contract, IRequestProcessor processor)
    {
        var create = typeof(ApiClientProxy).GetMethod(nameof(ApiClientProxy.CreateForContract))!;
        return create.MakeGenericMethod(contract).Invoke(null, [processor, new ApiClientOptions()])!;
    }

    static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    sealed class CapturingProcessor : IRequestProcessor
    {
        public Func<HttpResponseMessage> Respond { get; set; }
        public string? Method { get; private set; }
        public string? Uri { get; private set; }
        public string? Body { get; private set; }
        public string? ContentType { get; private set; }

        public CapturingProcessor(Func<HttpResponseMessage> respond)
        {
            Respond = respond;
        }

        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Method = request.Method.Method;
            Uri = request.RequestUri?.OriginalString;
            if (request.Content != null)
            {
                Body = await request.Content.ReadAsStringAsync(cancellationToken);
                ContentType = request.Content.Headers.ContentType?.MediaType;
            }
            else
            {
                Body = null;
                ContentType = null;
            }

            return Respond();
        }
    }
}

public sealed class ApiSpecFixture
{
    readonly CompiledAssembly _compiled;

    public ApiSpecFixture()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "api-spec.yml");
        Document = ApiContractGenerator.Generate(File.ReadAllText(path));
        _compiled = GeneratedCompiler.Compile(Document);
    }

    public GeneratedDocument Document { get; }

    public Type Require(string name)
    {
        return _compiled.Assembly.GetType(Document.Namespace + "." + name)
               ?? throw new InvalidOperationException("Type '" + name + "' was not generated.");
    }
}
