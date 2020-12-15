## MyLab.ApiClient -Тестирование 

При написании функциональных и интеграционных тестов, для взаимодействия с сервисом через его контракт `API`, используйте класс `ApiClient<>` и провайдер `DelegateHttpClientProvider`. 

Ниже приведены примеры тестов с разным подходом в создании клиентов в зависимости от особенностей взаимодействия:

* можно создать один `api`-клиент на тестовый класс, если в каждом методе, где он используется, будет один вызов сервиса;

```C#
 public class TestServerBehavior : IClassFixture<WebApplicationFactory<Startup>>
 {
     private readonly ApiClient<ITestServer> _client;

     public TestServerBehavior(
         WebApplicationFactory<Startup> webApplicationFactory)
     {
         var clientProvider = new DelegateHttpClientProvider(
             webApplicationFactory.CreateClient);

         _client = new ApiClient<ITestServer>(clientProvider); 
     }

     [Fact]
     public async Task ShouldReturnPayload()
     {
         //Arrange

         //Act 
         var result = await _client.Call(s => s.Get()).GetResult();

         //Assert
         Assert.NotNull(result);
     }

     [Api("test/resource")]
     interface ITestServer
     {
         [Get]
         Task<string> Get();
     }
 }
```

+ можно создать `HttpClient` в тестовом методе, если будут многократные запросы к сервису.

```C#
public class TestServerBehavior : IClassFixture<WebApplicationFactory<Startup>>
 {
     private readonly WebApplicationFactory<Startup> _webApplicationFactory;

     public TestServerBehavior(
         WebApplicationFactory<Startup> webApplicationFactory)
     {
         _webApplicationFactory = webApplicationFactory;
     }

     [Fact]
     public async Task ShouldReturnPayload()
     {
         //Arrange
         var clProvider = new SingleHttpClientProvider(
             _webApplicationFactory.CreateClient());
         var client = new ApiClient<ITestServer>(clProvider);

         //Act
         await client.Call(s => s.Post("foo")).GetResult();
         var result = await client.Call(s => s.Get()).GetResult();

         //Assert
         Assert.Equal("foo", result);
     }

     [Api("test/resource")]
     interface ITestServer
     {
         [Post]
         Task Post([StringContent]string str);
         [Get]
         Task<string> Get();
     }
 }
```

