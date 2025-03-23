using Grpc.Core;
using grpc_server.v1;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;

namespace grpc_server.Services.v1
{
    public class SharedStreamer
    {
        // _instance is a lazily initialized, thread-safe singleton instance of SharedStreamer.
        // The SharedStreamer object will only be created when _instance.Value is accessed for the first time.
        private static readonly Lazy<SharedStreamer> _instance = new(() => new SharedStreamer());
        public static SharedStreamer Instance => _instance.Value;

        public ISubject<GetDataReply> Subject { get; }
        private readonly CancellationTokenSource _cts;
        private readonly IDisposable _subscription;

        private SharedStreamer()
        {
            Subject = new Subject<GetDataReply>();
            _cts = new CancellationTokenSource();

            // Setup background data generation with proper lifecycle management
            _subscription = Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    var random = new Random();
                    Subject.OnNext(new GetDataReply
                    {
                        Number = random.Next(maxValue: 100),
                        Datetime = DateTime.UtcNow.ToString()
                    });
                });
        }

        public void Shutdown()
        {
            _cts.Cancel();
            _subscription.Dispose();
            Subject.OnCompleted();
            (Subject as IDisposable)?.Dispose();
        }
    }

    public class StreamerService : Streamer.StreamerBase
    {
        private readonly ILogger<StreamerService> _logger;
        private readonly SharedStreamer _sharedStreamer;

        public StreamerService(ILogger<StreamerService> logger)
        {
            _logger = logger;
            _sharedStreamer = SharedStreamer.Instance;
        }

        public override async Task GetDataAsync(
            GetDataRequest request,
            IServerStreamWriter<GetDataReply> responseStream,
            ServerCallContext context)
        {
            var requestId = context.GetHttpContext().TraceIdentifier;
            var cancellationToken = context.CancellationToken;

            try
            {
                _logger.LogInformation($"{requestId}: Setting up connection");
                
                // Create subscription for this specific client
                using var subscription = _sharedStreamer.Subject
                    .ObserveOn(System.Reactive.Concurrency.TaskPoolScheduler.Default)
                    .Subscribe(async data =>
                    {
                        _logger.LogInformation($"{requestId}: Writing data: {data.Number}");
                        await responseStream.WriteAsync(data);
                    },
                    onError: ex => _logger.LogError(ex, $"{requestId}: Error occurred"));

                // Register cancellation
                using var _ = cancellationToken.Register(() => subscription.Dispose());

                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"{requestId}: Client disconnected");
            }
        }
    }
}
