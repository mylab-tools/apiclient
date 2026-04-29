# MyLab.ApiClient

[![NuGet Version](https://img.shields.io/nuget/v/MyLab.ApiClient.svg)](https://www.nuget.org/packages/MyLab.ApiClient) [![NuGet Downloads](https://img.shields.io/nuget/dt/MyLab.ApiClient.svg)](https://www.nuget.org/packages/MyLab.ApiClient) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE) [![.NET10+](https://img.shields.io/badge/framework-.NET10%2B-8A2BE2)](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0)

.NET библиотека для декларативного описания HTTP API-контрактов и вызова удаленных сервисов через типобезопасный .NET-интерфейс.

## Возможности

- Описание API через интерфейсы и атрибуты (`ApiContract`, `Get`, `Post`, и др.);
- Прозрачный proxy-клиент: вызываете методы интерфейса как обычный сервис;
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
public interface IOrderService
{
    [Get("last-year")]
    Task GetLastYearOrdersAsync();
    
    [Post]
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
Headers:
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
