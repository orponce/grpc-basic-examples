using System.Threading.Tasks;
using Grpc.Net.Client;
//using Ex1HelloWorld;

namespace WFormsEx1HelloWorld
{
    public partial class Form1 : Form
    {
        Greeter.GreeterClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStartClient_Click(object sender, EventArgs e)
        {
            if (client== null)
            {
                var channel = GrpcChannel.ForAddress("http://localhost:50051");
                client = new Greeter.GreeterClient(channel);

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
            var response = client.SayHello(new HelloRequest { Name = "Oscar" });
            string msg = "Greeter client received: " + response.Message;
            textBox1.AppendText(msg);
            textBox1.AppendText(Environment.NewLine);
        }
    }
}