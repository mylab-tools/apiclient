# MyLab.ApiClient
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.ApiClient)](https://www.nuget.org/packages/MyLab.ApiClient)
```
Поддерживаемые платформы: .NET Standard 2.0+, .NET Framework 4.6.1+, .NET Core 2.0+
```
## Обзор

`MyLab.ApiClient` предоставляет возможность создавать клиенты для `WEB API` на основе контрактов.

Чтобы описать `WEB API` контракт, следует:

* объявить контракт сервиса как интерфейс
* пометить интерфейс атрибутом `ApiAttribute`
* объявить методы, которые будут соответствовать конечным точкам сервиса
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
    int CreateOrder([JsonContent] Order order);
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
int newOrderId = await _client.Call(s => s.CreateOrder(order)).GetResult();
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

### Разметка

Метод контракта должен быть помечен атрибутом `ApiMethodAttribute` или его наследником. Здесь определяется относительный путь и `HTTP`-метод. Также у `ApiMethodAttribute`  есть ряд наследников для основных случаев:

```C#
[Api]
public interface IService
{
    [ApiMethod("orders", HttpMethod.Get)]
    void GetOrders1();
    
    [Get("orders")]
    void GetOrders2();
    
    [Get]
    void GetOrders3();
    
    [Post]
    void PostOrders();
    
    [Put]
    void PutOrders();
    
    [Head]
    void HeadOrders();
    
    [Delete]
    void DeleteOrders();
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
    void Get([Path]string id);
}

//Result path with id=2: 
//		`/company-services/api/orders/2`
```

#### QueryAttribute

Аргумент - часть запроса в URL.

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders")]
    WebApiCall Get([Query]string id);
}

//Result path with id=2: 
//		`/company-services/api/orders?id=2`
```

#### HeaderAttribute

Аргумент - заголовок

```C#
[Api("company-services/api")]
public interface IService
{   
    [Get("orders")]
    WebApiCall Get([Header("X-Identifier")]string id);
}

//Result header with id=2: 
//		`X-Identifier: 2`
```

#### StringContentAttribute

Аргумент - содержательная часть запроса в строковой форме

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    void Create([StringContent] int orderId);
}

//Result content with Id=2: 2
```

#### JsonContentAttribute

Аргумент - содержательная часть запроса в формате `JSON`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    void Create([JsonContent] Order order);
}

public class Order
{
	public string Id { get; set; }
}

//Result content with Id=2: 
//		{"Id":"2"}
```

#### XmlContentAttribute

Аргумент - содержательная часть запроса в формате `XML`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    void Create([XmlContent] Order order);
}

public class Order
{
	public string Id { get; set; }
}

//Result content with Id=2: 
//		<Order><Id>2</Id></Order>
```

#### FormContentAttribute

Аргумент - содержательная часть запроса в формат `URL encoded form`

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    void Create([FormContent] Order order);
}

public class Order
{
	public string Id { get; set; }
    public string Number { get; set; }
}

//Result content with Id=2 and Number=foo: 
//		Id=2&Number=foo
```

#### BinContentAttribute

Аргумент - содержательная часть запроса в бинарном формате

```C#
[Api("company-services/api")]
public interface IService
{   
    [Post("orders")]
    void Create([BinContent] byte[] orderData);
}
```

Аргумент должен быть типа `byte[]`.

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
    int GetOrdersCount();
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

## Вызов

### Результат

На следующем примере показан вызов сервиса с получением результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    int CreateOrder(Order order);
}

//....

var orderId = await service.Call(s => s.CreateOrder(order)).GetResult();
```

Вызов сервиса без получения результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    void CreateOrder(Order order);
}

//....

await service.Call(s => s.CreateOrder(order)).GetResult();
```

При получении непредвиденного статус-кода, кроме `200 (OK)`, метод `GetResult` выдаёт исключение `ResponseCodeException`. Это можно использовать следующим образом:

```C#
try
{
    await service.Call(s => s.CreateOrder(order)).GetResult();
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
/// Contains detailed service call information
/// </summary>
public class CallDetails<T>
{
    /// <summary>
    /// Expected response content
    /// </summary>
    public T ResponseContent { get; set; }
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
    int CreateOrder(Order order);
}

//....

var response = await service.Call(s => s.CreateOrder(order)).GetDetailed();
```

Вызов сервиса без получения результата:

```C#
[Api]
public interface IService
{
    [Post("orders")]
    void CreateOrder(Order order);
}

//....

var response = await service.Call(s => s.CreateOrder(order)).GetDetailed();
```

В случае, когда метод контракта сервиса не имеет возвращаемого значения, метод `GetDetailed` возвращает объект детализации со строковым содержимым ответа: `CallDetails<string>`.

При получении непредвиденного статус-кода, кроме `200 (OK)`, метод `GetDetailed` не выбрасывает исключение, а устанавливает свойства объекта детализации `IsUnexpectedStatusCode` в `true`.

```C#
var response = await service.Call(s => s.CreateOrder(order)).GetResult();

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

