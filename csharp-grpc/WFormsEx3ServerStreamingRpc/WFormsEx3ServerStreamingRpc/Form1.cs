using System.Threading.Tasks;
using Grpc.Net.Client;

namespace WFormsEx3ServerStreamingRpc
{
    public partial class Form1 : Form
    {
        RouteGuide.RouteGuideClient client;

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

                string msg = "Started client listening at localhost:50051\n";
                textBox1.AppendText(msg);
                textBox1.AppendText(Environment.NewLine);
            }
            else
            {
                string msg = "Client already started. Cannot restart it!\n";
                textBox1.AppendText(msg);
                textBox1.AppendText(Environment.NewLine);
            }
        }

        async private void buttonSendRequest_Click(object sender, EventArgs e)
        {
            await GuideListFeatures(client, textBox1);
        }

        static async Task GuideListFeatures(RouteGuide.RouteGuideClient client, TextBox textBox)
        {
            Rectangle rectangle = new Rectangle
            {
                Lo = new Point { Longitude = -750000000, Latitude = 400000000 },
                Hi = new Point { Longitude = -730000000, Latitude = 420000000 }
            };

            textBox.AppendText("Looking for features between 40, -75 and 42, -73");

            using var feature = client.ListFeatures(rectangle);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (await feature.ResponseStream.MoveNext(cancellationToken))
            {
                textBox.AppendText(Environment.NewLine);
                textBox.AppendText($"Feature called {feature.ResponseStream.Current.Name} " + Environment.NewLine);
                textBox.AppendText($"   at latitude {feature.ResponseStream.Current.Location.Latitude} " +
                                   $"and longitude {feature.ResponseStream.Current.Location.Longitude}");
                textBox.AppendText(Environment.NewLine);
            }
        }
    }
}