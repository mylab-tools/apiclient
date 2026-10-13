# MyLab.ApiClient

[![NuGet Version](https://img.shields.io/nuget/v/MyLab.ApiClient.svg)](https://www.nuget.org/packages/MyLab.ApiClient) [![NuGet Downloads](https://img.shields.io/nuget/dt/MyLab.ApiClient.svg)](https://www.nuget.org/packages/MyLab.ApiClient) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE) [![.NET10+](https://img.shields.io/badge/framework-.NET10%2B-8A2BE2)](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0)

.NET библиотека для декларативного описания HTTP API-контрактов и вызова удаленных сервисов через типобезопасный .NET-интерфейс.

## Возможности

- Описание API через интерфейсы и атрибуты (`ApiContract`, `Get`, `Post`, и др.);
- Прозрачный proxy-клиент: вызываете методы интерфейса как обычный объект-сервис;
- Гибкая передача данных: `Path`, `Query`, `Header`, `JsonContent`, `FormContent`, `MultipartContent`, `BinContent`;
- Интеграция с `IServiceCollection` и `IHttpClientFactory`;
- Конфигурация endpoint-ов через `appsettings.json` или код;
- Поддержка детализированного результата вызова (`CallDetails`).

## Быстрый старт

### 1) Опишите контракт API

```csharp
using MyLab.ApiClient.Contracts.Attributes.ForContract;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;

public interface IOrdersApi
{
    [Get("{id}")]
    Task<OrderDto> GetByIdAsync([Path] string id);

    [Post]
    Task CreateAsync([JsonContent] CreateOrderDto dto);
}
```

### 2) Добавьте конфигурацию endpoint-ов

```json
{
  "Api": {
    "IOrdersApi": "https://example.org/api/orders"
  }
}
```

### 3) Зарегистрируйте клиента в DI

```csharp
services
    .AddApiClient<IOrdersApi>()
    .ConfigureApiClient(Configuration);
```

### 4) Используйте контракт как зависимость

```csharp
public class OrdersService(IOrdersApi api)
{
    public Task CreateOrderAsync(CreateOrderDto dto) => api.CreateAsync(dto);
}
```

## Контракт сервиса

Чтобы начать описание сервиса, объявите его контракт в виде интерфейса. **Опционально** контракт может декорироваться атрибутом `ApiContractAttribute`:

```c# 
[ApiContract] //Опционально
public interface IOrdersApi
{
}
```

На уровне контракта можно указать:

* общий для всех методов путь относительно базового пути из настроек
* ключ, по которому будет осуществляться привязка к настройкам

```C#
[ApiContract(Url="v1", Binding="orders")] 
public interface IOrdersApiV1
{
}

//appsettings.json
//
//{
//  "Api": {
//    "orders": "https://example.org/api/orders"
//  }
//}
```

### Методы

#### Асинхронные методы

Все методы контракта `API` должны быть асинхронными, т.е. возвращать `Task` или `Task<T>`, где `T` - ожидаемое содержание ответа.

#### Разметка

Все методы контракта должны быть помечены атрибутами (наследниками `ApiMethodAttribute`), которые соответствуют HTTP-методам:

* `GET`- `GetAttribute`;
* `HEAD` - `HeadAttribute`;
* `POST` - `PostAttribute`;
* `PUT` - `PutAttribute`;
* `PATCH` - `PatchAttribute`;
* `DELETE` - `DeleteAttribute`.

В этих атрибутах можно указать часть пути, относительно пути уровня контракта:

```C#
[ApiContract(Url="orders")] 
public interface IOrderService
{
    [Get("last-year")] // GET orders/last-year
    Task GetLastYearOrdersAsync();
    
    [Post]	// POST orders
    Task PostOrdersAsync();
}
```

### Аргументы

Аргументы метода определяют данные передаваемые в запросе. Для определения места расположения и формата передаваемых данных, используйте наследников атрибута `ApiParameterAttribute`.

#### PathAttribute

Аргумент - часть пути

```C#
[ApiContract("company-services/api")]
public interface IService
{   
    [Get("orders/{id}")]
    Task GetAsync([Path]string id);
}
```

Вызов:

```C#
await srv.GetAsync("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders/2
```

#### QueryAttribute

Аргумент - часть запроса в URL.

```C#
[ApiContract("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task GetAsync([Query]string id);
}
```

Вызов:

```C#
await srv.GetAsync("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders?id=2
```

#### HeaderAttribute

Аргумент - заголовок

```C#
[ApiContract("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task GetAsync([Header("X-Identifier")]string id);
}
```

Вызов:

```C#
await srv.GetAsync("2");
```

Результирующий запрос:

```http
GET /company-services/api/orders

X-Identifier: 2
```

##### Заголовок If-Modified-Since

Поддерживаются аргументы типа DateTime и DateTimeOffset - в этом случае будет правильная сериализация в строку специального формата.

#### HeaderCollectionAttribute

Аргумент - произвольный список заголовков. Тип параметра должен реализовывать интерфейс `IEnumerable<KeyValuePair<string, object>>`;

```C#
[ApiContract("company-services/api")]
public interface IService
{   
    [Get("orders")]
    Task GetAsync([HeaderCollection] Dictionary<string, object> headers);
}
```

Вызов:

```C#
var headers = new Dictionary<string, object>
{
    {"X-Header-1", "foo"}, 
    {"X-Header-2", "bar"}    
}

await srv.GetAsync(headers);
```

Результирующий запрос:

```http
GET /company-services/api/orders

X-Header-1: foo	
X-Header-2: bar
```

#### StringContentAttribute

Аргумент - содержательная часть запроса в строковой форме

```C#
[ApiContract("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task CreateAsync([StringContent] int orderId);
}
```

Вызов:

```C#
await srv.CreateAsync(2);
```

Результирующий запрос:

```http
POST /company-services/api/orders

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
    Task CreateAsync([JsonContent] Order order);
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

await srv.CreateAsync(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders

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
    Task CreateAsync([XmlContent] Order order);
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

await srv.CreateAsync(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders

Content-Type: application/xml

<Order><Id>2</Id></Order>
```

#### FormContentAttribute

Аргумент - содержательная часть запроса в формат `URL encoded form`. Для переопределния имён элементов формы, используйте `UrlFormItemAttribute` на свойствах объекта формы.

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    Task CreateAsync([FormContent] Order order);
}

public class Order
{
    public string Id { get; set; }
    
    [UrlFormItem(Name = "order_number")]
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

await srv.CreateAsync(order);
```

Результирующий запрос:

```http
POST /company-services/api/orders

Content-Type: application/x-www-form-urlencoded

Id=2&order_number=foo
```

#### BinContentAttribute

Аргумент - содержательная часть запроса в бинарном формате

```C#
[Api("company-services/api")]
public interface IService
{       
    [Post("orders")]    
    Task CreateAsync([BinContent] byte[] orderData);
}
```

Вызов:

```C#
var bin = Encoding.UTF8.GetBytes("foo")

await srv.CreateAsync(bin);
```

Результирующий запрос:

```http
POST /company-services/api/orders

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
    Task CreateAsync([MultipartContent] TestMultipartParameter p);
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

await srv.CreateAsync(p);
```

Результирующий запрос:

```http
POST /company-services/api/orders

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

#### CancellationToken

Для передачи в метод `CancellationToken`, который будет использоваться при отправке запроса, просто добавьте этот параметр в любое место списка входящих параметров метода.

```c#
[ApiContract("company-services/api")]
public interface IService
{   
    [Get("orders/{id}")]
    Task GetAsync([Path]string id, CancellationToken ct);
}
```

### Результат

#### Статус-код

`WEB API` может вернуть как успешный ответ: `2xx`, так и ответ с ошибкой: `4xx`(по вине клиента) или `5xx`(по вине сервера). `1xx` и `3xx` статусы обыяно в `WEB API` не применяются. 

`ApiClient` определяет, что вызов сервиса прошёл успешно, если ответ будет содержать статус-код `2xx`. Во всех других случаях возникнет исключение `ResponseCodeException`, в свойствах которого доступны:

* `StatusCode` - код ответа
* `ServerMessage` - текстовое создание ответа (до 1024 символов)

#### Содержание ответа

Выходной параметр метода контракта должен быть `Task` - если содержательная часть метода не ожидается, или `Task<T>` - если ожидается результат типа `Т`.

Поддерживаются следующие типы:

* примитивы: `string`, `bool`, `int`, `uint`, `short`, `ushort`, `long`, `ulong`, `decimal`, `double`, `float`, если `Content-Type` = `application/json` или `text/plain`
* типы значений: `Guid`, `DateTime`, `TimeSpan`, если `Content-Type` = `application/json` или `text/plain`
* бинарные данные: `byte[]` 
* объекты, если `Content-Type` = `application/json`, `application/xml`
* множества: `IEnumerable<T>` , если `Content-Type` = `application/json` или `application/xml`
* проблемы: `ProblemDetails` и `ValidationProblemDetails`, если  `Content-Type` = `application/problem+json`

Если ожидаемый тип ответа не будет соответствовать `Content-Type`, то возникнет исключение `UnexpectedResponseContentTypeException`, которое по мимо всего стандартного имеет поле `ActualContentType`, в котором указан актальный тип содержимого из ответа.

## Конфигурирование

Клиент сопоставляется с настройками конечной точки по ключу привязки. Ключ ищется в секции `Api` (имя по умолчанию, `ApiClientOptions.DefaultSectionName`).

### Привязка к конфигурации

Для каждого контракта библиотека проверяет следующие ключи (в порядке объявления):

1. короткое имя типа контракта — `IOrdersApi`;
2. полное имя типа контракта — `MyApp.Clients.IOrdersApi`;
3. значение `Binding` из атрибута `ApiContract`, если оно задано — например, `orders`.

Достаточно совпадения любого из этих ключей с записью в конфигурации.

#### По имени интерфейса

```csharp
public interface IOrdersApi
{
    // ...
}
```

```json
{
  "Api": {
    "IOrdersApi": "https://example.org/api/orders/"
  }
}
```

#### По полному имени типа

```csharp
namespace MyApp.Clients;

public interface IOrdersApi
{
    // ...
}
```

```json
{
  "Api": {
    "MyApp.Clients.IOrdersApi": "https://example.org/api/orders/"
  }
}
```

#### По явному ключу `Binding`

```csharp
[ApiContract(Binding = "orders")]
public interface IOrdersApi
{
    // ...
}
```

```json
{
  "Api": {
    "orders": "https://example.org/api/orders/"
  }
}
```

### Конфигурация ApiClient

Пример `appsettings.json` с несколькими конечными точками и дополнительными настройками:

```json
{
  "Api": {
    "UrlFormSettings": {
      "EscapeSymbols": true,
      "IgnoreNullValues": true
    },
    "Endpoints": {
      "orders": {
        "Url": "https://example.org/api/orders/",
        "SkipServerSslCertVerification": false
      },
      "ICatalogApi": {
        "Url": "https://example.org/api/catalog/",
        "SkipServerSslCertVerification": true
      }
    }
  }
}
```

Краткая форма — значение ключа сразу задаёт URL:

```json
{
  "Api": {
    "orders": "https://example.org/api/orders/",
    "ICatalogApi": "https://example.org/api/catalog/"
  }
}
```

Загрузка конфигурации в DI:

```csharp
services
    .AddApiClient<IOrdersApi>()
    .AddApiClient<ICatalogApi>()
    .ConfigureApiClient(Configuration);
```

Можно указать имя секции явно:

```csharp
services.ConfigureApiClient(Configuration, "MyApi");
```

### Свободная конфигурация конечной точки

Если настройки конечной точки лежат вне общей секции `Api`, их можно привязать отдельно:

```csharp
// по ключу привязки
services.ConfigureApiClientEndpoint(
    "orders",
    Configuration,
    "ExternalServices:Orders");

// по типу контракта (ключ = FullName типа)
services.ConfigureApiClientEndpoint<IOrdersApi>(
    Configuration,
    "ExternalServices:Orders");
```

Пример соответствующей секции:

```json
{
  "ExternalServices": {
    "Orders": {
      "Url": "https://example.org/api/orders/",
      "SkipServerSslCertVerification": false
    }
  }
}
```

### Раздельная конфигурация

Часто в `appsettings.json` оставляют только адрес конечной точки, а остальные параметры задают в коде через перегрузку `ConfigureApiClient(Action<ApiClientOptions>)`.

Изменения, внесённые в коде, имеют более высокий приоритет, чем значения из конфигурационного файла: при последовательной регистрации сначала загружается файл, затем применяется код — итоговыми становятся значения из кода.

`appsettings.json` — только URL и базовое значение настройки формы:

```json
{
  "Api": {
    "UrlFormSettings": {
      "EscapeSymbols": true,
      "IgnoreNullValues": false
    },
    "orders": "https://example.org/api/orders/"
  }
}
```

В коде адрес берётся из файла, а параметры `UrlFormSettings` переопределяются:

```csharp
services
    .AddApiClient<IOrdersApi>()
    .ConfigureApiClient(Configuration)
    .ConfigureApiClient(opt =>
    {
        opt.UrlFormSettings ??= new ApiUrlFormSettings();
        opt.UrlFormSettings.EscapeSymbols = false;      // было true в appsettings.json
        opt.UrlFormSettings.IgnoreNullValues = true;    // было false в appsettings.json
    });
```

В результате клиент использует URL из конфигурационного файла, а `EscapeSymbols` и `IgnoreNullValues` — значения, заданные в коде.

## Прозрачный прокси

Прозрачный proxy реализует интерфейс контракта и позволяет вызывать API как обычный DI-сервис: без явной работы с `HttpClient` и без построения выражений вызова.

### Регистрация в DI

```csharp
services
    .AddApiClient<IOrdersApi>()
    .ConfigureApiClient(Configuration);
```

Для области видимости `Scoped` используйте `AddApiClientScoped<IOrdersApi>()`.

### Применение

```csharp
public class OrdersService(IOrdersApi ordersApi)
{
    public async Task<OrderDto> GetAsync(string id)
    {
        return await ordersApi.GetByIdAsync(id);
    }
}
```

При успешном ответе (`2xx`) метод возвращает десериализованное содержимое (для `Task<T>`) или завершается без результата (для `Task`).

### Обработка ошибок

При вызове через proxy возможны два типичных исключения:

| Исключение | Когда возникает | Полезные свойства |
|---|---|---|
| `ResponseCodeException` | HTTP-код ответа не входит в `2xx` | `StatusCode`, `ServerMessage` |
| `UnexpectedResponseContentTypeException` | `Content-Type` ответа не соответствует ожидаемому типу результата | `ActualContentType` |

Пример обработки:

```csharp
public async Task<OrderDto?> TryGetAsync(string id)
{
    try
    {
        return await _ordersApi.GetByIdAsync(id);
    }
    catch (ResponseCodeException e) when (e.StatusCode == HttpStatusCode.NotFound)
    {
        // заказ не найден
        return null;
    }
    catch (ResponseCodeException e) when (e.StatusCode == HttpStatusCode.Forbidden)
    {
        // нет доступа
        throw;
    }
    catch (UnexpectedResponseContentTypeException e)
    {
        // например, ожидали application/json, а пришло text/html
        throw new InvalidOperationException(
            $"Unexpected content type: {e.ActualContentType}", e);
    }
}
```

## Детальный вызов

Для сценариев с нестандартными статус-кодами, ветвлением по диапазонам ответов и доступом к дампам запроса/ответа используйте `IApiClientInvoker<TContract>`.

### Инъекция invoker

#### Регистрация в DI

```csharp
services
    .AddApiClient<IOrdersApi>()
    .ConfigureApiClient(Configuration);
```

Та же регистрация, что и для прозрачного proxy: вместе с `IOrdersApi` в контейнер добавляется `IApiClientInvoker<IOrdersApi>`. Для области видимости `Scoped` используйте `AddApiClientScoped<IOrdersApi>()`.

#### Инъекция

```csharp
public class OrdersService(IApiClientInvoker<IOrdersApi> ordersApi)
{
}
```

### Успешный вызов с получением результата

#### Вариант 1 - обработать в делегате

```c#
OrderDto? resOrder = null;

var callDetails = await ordersApi
    .ForExpression(api => api.GetByIdAsync(orderId))
    .When2xx().ProcessResult<OrderDto>((order, code) =>
    {
        // order — десериализованный результат, code — фактический HTTP-код
        ProcessOrder(order);
    })
    .ThrowIfAnotherStatusCode()
    .InvokeAsync();
```

#### Вариант 2 - сохранить в делегате

```csharp
OrderDto? resOrder = null;

var callDetails = await ordersApi
    .ForExpression(api => api.GetByIdAsync(orderId))
    .When2xx().ProcessResult<OrderDto>((order, code) =>
    {
        // order — десериализованный результат, code — фактический HTTP-код
        resOrder = order;
    })
    .ThrowIfAnotherStatusCode()
    .InvokeAsync();
```

#### Вариант 3 - получить из деталей

```c#
var callDetails = await ordersApi
    .ForExpression(api => api.GetByIdAsync(orderId))
    .InvokeAsync();

var resOrder = await callDetails.ReadContentAsync<OrderDto>();
```

### Нестандартные коды ответа

Fluent-API позволяет описать разное поведение для разных кодов и диапазонов:

```csharp
OrderDto? order = null;
ProblemDetails? problem = null;

var callDetails = await ordersApi
    .ForExpression(api => api.GetByIdAsync(orderId))
    .When(HttpStatusCode.OK, HttpStatusCode.Created)
        .ProcessResult<OrderDto>((result, _) => order = result)
    .When(HttpStatusCode.NotFound)
        .ProcessStatusCode(_ => order = null)
    .When4xx()
        .ProcessProblem((p, _) => problem = p)
    .When5xx()
        .ProcessStatusCode(code =>
            throw new InvalidOperationException($"Upstream error: {(int)code}"))
    .ThrowIfAnotherStatusCode()
    .InvokeAsync();
```

Доступные условия:

- `When(HttpStatusCode, params HttpStatusCode[])` — конкретные коды;
- `When1xx()`, `When2xx()`, `When3xx()`, `When4xx()`, `When5xx()` — диапазоны;
- `ProcessResult` / `ProcessResultAsync` — десериализация содержимого;
- `ProcessStatusCode` / `ProcessStatusCodeAsync` — только код ответа;
- `ProcessProblem` / `ProcessValidationProblem` (+ async-варианты) — RFC 7807;
- `ThrowIfAnotherStatusCode()` — если код не обработан ни одним `When*`, будет выброшено `UnexpectedResponseStatusCodeException`.

Асинхронные обработчики:

```csharp
await ordersApi
    .ForExpression(api => api.CreateAsync(dto))
    .When2xx().ProcessResultAsync<int>(async (id, code) =>
    {
        await _log.WriteAsync($"Created {id} with {(int)code}");
    })
    .When4xx().ProcessStatusCodeAsync(async code =>
    {
        await _log.WriteAsync($"Client error {(int)code}");
    })
    .ThrowIfAnotherStatusCode()
    .InvokeAsync();
```

### CancellationToken

Для управления отменой отправки запроса, в метод `InvokeAsync` можно передать `CancellationToken`.

```c#
CancellationToken ct = ...

await ordersApi
    .ForExpression(api => api.CreateAsync(dto))
    .InvokeAsync(ct);
```

Если по контракту метод `API` тоже принимает `CancellationToken`, то следует использовать `CancellationTokenInsert.FromInvoke`, чтобы указать, что для вызова метода `API`  `CancellationToken` нужно брать из параметров метода `InvokeAsync`.

```c#
CancellationToken ct = ...

await ordersApi
    .ForExpression(api => api.CreateAsync(dto, CancellationTokenInsert.FromInvoke))
    .InvokeAsync(ct);
```

### Результат детального вызова — `CallDetails`

`InvokeAsync()` всегда возвращает `CallDetails` со следующими свойствами:

| Свойство | Описание |
|---|---|
| `StatusCode` | HTTP-код ответа |
| `IsOK` | `true`, если код входит в диапазон `2xx` |
| `RequestMessage` | исходный `HttpRequestMessage` |
| `ResponseMessage` | исходный `HttpResponseMessage` |
| `RequestDump` | текстовый дамп запроса |
| `ResponseDump` | текстовый дамп ответа |

Дополнительно:

- `ThrowIfNotOKAsync()` — выбрасывает `ResponseCodeException`, если `IsOK == false`;
- `ReadContentAsync<T>()` — десериализует тело ответа в тип `T`.

### `RequestDump` и `ResponseDump`

Поля заполняются через `HttpMessageDumper` (`MyLab.ApiClient.Tools.HttpMessageDumper`). Дампер сериализует сообщение в читаемый текст: стартовая строка, заголовки и тело (с ограничением размера).

Пример дампа запроса:

```http
POST https://example.org/api/orders/

Content-Type: application/json; charset=utf-8

{"Name":"foo"}
```

Пример дампа ответа:

```http
201 Created

Content-Type: application/json; charset=utf-8

{"Id":"42"}
```

Если тело превышает лимит (`DefaultMaxBodySize` = 10 КБ, настраивается через `MaxRequestBodySize` / `MaxResponseBodySize`), в конец добавляется `... [content is too large to dump]`.

Dumper можно задать в опциях:

```csharp
var options = new ApiClientOptions
{
    Dumper = new HttpMessageDumper
    {
        MaxRequestBodySize = 32 * 1024,
        MaxResponseBodySize = 32 * 1024
    }
};
```

Либо использовать напрямую:

```csharp
var dumper = new HttpMessageDumper();
string requestDump = await dumper.DumpAsync(httpRequestMessage);
string responseDump = await dumper.DumpAsync(httpResponseMessage);
```

## Тестирование

Пакет/проект `MyLab.ApiClient.Test` предоставляет `TestApiFixture<TStartup, TContract>` для интеграционных тестов на базе `WebApplicationFactory`.

### Подключение

Добавьте ссылку на `MyLab.ApiClient.Test` и используйте xUnit class fixture:

```csharp
public class OrdersApiBehavior : IClassFixture<TestApiFixture<Program, IOrdersApi>>
{
    readonly TestApiFixture<Program, IOrdersApi> _fixture;

    public OrdersApiBehavior(
        TestApiFixture<Program, IOrdersApi> fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.Output = output; // опционально: логи запросов/ответов в вывод теста
    }
}
```

### Тест через прозрачный proxy

```csharp
[Fact]
public async Task ShouldReturnOrder()
{
    //Arrange
    var api = _fixture.StartAppWithProxy().Proxy;

    //Act
    var order = await api.GetByIdAsync("42");

    //Assert
    Assert.NotNull(order);
    Assert.Equal("42", order.Id);
}
```

### Тест через `ApiClientInvoker`

```csharp
[Fact]
public async Task ShouldReturnOrderWithDetails()
{
    //Arrange
    var invoker = _fixture.StartAppWithInvoker().Invoker;

    //Act
    var callDetails = await invoker
        .ForExpression(api => api.GetByIdAsync("42"))
        .When2xx().ProcessResult<OrderDto>((_, _) => { })
        .ThrowIfAnotherStatusCode()
        .InvokeAsync();

    var order = await callDetails.ReadContentAsync<OrderDto>();

    //Assert
    Assert.True(callDetails.IsOK);
    Assert.Equal(HttpStatusCode.OK, callDetails.StatusCode);
    Assert.NotNull(order);
}
```

### Дополнительные возможности fixture

- `ServiceOverrider` / параметр `serviceOverrider` — подмена сервисов тестового приложения;
- `HttpClientTuner` / параметр `httpClientTuner` — дополнительная настройка `HttpClient`;
- `StartAppWithProxy()` возвращает `ProxyTestApiAsset<TContract>` (`Proxy`, `HttpClient`, `ServiceProvider`);
- `StartAppWithInvoker()` возвращает `InvokerTestApiAsset<TContract>` (`Invoker`, `HttpClient`, `ServiceProvider`).

Пример с подменой сервисов:

```csharp
var asset = _fixture.StartAppWithProxy(services =>
{
    services.AddSingleton<IOrdersRepository, FakeOrdersRepository>();
});

var order = await asset.Proxy.GetByIdAsync("42");
```
