# MyLab.ApiClient
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.ApiClient)](https://www.nuget.org/packages/MyLab.ApiClient)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Оглавление

* [Обзор](#Обзор)
* [Контракт сервиса](./doc/contract.md)
* [Методы](./doc/methods.md)
* [Результат](./doc/result.md)
* [Вызов](./doc/call.md)
* [DI инъекция](./doc/di.md)
* [Тестирование](./doc/test.md)

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
