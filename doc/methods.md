## MyLab.ApiClient - Методы

### Асинхронные методы

Все методы контракта `API` должны быть асинхронными, т.е. возвращать `Task` или `Task<>`.

### Разметка

Метод контракта должен быть помечен атрибутом `ApiMethodAttribute` или его наследником. Здесь определяется относительный путь и `HTTP`-метод. Также у `ApiMethodAttribute`  есть ряд наследников для основных случаев:

```C#
[Api]
public interface IService
{
    [ApiMethod("orders", HttpMethod.Get)]
    Task GetOrders1();
    
    [Get("orders")]
    Task GetOrders2();
    
    [Get]
    Task GetOrders3();
    
    [Post]
    Task PostOrders();
    
    [Put]
    Task PutOrders();
    
    [Head]
    Task HeadOrders();
    
    [Delete]
    Task DeleteOrders();
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
    Task Get([Path]string id);
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
    Task Get([Query]string id);
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
    Task Get([Header("X-Identifier")]string id);
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
    Task Create([StringContent] int orderId);
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
    Task Create([JsonContent] Order order);
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
    Task Create([XmlContent] Order order);
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
    Task Create([FormContent] Order order);
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
    Task Create([BinContent] byte[] orderData);
}
```

Аргумент должен быть типа `byte[]`.