
# gRPC test project
In this project I'm doing some tests with this technology.

## Notes
- the way to return an "array of results" is through streaming, e.g. `rpc GetDataAsync(GetDataRequest) returns (stream GetDataReply);`
- the other way is to return an array inside a messege envelop, but this is not too performatic
- `proto files` = kind of interfaces to define the message format.
- `rpc SayHello (HelloRequest) returns (HelloReply);` = service definition
- each message should have a `order identifier`, e.g.:
    ```
    message HelloReply {
        string message = 1;
    }
    ```
- to create new services we should register them individually on Startup `endpoints.MapGrpcService<v1.StreamerService>();`
- [disabling TLS and making gRPC server run over http (insercure) due to OS compatibility (macOS)](https://docs.microsoft.com/pt-br/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1&source=docs#unable-to-start-aspnet-core-grpc-app-on-macos)
- [call insecure (http) gRPC services](https://docs.microsoft.com/pt-br/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1&source=docs#call-insecure-grpc-services-with-net-core-client)

## Nuget Package info
https://github.com/grpc/grpc-dotnet#available-now-on-net-core-30
AspNetCore 3.0 packages
gRPC functionality for .NET Core 3.0 includes:

- Grpc.AspNetCore – An ASP.NET Core framework for hosting gRPC services. gRPC on ASP.NET Core integrates with standard ASP.NET Core features like logging, dependency injection (DI), authentication and authorization.
- Grpc.Net.Client – A gRPC client for .NET Core that builds upon the familiar HttpClient. The client uses new HTTP/2 functionality in .NET Core.
- Grpc.Net.ClientFactory – gRPC client integration with HttpClientFactory. The client factory allows gRPC clients to be centrally configured and injected into your app with DI.

Server
- Grpc.AspNetCore

Client
- Grpc.Net.Client, which contains the .NET Core client.
- Google.Protobuf, which contains protobuf message APIs for C#.
- Grpc.Tools, which contains C# tooling support for protobuf files. The tooling package isn't required at runtime, so the dependency is marked with PrivateAssets="All".


## Other References
- gRPC status codes https://datatracker.ietf.org/doc/html/rfc7540#page-50
- Kestrel gRPC configuration https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/http2?view=aspnetcore-6.0
- Kestrel endpoints configuration https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0#listenoptionsprotocols