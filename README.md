# MyLab.ApiClient
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.ApiClient)](https://www.nuget.org/packages/MyLab.ApiClient)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Обзор 

`MyLab.ApiClient` предоставляет возможность создавать клиенты для `WEB API` на основе контрактов.

Чтобы описать `WEB API` контракт, следует:

* объявить контракт сервиса как интерфейс
* пометить интерфейс атрибутом `ApiAttribute`
* объявить асинхронные методы, которые будут соответствовать конечным точкам сервиса
* пометить соответствующими атрибутами (`ApiMethodAttribute` или наследниками)
* указать у методов типы возвращаемых параметров в соответствии с содержанием, которое возвращает сервис
* указать у методов аргументы, соответствующие передаваемым в запросе данным
* пометить аргументы соответствующими атрибутами, указывающими на расположение и формат этих данных (наследники `ApiParameterAttribute`)

Описание контракта сервиса:

```C#
[Api("api")]
public interface IServiceContract
{   
    [Post("orders")]
    Task<int> CreateOrder([JsonContent] Order order);
}
```

Описание контракта данных (не требует дополнительной разметки):

```C#
public class Order
{
	public string Foo { get; set; }
}
```

Контроллер сервера:

```C#
[ApiController]
[Route("api")]
public class OrderController : ControllerBase
{
    [HttpPost("orders")]
    public IActionResult CreateOrder([FromBody]Order order)
    {
        //...
        return Ok(newOrderId);
    }
}
```

Использование:

```C#
HttpClient httpClient = ...
var s = ApiClient<ITestServer>.Create(new SingleHttpClientProvider(httpClient));

var order = new Order{ Foo ="bar" }
int newOrderId = await _client.Request(s => s.CreateOrder(order)).GetResultAsync();
```

## Контракт сервиса

Чтобы начать описание сервиса, объявите его контракт в виде интерфейса.

Используйте `ApiAttribute` чтобы отметить интерфейс-контракт сервиса:

```C#
[Api]
public interface IService
{
    //...
}
```

В этом атрибуте можно указать базовый путь к сервису, который будет использоваться как базовый для формирования полного адреса запроса с учётом относительных путей конечных точек (методов):

```C#
[Api("orders/v1")]
public interface IService
{
    //...
}
```

## Методы

### Асинхронные методы

Все методы контракта `API` должны быть асинхронными, т.е. возвращать `Task` или `Task<>`.

### Разметка

Метод контракта должен быть помечен атрибутом `ApiMethodAttribute` или его наследником. Здесь определяется относительный путь и `HTTP`-метод. Также у `ApiMethodAttribute`  есть ряд наследников для основных случаев:

```C#
[Api]
public interface IService
{
    [ApiMethod("orders", HttpMethod.Get)]
    Task GetOrders1();
    
    [Get("orders")]
    Task GetOrders2();
    
    [Get]
    Task GetOrders3();
    
    [Post]
    Task PostOrders();
    
    [Put]
    Task PutOrders();
    
    [Head]
    Task HeadOrders();
    
    [Delete]
    Task DeleteOrders();
}
```

### Аргументы

Аргументы метода определяют данные передаваемые в запросе. Для определения места расположения и формата передаваемых данных, используйте наследников атрибута `ApiParameterAttribute`.

#### PathAttribute

Аргумент - часть пути

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders/{id}")]
    Task Get([Path]string id);
}
```

Вызов:

```C#
await srv.Get("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders/2
```

#### QueryAttribute

Аргумент - часть запроса в URL.

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task Get([Query]string id);
}
```

Вызов:

```C#
await srv.Get("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders?id=2
```

#### HeaderAttribute

Аргумент - заголовок

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task Get([Header("X-Identifier")]string id);
}
```

Вызов:

```C#
await srv.Get("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders
Headers:
X-Identifier: 2
```

#### HeaderCollectionAttribute

Аргумент - произвольный список заголовков. Тип параметра должен реализовывать интерфейс `IEnumerable<KeyValuePair<string, object>>`;

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task Get([HeaderCollection] Dictionary<string, object> headers);
}
```

Вызов:

```C#
var headers = new Dictionary<string, object>
{
	{"X-Header-1", "foo"}, 
    {"X-Header-2", "bar"}    
}

await srv.Get(headers);
```

Результирующий запрос:

```http
GET /company-services/api/orders
Headers:
X-Header-1: foo	
X-Header-2: bar
```

#### StringContentAttribute

Аргумент - содержательная часть запроса в строковой форме

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task Create([StringContent] int orderId);
}
```

Вызов:

```C#
await srv.Create(2);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
X-Header-1: foo	
X-Header-2: bar
Content-Type: text/plain

2
```

#### JsonContentAttribute

Аргумент - содержательная часть запроса в формате `JSON`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task Create([JsonContent] Order order);
}

public class Order
{
	public string Id { get; set; }
}
```

Вызов:

```C#
var order = new Order
{
    Id = "2"
}

await srv.Create(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
Content-Type: application/json

{"Id":"2"}
```

#### XmlContentAttribute

Аргумент - содержательная часть запроса в формате `XML`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task Create([XmlContent] Order order);
}

public class Order
{
	public string Id { get; set; }
}
```

Вызов:

```C#
var order = new Order
{
    Id = "2"
}

await srv.Create(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
Content-Type: application/xml

<Order><Id>2</Id></Order>
```

#### FormContentAttribute

Аргумент - содержательная часть запроса в формат `URL encoded form`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task Create([FormContent] Order order);
}

public class Order
{
	public string Id { get; set; }
    public string Number { get; set; }
}
```

Вызов:

```C#
var order = new Order
{
    Id = "2",
    Number = "foo"
}

await srv.Create(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
Content-Type: application/x-www-form-urlencoded

Id=2&Number=foo
```

#### BinContentAttribute

Аргумент - содержательная часть запроса в бинарном формате

```C#
[Api("company-services/api")]
public interface IService
{       
    [Post("orders")]    
    Task Create([BinContent] byte[] orderData);
}
```

Вызов:

```C#
var bin = Encoding.UTF8.GetBytes("foo")

await srv.Create(bin);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
Content-Type: application/octet-stream

foo
```

#### MultipartContentAttribute

Аргумент - содержательная часть запроса в формате `multipart-form`. Параметр должен реализовывать интерфейс `IMultipartContentParameter`.

```C#
[Api("company-services/api")]
public interface IService
{       
    [Post("orders")]    
    Task Create([MultipartContent] TestMultipartParameter p);
}

 public class TestMultipartParameter : IMultipartContentParameter
 {
     public string Part1 { get; set; }
     public string Part2 { get; set; }

     public void AddParts(MultipartFormDataContent content)
     {
         content.Add(new StringContent(Part1), "part1");
         content.Add(new StringContent(Part2), "part2");
     }
 }
```

Вызов:

```C#
var p = new TestMultipartParameter{ Part1 = "fo", Part2 = "o"}

await srv.Create(p);
```

Результирующий запрос:

```http
POST /company-services/api/orders
Headers:
Content-Type: multipart/form-data; boundary="2150a4df-de36-421a-8ef7-028f86f90403"

--2150a4df-de36-421a-8ef7-028f86f90403

Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=part1


fo

--2150a4df-de36-421a-8ef7-028f86f90403
Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=part2


o

--2150a4df-de36-421a-8ef7-028f86f90403--
```

## Результат

### Статус-код

`WEB API` может вернуть как успешный ответ, так и ответ с шибкой. Положительным ответом считаются ответы со статус-кодом `2xx`, а `4xx` и `5xx` - ошибочными. (`3xx` при разработке API обычно не используются) 

Часто при проектировании `WEB API` ответы 2хх, как и 4хх наделяют особым смыслом. Поэтому важно проверять, что статус-код входит в определённое подмножество установленных возможных статус-кодов.

Для этого в `MyLab.ApiCLient` есть атрибут `ExpectedCodeAttribute`. Отметьте на целевом методе статус-коды, которые ожидаются в ответ на вызов сервера:

```C#
[Api]
public interface IService
{
    [ExpectedCode(HttpStatusCode.BadRequest)]
    [Get("orders/count")]
    Task<int> GetOrdersCount();
}
```

Алгоритм проверки статус-кода выглядит следующим образом:

* если код == 200 - успех
* если код есть в списке, определённом атрибутами `ExpectedCodeAttribute` - успех
* ошибка `ResponseCodeException`

### Содержание ответа

Тип содержания определяется типом возвращаемым значением соответствующего метода. Поддерживаются следующие типы:

* `void` - если важен только статус-код ответа 
* примитивы: `string`, `bool`, `int`, `uint`, `double`
* типы значений: `DateTime`, `TimeSpan`, `Guid`
* объекты/структуры: только если содержательная часть ответа в формате `XML`, `JSON`или `url-encoded-form`

В случае, если содержательная часть ответа отсутствует, метод будет возвращать значения по умолчанию:

- `null` для ссылочных типов;
- `default()` - для типов значений.

## Вызов

### Результат

На следующем примере показан вызов сервиса с получением результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    Task<int> CreateOrder(Order order);
}

//....

var orderId = await service.Request(s => s.CreateOrder(order)).GetResultAsync();
```

Вызов сервиса без получения результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    Task CreateOrder(Order order);
}

//....

await service.Call(s => s.CreateOrder(order)).CallAsync();
```

При получении непредвиденного статус-кода, кроме `200 (OK)`, метод `GetResultAsync` выдаёт исключение `ResponseCodeException`. Это можно использовать следующим образом:

```C#
try
{
    await service.Request(s => s.CreateOrder(order)).GetResultAsync();
}
catch(ResponseCodeException e) when (e.StatusCode == HttpStatusCode.BadRequest)
{
    //when status code = 400 
}
catch(ResponseCodeException e) when (e.StatusCode == HttpStatusCode.Forbidden)
{
    //when status code = 403
}
```

### Детализация

Детализация по вызову представляет собой объект, содержащий всё необходимое для составления представления о выполненном запросе и полученном ответе:

```C#
/// <summary>
/// Contains detailed service call information with response
/// </summary>
public class CallDetails<T> : CallDetails
{
    /// <summary>
    /// Expected response content
    /// </summary>
    public T ResponseContent { get; set; }
}

/// <summary>
/// Contains detailed service call information 
/// </summary>
public class CallDetails
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// Gets true if status code is unexpected
    /// </summary>
    public bool IsUnexpectedStatusCode { get; set; }
    /// <summary>
    /// Text request dump
    /// </summary>
    public string RequestDump { get; set; }
    /// <summary>
    /// Text response dump
    /// </summary>
    public string ResponseDump { get; set; }
    /// <summary>
    /// Response object
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; set; }
    /// <summary>
    /// Request object
    /// </summary>
    public HttpRequestMessage RequestMessage { get; set; }
}
```

На следующем примере показан вызов сервиса с получением детализированного результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    Task<int> CreateOrder(Order order);
}

//....

CallDetails<int> response = await service.Request(s => s.CreateOrder(order)).GetDetailedAsync();
```

Вызов сервиса без получения результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    Task CreateOrder(Order order);
}

//....

CallDetails response = await service.Request(s => s.CreateOrder(order)).GetDetailedAsync();
```

В случае, когда метод контракта сервиса не имеет возвращаемого значения, метод `GetDetailedAsync` возвращает объект детализации без содержимого ответа: `CallDetails`.

При получении непредвиденного статус-кода, кроме `200 (OK)`, метод `GetDetailedAsync` не выбрасывает исключение, а устанавливает свойства объекта детализации `IsUnexpectedStatusCode` в `true`.

```C#
var response = await service.Request(s => s.CreateOrder(order)).GetResultAsync();

if (response.IsUnexpectedStatusCode)
{
    switch (response.StatusCode)
    {
        case HttpStatusCode.BadRequest:
            //when status code = 400
            break;
        case HttpStatusCode.Forbidden:
            //when status code = 403
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}
```

Пример дампа запроса из детализации:

```
POST http://localhost/test/ping/body/obj/json

Cookie: <empty>
Content-Type: application/json; charset=utf-8

{"TestValue":"foo"}
```

Пример дампа ответа из детализации:

```
200 OK

Content-Type: text/plain; charset=utf-8

foo
```

## DI инъекция

### Обзор

Особенности DI инъекции:

* определение настроек подключения к удалённым API через конфигурацию;
* регистрация контрактов API на этапе конфигурирования сервисов в `Startup.ConfigureServices`;
* сопоставление зарегистрированных контрактов и конфигураций;
* получение клиентов в целевых объектах в качестве зависимостей двумя способами.

Данный механизм основан на использовании [фабрики HttpClient](https://docs.microsoft.com/ru-ru/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)-ов. 

### Конфигурирование

Целью загрузки конфигурации является создание именованных фабрик http-клиентов в соответствии параметрам из конфигурации.

На примере ниже представлены способы определения конфигураций подключений к API:

```C#
public class Startup
{
    public Startup(IConfiguration configuration)
    {
    	Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Simple case - using default section name "Api"
    	services.AddApiClients(null, Configuration);
        
        // Or specify custom section name
        services.AddApiClients(null, Configuration, "MyApiSectionName");

        // Or create options directly in code
        services.AddApiClients(null, new ApiClientsOptions
            {
                List = new Dictionary<string, ApiConnectionOptions>
                {
                    { "foo", new ApiConnectionOptions{Url = "http://test.com"}}
                }
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    	...
    }
}
```

Объектная модель конфигурации:

```C#
/// <summary>
/// Contains api clients infrastructure options
/// </summary>
public class ApiClientsOptions
{
    /// <summary>
    /// List of api connections options
    /// </summary>
    public Dictionary<string, ApiConnectionOptions> List { get; set; }
}

/// <summary>
/// Contains api connection options
/// </summary>
public class ApiConnectionOptions
{
    /// <summary>
    /// API base url
    /// </summary>
    public string Url { get; set; }
}
```

Пример файла конфигурации:

```json
{
  "Api": {
    "List": {
      "foo": { "Url": "http://foo-test.com" },
      "bar": { "Url": "http://bar-test.com" }
    }
  }
}
```

### Сопоставление контрактов 

Для сопоставления контракта API и настроек конфигурации используется ключ контракта, указываемый в атрибуте `ApiAttribute` в поле `Key`. 

Пример контракта `API` с указанным кодом контракта:

```c#
 [Api("echo", Key = "foo")]
 interface ITestServer
 {
     [Get]
     Task<string> Echo([JsonContent]string msg);
 }
```

Конфигурационный файл с сопоставленной записью:

```json
{
  "Api": {
    "List": {
      "foo": { "Url": "http://foo-test.com" }, //<--- here it is 
      "bar": { "Url": "http://bar-test.com" }
    }
  }
}
```

### Инъекция IHttpClientFactory

Инъекция `IHttpClientFactory` в объект-потребитель позволяет создавать объекты `ApiClient<>` для дальнейшей работы с `API` через методы `Call` с передачей `Expressions`-выражений вызова методов контракта `API`. 

Это может быть полезно, например, если в дальнейшем нужно получить детали вызова метода API.

Ниже приведён пример класса-потребителя с использованием инъекции `IHttpClientFactory`:

```C#
class TestServiceForHttpClientFactory
{
    private readonly ApiClient<ITestServer> _server;

    public TestServiceForHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _server = httpClientFactory.CreateApiClient<ITestServer>();
    }

    public async Task<string> TestMethod(string msg, ITestOutputHelper log)
    {
        var resp = await _server.Request(s => s.Echo(msg)).GetDetailedAsync();

        log.WriteLine("Resquest dump:");
        log.WriteLine(resp.RequestDump);
        log.WriteLine("Response dump:");
        log.WriteLine(resp.ResponseDump);

        return resp.ResponseContent;
    }
}
```

Для создания клиента таким образом, у контракта `API`  должен быть определён ключ контракта в атрибуте `ApiAttribute` и должна быть загружена конфигурация с соответствующим ключом.

### Прозрачное прокси 

#### Инъекция

Инъекция прозрачного прокси в объект-потребитель позволяет использовать контракт `API` так же, как любой другой сервис, добавляемый через DI контейнер. Кроме того, это значительно упрощает тестирование класса-потребителя и избавляет от лишнего погружения в детали реализации зависимости.

Для обеспечения инъекции прозрачных прокси контрактов API необходимо зарегистрировать эти контракты следующим образом:

```C#
public void ConfigureServices(IServiceCollection services)
{
    // Simple case - using default section name "Api"
    services.AddApiClients(
        registrar => 
        {
            registrar.RegisterContract<ITestServer>();
        }, Configuration);
}
```

Для регистрации контракта таким образом, у контракта `API`  должен быть определён ключ контракта в атрибуте `ApiAttribute` и должна быть загружена конфигурация с соответствующим ключом.

Ниже приведён пример использования инъекции прозрачного прокси:

```C#
class TestServiceForProxy
{
    private readonly ITestServer _server;

    public TestServiceForProxy(ITestServer server)
    {
    	_server = server;
    }

    public Task<string> TestMethod(string msg)
    {
    	return _server.Echo(msg);
    }
}
```

#### Детализация

Прозрачное прокси поддерживает возврат детализации (`CallDetails`) методом контракта:

```C#
[Api("echo")]
interface ITestServer
{
    [Get]
    Task<CallDetails<string>> CallEchoAndGetDetails([JsonContent] string msg);
    
    [Get]
    Task<CallDetails> CallEchoAndGetDetailsWithoutResonse([JsonContent] string msg);
}

//....
    
CallDetails<string> call = await api.CallEchoAndGetDetails("foo");

CallDetails call = await api.CallEchoAndGetDetailsWithoutResonse("foo");
```

## Тестирование 

При написании функциональных и интеграционных тестов, для взаимодействия с сервисом через его контракт `API`, используйте класс `ApiClient<>` и провайдер `DelegateHttpClientProvider`. 

Ниже приведены примеры тестов с разным подходом в создании клиентов в зависимости от особенностей взаимодействия:

* можно создать один `api`-клиент на тестовый класс, если в каждом методе, где он используется, будет один вызов сервиса;

```C#
 public class TestServerBehavior : IClassFixture<WebApplicationFactory<Startup>>
 {
     private readonly ApiClient<ITestServer> _client;

     public TestServerBehavior(
         WebApplicationFactory<Startup> webApplicationFactory)
     {
         var clientProvider = new DelegateHttpClientProvider(
             webApplicationFactory.CreateClient);

         _client = new ApiClient<ITestServer>(clientProvider); 
     }

     [Fact]
     public async Task ShouldReturnPayload()
     {
         //Arrange

         //Act 
         var result = await _client.Request(s => s.Get()).GetResultAsync();

         //Assert
         Assert.NotNull(result);
     }

     [Api("test/resource")]
     interface ITestServer
     {
         [Get]
         Task<string> Get();
     }
 }
```

+ можно создать `HttpClient` в тестовом методе, если будут многократные запросы к сервису.

```C#
public class TestServerBehavior : IClassFixture<WebApplicationFactory<Startup>>
 {
     private readonly WebApplicationFactory<Startup> _webApplicationFactory;

     public TestServerBehavior(
         WebApplicationFactory<Startup> webApplicationFactory)
     {
         _webApplicationFactory = webApplicationFactory;
     }

     [Fact]
     public async Task ShouldReturnPayload()
     {
         //Arrange
         var clProvider = new SingleHttpClientProvider(
             _webApplicationFactory.CreateClient());
         var client = new ApiClient<ITestServer>(clProvider);

         //Act
         await client.Request(s => s.Post("foo")).GetResultAsync();
         var result = await client.Request(s => s.Get()).GetResultAsync();

         //Assert
         Assert.Equal("foo", result);
     }

     [Api("test/resource")]
     interface ITestServer
     {
         [Post]
         Task Post([StringContent]string str);
         [Get]
         Task<string> Get();
     }
 }
```

