## MyLab.ApiClient - DI инъекция

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
        var resp = await _server.Call(s => s.Echo(msg)).GetDetailed();

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

Прозрачное прокси поддерживает возврат детализации (`CallDetails<>`) методом контракта:

```C#
[Api("echo")]
interface ITestServer
{
    [Get]
    Task<CallDetails<string>> CallEchoAndGetDetails([JsonContent] string msg);
}

//....
    
CallDetails<string> call = await api.CallEchoAndGetDetails("foo");
```

## 