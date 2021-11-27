using System.Threading.Tasks;
using Grpc.Net.Client;
using WFormsEx2UnaryRpc;

namespace WFormsEx2UnaryRpc
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

        private void buttonSendRequest_Click(object sender, EventArgs e)
        {
            //var response = client.SayHello(new HelloRequest { Name = "Oscar" });
            Point point = new Point { Latitude = 409146138, Longitude = -746188906 };
            GuideGetOneFeature(point);

            GuideGetOneFeature(new Point { Latitude = 0, Longitude = 0 });

            //string msg = "Greeter client received: " + response.Message;
            //textBox1.AppendText(msg);
            //textBox1.AppendText(Environment.NewLine);
        }

        void GuideGetOneFeature(Point point)
        {
            var response = client.GetFeature(point);

            if (response.Location == null)
            {
                textBox1.AppendText(Environment.NewLine + "Server returned incomplete feature");
                textBox1.AppendText(Environment.NewLine);
                return;
            }

            if (!(response.Name == ""))
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText($"Feature called {response.Name} at ");
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText($"   latitude: {response.Location.Latitude.ToString()} and " +
                                    $"longitude: {response.Location.Longitude.ToString()}");
                textBox1.AppendText(Environment.NewLine);
            }
            else
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Found no feature at " +
                                    $"latitude: {response.Location.Latitude.ToString()} and " +
                                    $"longitude: {response.Location.Longitude.ToString()}");
                textBox1.AppendText(Environment.NewLine);
            }
        }


    }
}