using Grpc.Core;
using grpc_server.v1;

namespace grpc_server.Services.v1
{
    public class StreamerService : Streamer.StreamerBase
    {
        /// <summary>
        /// Writing 10 numbers through a stream with an interval of 1 second
        /// on each message.
        /// </summary>
        /// <param name="request">The received object containing the seed</param>
        /// <param name="responseStream">The response stream used to reply the message</param>
        /// <param name="context">Server call context</param>
        /// <returns>Task</returns>
        public async override Task GetDataAsync(
            GetDataRequest request,
            IServerStreamWriter<GetDataReply> responseStream,
            ServerCallContext context)
        {
            var random = new Random(request.Seed);

            for (int i = 0; i < 10; i++)
            {
                await responseStream.WriteAsync(new GetDataReply
                {
                    Number = random.Next(maxValue: 100),
                    Datetime = DateTime.UtcNow.ToString()
                });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
