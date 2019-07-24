using ConsoleOut.Example.Contracts;
using ConsoleOut.GrpcCallInterecpter;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ConsoleOut.Example.AppClient
{
    class Program
    {
        static async Task Main(string[] args)
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
        }
    }
}