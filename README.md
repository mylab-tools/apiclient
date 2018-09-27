# MyLab.ApiClient
For .NET Core 2.1+

Provides abilities to make client for `WEB API` based on contracts.

To make `WEB API` client you should:
* declare service contract as interface
* mark interface as `API`
* declare methods which will be mapped to `API` methods
* mark methods as `WEB API` methods
* specify return types for methods
* specify input parameters for methods
* mark input parameters as part of requests

## Service Contract

Deplare service contract to descripbe servce specification details.

Use `ApiAttribute` to mark an interface that represent a service contract:

```C#
[Api]
public interface IOrderService
{
    //...
}
```
There is ability to specify common path for all API methods releted of base path:

```C#
[Api("orders/v1")]
public interface IOrderService
{
    //...
}
```

## Methods

All contract methods should be asynchronous. 

A сщтекфсе method should be marked by `ApiMethodAttribute`. That attribute defines HTTP method and related method path if defined. Also there are several inherited attributes fro most popular HTTP methods:

```C#
[Api]
public interface IOrderService
{
    [ApiMethod(HttpMethod.Get, RelPath="orders")]
    Task GetOrders1();
    
    [ApiGet(RelPath="orders")]
    Task GetOrders2();
    
    [ApiGet]
    Task GetOrders3();
    
    [ApiPost]
    Task PostOrders();
    
    [ApiPut]
    Task PutOrders();
    
    [ApiHead]
    Task HeadOrders();
    
    [ApiDelete]
    Task DeleteOrders();
}
```

## The Return

A `WEB API` can return both positive or negative response. Positive response is an `HTTP` response with code between `200` and `299` and may contain a response payload. A negative response has another code also may contin payload which describe a status.

There is default behaviour when response has `2xx` code. A method returns the expected result in this case. In other case the `WrongResponseException` will be thrown.

To declare method response payload type that type should be specified as generaic parameter of `Task<>` at return parameter definition as follow:

```C#
[Api]
public interface IOrderService
{
    [ApiGet]
    Task<string> GetString();
    
    [ApiGet]
    Task<DataContract> GetObject();
    
    [ApiGet]
    Task<byte[]> GetObject();
}
```

There are many types are supported:
* primitive: `string`, `bool`, `int`, `uint`, `double`
* object/struct: only if payload is `XML` or `JSON`
* binary: only if payload is `base64` string

## Method Parameters
```C#
throw new NotImplementedException();
```
## Factoring
```C#
throw new NotImplementedException();
```
