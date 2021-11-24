using System.Threading.Tasks;
using Grpc.Net.Client;
using Ex2UnaryRpc;


void GuideGetOneFeature(RouteGuide.RouteGuideClient client, Point point)
{
    var response = client.GetFeature(point);

    if (response.Location == null)
    {
        Console.WriteLine("Server returned incomplete feature");
        return;
    }

    if (!(response.Name == ""))
    {
        Console.WriteLine($"Feature called {response.Name} at " + 
                          $"latitude: {response.Location.Latitude.ToString()} and " +
                          $"longitude: {response.Location.Longitude.ToString()}");
    }
    else
    {
        Console.WriteLine("Found no feature at " +
                          $"latitude: {response.Location.Latitude.ToString()} and " +
                          $"longitude: {response.Location.Longitude.ToString()}");
    }
}


void GuideGetFeature(RouteGuide.RouteGuideClient client)
{
    Point point = new Point { Latitude = 409146138, Longitude = -746188906 };
    GuideGetOneFeature(client, point);

    GuideGetOneFeature(client, new Point { Latitude = 0, Longitude = 0 });
}


var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new RouteGuide.RouteGuideClient(channel);
Console.WriteLine("----------- Unary RPC: Get Feature ---------------");
GuideGetFeature(client);
