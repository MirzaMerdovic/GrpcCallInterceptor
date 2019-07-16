using Grpc.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleOut.GrpcCallInterecpter
{
    public class CallInterceptor : DefaultCallInvoker
    {
        private readonly CallInterceptorOptions _options;

        public CallInterceptor(IOptionsMonitor<CallInterceptorOptions> options, Channel channel) : base(channel)
        {
            _options = options.CurrentValue;
        }


        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            if (_options.ServiceName.Equals(method.ServiceName, StringComparison.InvariantCultureIgnoreCase) &&
                options.Headers.Where(x => x.Key.Equals("tag", StringComparison.InvariantCultureIgnoreCase)).Any(x => x.Value.Equals(_options.Tag, StringComparison.InvariantCultureIgnoreCase)))
            {
                var response = JsonConvert.DeserializeObject<TResponse>(_options.JsonResponseContent);

                return new AsyncClientStreamingCall<TRequest, TResponse>(
                    null,
                    Task.FromResult(response),
                    Task.FromResult(options.Headers),
                    () => Status.DefaultSuccess,
                    () => options.Headers,
                    () => { });
            }

            return base.AsyncClientStreamingCall(method, host, options);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            throw new NotImplementedException();
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
