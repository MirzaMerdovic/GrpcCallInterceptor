using ConsoleOut.Example.Contracts;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace ConsoleOut.Example.App
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomerService.BindService(new CustomerServiceImpl());

            var server = new Server();
            server.Ports.Add("localhost", 5001, ServerCredentials.Insecure);
            server.Start();

            Console.ReadKey();
        }
    }

    public class CustomerServiceImpl : CustomerService.CustomerServiceBase
    {
        public override Task<AddCustomersResponse> AddCustomers(IAsyncStreamReader<Customer> requestStream, ServerCallContext context)
        {
            return Task.FromResult(new AddCustomersResponse());
        }

        public override Task CheckCustomer(IAsyncStreamReader<CheckCustomerRequest> requestStream, IServerStreamWriter<CheckCustomerResponse> responseStream, ServerCallContext context)
        {
            return Task.CompletedTask;
        }

        public override Task<GetCustomerByIdResponse> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetCustomerByIdResponse());
        }

        public override Task ListCustomers(CustomerSearch request, IServerStreamWriter<Customer> responseStream, ServerCallContext context)
        {
            return Task.CompletedTask;
        }
    }
}
