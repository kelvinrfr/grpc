using Grpc.Core;
using grpc_server.v1;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace grpc_server.Services.v1
{
    public class StreamerService : Streamer.StreamerBase
    {
        private ILogger<StreamerService> _logger;
        private static ISubject<GetDataReply>? _subject;

        public StreamerService(ILogger<StreamerService> logger)
        {
            _logger = logger;
            _subject = new Subject<GetDataReply>();
            Task.Run(BackgroundTask);
        }

        static void BackgroundTask()
        {
            var random = new Random();
            while (true)
            {
                _subject!.OnNext(new GetDataReply
                {
                    Number = random.Next(maxValue: 100),
                    Datetime = DateTime.UtcNow.ToString()
                });
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

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
            var requestId = context.GetHttpContext().TraceIdentifier;
            var cancellationToken = context.CancellationToken;

            try
            {
                _logger.LogInformation($"{requestId}: Setting up connection");
                _subject!.Subscribe(async data =>
                {
                    _logger.LogInformation($"{requestId}: Writing data: {data.Number}");
                    await responseStream.WriteAsync(data);
                }, cancellationToken);

                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"{requestId}: Client disconnected");
            }
        }
    }
}
