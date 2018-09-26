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
```C#
throw new NotImplementedException();
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
