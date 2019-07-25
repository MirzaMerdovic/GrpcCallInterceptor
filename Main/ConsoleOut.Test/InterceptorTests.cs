using ConsoleOut.Example.App;
using ConsoleOut.Example.Contracts;
using ConsoleOut.GrpcCallInterecpter;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleOut.Test
{
    public class InterecptorTests
    {
        [Fact]
        public async Task ShouldIntercept()
        {
            var channel = new Channel("localhost", 5001, ChannelCredentials.Insecure);

            var options = Options.Create(new CallInterceptorOptions
            {
                ServiceName = "google.protobuf.CustomerService",
                ResponseType = "GetCustomerByIdResponse",
                JsonResponseContent = JsonConvert.SerializeObject(new GetCustomerByIdResponse { Customer = new Customer { Id = 1, FirstName = "Ed", LastName = "Torsten" } })
            });

            var interceptor = new CallInterceptor(options, channel);

            var client = new CustomerService.CustomerServiceClient(interceptor);
            GetCustomerByIdResponse response = await client.GetCustomerByIdAsync(new GetCustomerByIdRequest());

            Assert.Equal(1, response.Customer.Id);
            Assert.Equal("Ed", response.Customer.FirstName);
            Assert.Equal("Torsten", response.Customer.LastName);
        }
        // Status(StatusCode=Unavailable, Detail="failed to connect to all addresses")
        [Fact]
        public async Task ShouldFailToIntercept()
        {
            var channel = new Channel("localhost", 5001, ChannelCredentials.Insecure);

            var options = Options.Create(new CallInterceptorOptions
            {
                ServiceName = "google.protobuf.CustomerService",
                ResponseType = "GetCustomerByIdResponse",
                JsonResponseContent = JsonConvert.SerializeObject(new AddCustomersResponse { Id = 1 })
            });

            var interceptor = new CallInterceptor(options, channel);

            var client = new CustomerService.CustomerServiceClient(interceptor);

            var exception = await Assert.ThrowsAsync<RpcException>(() => client.AddCustomers(new Metadata()).ResponseAsync);
            Assert.Equal(StatusCode.Unavailable, exception.StatusCode);
        }
    }
}
