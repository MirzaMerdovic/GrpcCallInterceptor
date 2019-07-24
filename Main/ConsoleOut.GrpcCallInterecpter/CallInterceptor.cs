using Grpc.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleOut.GrpcCallInterecpter
{
    internal static class Extensions
    {
        public static bool AreEqual(this string value, string compareValue)
        {
            return value.Equals(compareValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class CallInterceptor : DefaultCallInvoker
    {
        private readonly CallInterceptorOptions _options;

        public CallInterceptor(IOptions<CallInterceptorOptions> options, Channel channel) : base(channel)
        {
            _options = options.Value;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            if (ShouldIntercept(_options, method))
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
            return base.AsyncDuplexStreamingCall(method, host, options);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            return base.AsyncServerStreamingCall(method, host, options, request);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            if (ShouldIntercept(_options, method))
            {
                var response = JsonConvert.DeserializeObject<TResponse>(_options.JsonResponseContent);

                return new AsyncUnaryCall<TResponse>(
                    Task.FromResult(response),
                    Task.FromResult(options.Headers),
                    () => Status.DefaultSuccess,
                    () => options.Headers,
                    () => { });
            }

            return base.AsyncUnaryCall(method, host, options, request);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            if (ShouldIntercept(_options, method))
            {
                var response = JsonConvert.DeserializeObject<TResponse>(_options.JsonResponseContent);

                return response;
            }

            return base.BlockingUnaryCall(method, host, options, request);
        }

        private static bool ShouldIntercept<TRequest, TResponse>(CallInterceptorOptions interceptorOptions, Method<TRequest, TResponse> method)
        {
            if (!interceptorOptions.ServiceName.AreEqual(method.ServiceName))
                return false;

            if (!interceptorOptions.ResponseType.AreEqual(typeof(TResponse).Name))
                return false;

            return true;
        }
    }
}