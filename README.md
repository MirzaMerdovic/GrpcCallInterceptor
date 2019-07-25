# GrpcCallInterceptor

Configurable CallInvoker that can be used to intercept and return desired response for the cases where we need a mocked behavior.

## Supporeted Methods

Currently only Unary calls are supported.

## How to use

Depending on how you are setting up your _gRPC_ services using a _DI_ or doing it manually setup will differ. The example below will show how set it up manually, but using DI would be
much better of course.

The _CallInterceptor_ expects to receive the configuration stored in _appsettings.json_ or similar it is injected using [Options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.2)

We will first construct the options object programatically:

```c#
var options = Options.Create(new CallInterceptorOptions
{
    ServiceName = "google.protobuf.CustomerService",
    ResponseType = "GetCustomerByIdResponse",
    JsonResponseContent = JsonConvert.SerializeObject(new GetCustomerByIdResponse { Customer = new Customer { Id = 1, FirstName = "Ed", LastName = "Torsten" } })
});
```

The configuration in _appsettings.json_ would look like this:
```js
{
    "CallInterceptorOptions": {
       "ServiceName": "google.protobuf.CustomerService",
       "ResponseType": "GetCustomerByIdResponse",
       "JsonResponseContent": "{Customer: {\"Id\":1,\"FirstName\":\"Ed\",\"LastName\":\"Torsten\"}}"
   }
}
```

Now that we have the configuration in place, we can go and create an instance of _CallInterceptor_:
```c#
var channel = new Channel("localhost", 5001, ChannelCredentials.Insecure);
var interceptor = new CallInterceptor(options, channel);
```

With the _CallInterceptor_ instance ready, we can use it to create our _gRPC_ client
```c#
var client = new CustomerService.CustomerServiceClient(interceptor);
```

Let's imagine that the _CustomerService_ has 2 methods (both Unary):
* GetCustomerById
* GetCustomerByName

Now if we use our instantiated client and try to consume _GetCustomerById_ method it will be intercepted and the predifined response that we configured will be returned. On the other hand trying to consume _GetCustomerByName_ method will work as if the CallInterceptor doesn not exist.

Note: If you are invoking intercepted methods you don't need to have the _gRPC_ server running on the other side.
