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
```C#
throw new NotImplementedException();
```
## Method Parameters
```C#
throw new NotImplementedException();
```
## Factoring
```C#
throw new NotImplementedException();
```
