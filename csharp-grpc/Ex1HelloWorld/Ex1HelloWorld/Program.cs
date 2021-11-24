using System.Threading.Tasks;
using Grpc.Net.Client;
using Ex1HelloWorld;

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new Greeter.GreeterClient(channel);

//var response = await client.SayHelloAsync(new HelloRequest { Name = "you" });
var response = client.SayHello(new HelloRequest { Name = "World" });

Console.WriteLine("Greeter client received: " + response.Message);
