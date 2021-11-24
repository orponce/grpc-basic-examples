using System.Threading.Tasks;
using Grpc.Net.Client;
using Ex5BidirectionalStreamingRpc;


static RouteNote MakeRouteNote(String message, int latitude, int longitude)
{
    RouteNote note = new RouteNote 
    { 
        Message = message,
        Location = new Point { Latitude = latitude, Longitude = longitude }
    };
    return note;
}

static List<RouteNote> GuideRecordRoute()
{
    List<RouteNote> messages = new List<RouteNote>();
    messages.Add(MakeRouteNote("First message", 0, 0));
    messages.Add(MakeRouteNote("Second message", 0, 1));
    messages.Add(MakeRouteNote("Third message", 1, 0));
    messages.Add(MakeRouteNote("Fourth message", 0, 0));
    messages.Add(MakeRouteNote("Fifth message", 1, 0));
    return messages;
}

static async Task GuideRouteChat(RouteGuide.RouteGuideClient client)
{
    using var call = client.RouteChat();

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken cancellationToken = cancellationTokenSource.Token;

    Console.WriteLine("Starting background task to receive messages");
    var readTask = Task.Run(async () =>
    {
        //await foreach (var response in call.ResponseStream.MoveNext())
        while (await call.ResponseStream.MoveNext(cancellationToken))
        {
            Console.WriteLine($"Received message {call.ResponseStream.Current.Message} at " +
                              $"Latitude {call.ResponseStream.Current.Location.Latitude.ToString()}" +
                              $" and longitude {call.ResponseStream.Current.Location.Longitude.ToString()}");
        }
    });

    Console.WriteLine("Starting to send messages");
    List<RouteNote> messages = GuideRecordRoute();

    foreach (var msg in messages)
    {
        Console.WriteLine($"Sending {msg.Message} at latitude {msg.Location.Latitude.ToString()}" +
                          $" and longitude {msg.Location.Longitude.ToString()}");
        await call.RequestStream.WriteAsync(msg);
    }
    Console.WriteLine("Disconecting");
    await call.RequestStream.CompleteAsync();
    await readTask;
}


var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new RouteGuide.RouteGuideClient(channel);
Console.WriteLine("----------- Bidirectional streaming RPC: RouteChat ---------------");
await GuideRouteChat(client);
