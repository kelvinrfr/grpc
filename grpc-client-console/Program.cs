using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using grpc_server; 

namespace grpc_client_console
{
    class Program
    {
        const string address = "http://localhost:5000";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting server warm up...");
            await Task.Delay(TimeSpan.FromSeconds(5));

            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            await GreeterService();
        }

        private static async Task GreeterService()
        {
            Console.WriteLine($"Creating gRPC client over channel '{address}'...");
            var channel = GrpcChannel.ForAddress(address);
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
