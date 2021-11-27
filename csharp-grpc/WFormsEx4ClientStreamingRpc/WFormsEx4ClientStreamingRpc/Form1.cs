using System.Threading.Tasks;
using Grpc.Net.Client;
using Newtonsoft.Json;

namespace WFormsEx4ClientStreamingRpc
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
            string filePath = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filePath = openFileDialog1.FileName;
                    textBox1.AppendText("Opening: " + filePath + Environment.NewLine);
                }
                catch (System.Security.SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }

            if (filePath == "")
            {
                textBox1.AppendText("Could not open file" + Environment.NewLine);
            }
            else
            {
                await GuideRecordRoute(client, textBox1, filePath);
            }
        }

        static async Task GuideRecordRoute(RouteGuide.RouteGuideClient client, TextBox textBox, string filePath)
        {
            List<Feature> feature_list = ReadDatabase();
            using var call = client.RecordRoute();

            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var feature = feature_list[random.Next(0, feature_list.Count())];
                textBox.AppendText($"Visiting point with latitude {feature.Location.Latitude.ToString()} " +
                                   $"and longitude {feature.Location.Longitude.ToString()}");
                textBox.AppendText(Environment.NewLine);
                await call.RequestStream.WriteAsync(feature.Location);
            }
            await call.RequestStream.CompleteAsync();

            var route_summary = await call;
            textBox.AppendText($"finished trip with {route_summary.PointCount.ToString()} points" + Environment.NewLine);
            textBox.AppendText($"Passed {route_summary.FeatureCount.ToString()} features" + Environment.NewLine);
            textBox.AppendText($"Travelled {route_summary.Distance.ToString()} meters" + Environment.NewLine);
            textBox.AppendText($"It took {route_summary.ElapsedTime.ToString()} seconds" + Environment.NewLine);

        }

        static List<Feature> ReadDatabase()
        {
            List<Feature> feature_list = new List<Feature>();

            // Read the JSON file
            StreamReader r = new StreamReader("route_guide_db.json");
            string json = r.ReadToEnd();
            List<DataFormat>? items = JsonConvert.DeserializeObject<List<DataFormat>>(json);

            foreach (var item in items)
            {
                // Convert each element in json to feature
                Point point = new Point { Latitude = item.location.latitude, Longitude = item.location.longitude };
                Feature feature = new Feature { Name = item.name, Location = point };
                feature_list.Add(feature);
            }
            return feature_list;
        }

    }
}