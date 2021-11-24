using System.Threading.Tasks;
using Grpc.Net.Client;
using Ex3ServerStreamingRpc;


static async Task GuideListFeatures(RouteGuide.RouteGuideClient client)
{
    Rectangle rectangle = new Rectangle
    {
        Lo = new Point { Longitude = -750000000, Latitude = 400000000 },
        Hi = new Point { Longitude = -730000000, Latitude = 420000000 }
    };

    Console.WriteLine("Looking for features between 40, -75 and 42, -73");

    using var feature = client.ListFeatures(rectangle);

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken cancellationToken = cancellationTokenSource.Token;

    while (await feature.ResponseStream.MoveNext(cancellationToken))
    {
        Console.WriteLine($"Feature called {feature.ResponseStream.Current.Name} " +
                          $"at latitude {feature.ResponseStream.Current.Location.Latitude} " +
                          $"and longitude {feature.ResponseStream.Current.Location.Longitude}");
    }
}


var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new RouteGuide.RouteGuideClient(channel);
Console.WriteLine("----------- Server streaming RPC: ListFeatures ---------------");
await GuideListFeatures(client);