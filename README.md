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
public interface IService
{
    //...
}
```
There is ability to specify common path for all API methods releted of base path:

```C#
[Api("orders/v1")]
public interface IService
{
    //...
}
```

## Methods

All contract methods should be asynchronous. 

A сщтекфсе method should be marked by `ApiMethodAttribute`. That attribute defines HTTP method and related method path if defined. Also there are several inherited attributes fro most popular HTTP methods:

```C#
[Api]
public interface IService
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

### Common

A `WEB API` can return both positive or negative response. Positive response is an `HTTP` response with code between `200` and 300 and may contain a response payload. A negative response has another code also may contain payload which describe a status.

There is default behavior when response has `2xx` code. A method returns the expected result in this case. In other case the `WrongResponseException` will be thrown.

###Payload

To declare method response payload type that type should be specified as generic parameter of `Task<>` at return parameter definition as follow:

```C#
[Api]
public interface IService
{
    //Returns primitve
    [ApiGet]
    Task<string> GetString();
    
    //Returns object
    [ApiGet]
    Task<DataContract> GetObject();
    
    //Returns binary
    [ApiGet]
    Task<byte[]> GetBinary();
}
```

There are many types are supported:
* primitive: `string`, `bool`, `int`, `uint`, `double`
* object/struct: only if payload is `XML` or `JSON`
* binary: any payload content

###The Void

When response has no payload then a simple `Task` return type should be used.

```C#
[Api]
public interface IService
{   
    [ApiGet]
    Task Get();
}
```

This method complete successfully if service response will be `2xx`. The  `WrongResponseException` will be thrown when meet another code.

### CodeResult

Use `CodeResult` result type when all possible api method results are combination of `HTTP` status code and text message as response payload.

```C#
[Api]
public interface IService
{   
    [ApiGet]
    Task<CoreResult> Get();
}
```

This method always complete successfully even the response code is no `2xx`. 

### WebApiCall

To full control an api call invocation the `WebApiCall` result type should be used. In this case the method should be synchronous. It because the method has factory role - it just create `WebApiCall` object with prepared `HttpRequestMessage` inside. An HTTP request will be sent when the `WebApiCall.Invoke()` method will be called.

```C#
[Api]
public interface IService
{   
    [ApiGet]
    WebApiCall Get();
}
```

`WebApiCall` provides access to `HTTP` request and response.  

```C#
HttpRequestMessage request = call.GetRequestClone();
HttpResponseMethod response = call.Response;
```

Specify generic result type to declare api method return type as using simple asynchronous case with `Task<>`. 

```C#
[Api]
public interface IService
{   
    [ApiGet]
    WebApiCall<Something> GetSomething();
}
```

In this case the `WebApiCall<>` also provides result value. 

```C#
Something result = call.Result;
```

## Method Parameters
The input method parameters uses to specify `HTTP` request parameters. There are several place in request can be used:

* part of URL path 
* URL query parameter
* payload
* header

## Factoring
```C#
throw new NotImplementedException();
```
