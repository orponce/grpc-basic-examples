using System.Threading.Tasks;
using Grpc.Net.Client;

namespace WFormsEx5BidirectionalStreamingRpc
{
    public partial class Form1 : Form
    {
        RouteGuide.RouteGuideClient? client;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStartClient_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                var channel = GrpcChannel.ForAddress("http://localhost:50051");
                client = new RouteGuide.RouteGuideClient(channel);
                textBox1.AppendText("Started client listening at localhost:5005" + Environment.NewLine);
            }
            else
            {
                textBox1.AppendText("Client already started. Cannot restart it!" + Environment.NewLine);
            }
        }

        async private void buttonSendRequest_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                textBox1.AppendText("Client has bot been started. Start the client first!" + Environment.NewLine);
            }
            else
            {
                await GuideRouteChat(client, textBox1);
            }
        }

        static async Task GuideRouteChat(RouteGuide.RouteGuideClient client, TextBox textBox)
        {
            using var call = client.RouteChat();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            textBox.AppendText(Environment.NewLine + "Starting background task to receive messages" +
                               Environment.NewLine);
            var readTask = Task.Run(async () =>
            {
                //await foreach (var response in call.ResponseStream.MoveNext())
                while (await call.ResponseStream.MoveNext(cancellationToken))
                {
                    textBox.AppendText($"Received message {call.ResponseStream.Current.Message} at " +
                                       $"Latitude {call.ResponseStream.Current.Location.Latitude.ToString()}" +
                                       $" and longitude {call.ResponseStream.Current.Location.Longitude.ToString()}" +
                                       Environment.NewLine);
                }
            });

            textBox.AppendText("Starting to send messages..." + Environment.NewLine);
            List<RouteNote> messages = GuideRecordRoute();

            foreach (var msg in messages)
            {
                textBox.AppendText($"Sending {msg.Message} at latitude {msg.Location.Latitude.ToString()}" +
                                   $" and longitude {msg.Location.Longitude.ToString()}" + 
                                   Environment.NewLine);
                await call.RequestStream.WriteAsync(msg);
            }
            await call.RequestStream.CompleteAsync();
            textBox.AppendText(Environment.NewLine + "Disconecting" + Environment.NewLine);
            await readTask;
        }

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
    }
}