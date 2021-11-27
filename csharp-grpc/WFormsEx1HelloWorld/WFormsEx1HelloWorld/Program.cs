using Grpc.Net.Client;

namespace WFormsEx1HelloWorld
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            //var channel = GrpcChannel.ForAddress("http://localhost:50051");
            //var client = new Greeter.GreeterClient(channel);
            //Application.Run(new Form1(client));
            Application.Run(new Form1());
        }
    }
}