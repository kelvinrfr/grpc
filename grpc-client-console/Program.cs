using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpc_server;
using grpc_server.v1;

namespace grpc_client_console
{
    class Program
    {
        const string address = "http://localhost:5000";
        static readonly GrpcChannel channel = GrpcChannel.ForAddress(address);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting server warm up...");
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Allowing gRPC run over insecure channels
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Console.WriteLine($"Creating gRPC clients over channel '{address}'...");
            await CallGreeterService();

            await CallStreamerService();
        }

        private static async Task CallStreamerService()
        {
            Console.WriteLine("Creating streamer client..");
            var streamerClient = new Streamer.StreamerClient(channel);

            Console.WriteLine("Calling service...");
            using var streamingReply = streamerClient.GetDataAsync(new GetDataRequest
            {
                Seed = 21
            });

            while (await streamingReply.ResponseStream.MoveNext())
            {
                var reply = streamingReply.ResponseStream.Current;
                Console.WriteLine($"Message received {reply.Datetime} = {reply.Number}");
            }
            Console.WriteLine("All messages received");
        }

        private static async Task CallGreeterService()
        {
            Console.WriteLine("Creating greeter client..");
            var greaterClient = new Greeter.GreeterClient(channel);

            Console.WriteLine("Calling client...");
            var reply = await greaterClient.SayHelloAsync(new HelloRequest
            {
                Name = "Kelvin"
            });

            Console.WriteLine($"Client reply: {reply.Message}");
        }
    }
}
