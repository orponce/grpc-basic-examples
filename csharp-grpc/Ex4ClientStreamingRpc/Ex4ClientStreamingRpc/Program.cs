using System.Threading.Tasks;
using Grpc.Net.Client;

using Newtonsoft.Json;
using Ex4ClientStreamingRpc;


static List<Feature> ReadDatabase()
{
    List<Feature> feature_list = new List<Feature>();

    // Read the JSON file
    StreamReader r = new StreamReader("route_guide_db.json");
    string json = r.ReadToEnd();
    List<DataFormat> items = JsonConvert.DeserializeObject<List<DataFormat>>(json);

    foreach (var item in items)
    {
        // Convert each element in json to feature
        Point point = new Point { Latitude = item.location.latitude, Longitude = item.location.longitude };
        Feature feature = new Feature { Name = item.name, Location = point };
        feature_list.Add(feature);
    }
    return feature_list;
}


//static async Task GuideRecordRoute(RouteGuide.RouteGuideClient client)
static async Task GuideRecordRoute(RouteGuide.RouteGuideClient client)
{
    List<Feature> feature_list = ReadDatabase();
    using var call = client.RecordRoute();

    Random random = new Random();
    for (int i = 0; i < 10; i++)
    {
        var feature = feature_list[random.Next(0, feature_list.Count())];
        Console.WriteLine($"Visiting point with latitude {feature.Location.Latitude.ToString()} " +
                          $"and longitude {feature.Location.Longitude.ToString()}");
        await call.RequestStream.WriteAsync(feature.Location);
    }
    await call.RequestStream.CompleteAsync();

    var route_summary = await call;
    Console.WriteLine($"finished trip with {route_summary.PointCount.ToString()} points");
    Console.WriteLine($"Passed {route_summary.FeatureCount.ToString()} features");
    Console.WriteLine($"Travelled {route_summary.Distance.ToString()} meters");
    Console.WriteLine($"It took {route_summary.ElapsedTime.ToString()} seconds");

}


var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new RouteGuide.RouteGuideClient(channel);
Console.WriteLine("----------- Client streaming RPC: RecordRoute ---------------");
await GuideRecordRoute(client);


class Location
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}

class DataFormat
{
    public Location? location { get; set; }
    public String? name { get; set; }
}
