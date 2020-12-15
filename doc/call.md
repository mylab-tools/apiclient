## MyLab.ApiClient -Вызов

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

var orderId = await service.Call(s => s.CreateOrder(order)).GetResult();
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
    Task<int> CreateOrder(Order order);
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
    Task CreateOrder(Order order);
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

## 