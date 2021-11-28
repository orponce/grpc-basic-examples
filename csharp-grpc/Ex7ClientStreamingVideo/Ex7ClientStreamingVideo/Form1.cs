using System.ComponentModel;

using System.Threading.Tasks;
using Grpc.Net.Client;

using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace Ex7ClientStreamingVideo
{
    public partial class Form1 : Form
    {
        // Create a video device
        private readonly VideoCapture capture;
        // gRPC client
        MainServer.MainServerClient? client;
        // Flag to stop the camera
        VideoDevice camDevice = new VideoDevice();

        public Form1()
        {
            InitializeComponent();
            capture = new VideoCapture();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            capture.Open(0, VideoCaptureAPIs.ANY);
            if (!capture.IsOpened())
            {
                // If no video device, close the form
                Close();
                return;
            }
            // Set the size of pictureBox to the size of the image
            pictureBox1.Size = new System.Drawing.Size(capture.FrameWidth, capture.FrameHeight);
            // backgroundWorker1.RunWorkerAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // The worker must support cancelation
            //backgroundWorker1.CancelAsync();
            
            // Release the resources
            capture.Dispose();
        }

        private void buttonStartClient_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                var channel = GrpcChannel.ForAddress("http://localhost:50051");
                client = new MainServer.MainServerClient(channel); 
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
                if (camDevice.isRunning)
                {
                    textBox1.AppendText("Cannot send another request while camera is running!" + Environment.NewLine);
                }
                else
                {
                    camDevice.stop = false;
                    camDevice.isRunning = true;
                    await CameraTask(client, textBox1, capture, pictureBox1, camDevice);
                }
            }
        }

        static async Task CameraTask(MainServer.MainServerClient client, TextBox textBox, VideoCapture capture,
                                     PictureBox pictureBox, VideoDevice camDevice)
        {
            // Request to the server
            using var call = client.getStream();

            // Print messages?
            bool Verbose = false;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Task to receive the stream from the server
            textBox.AppendText(Environment.NewLine + "Starting background task to receive messages" +
                               Environment.NewLine);
            var readTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext(cancellationToken))
                {
                    textBox.AppendText($"Received {call.ResponseStream.Current.Reply_.ToString()} " +
                                        " frames" + Environment.NewLine);
                }
            });

            // Send stream to the server
            textBox.AppendText("Starting to send messages..." + Environment.NewLine);
            //for (int i = 0; i < 100; i++)
            while(!camDevice.stop)
            {
                //var frameMat = capture.RetrieveMat();
                var frameMat = new Mat();
                capture.Read(frameMat);
                if (Verbose)
                {
                    textBox.AppendText(Environment.NewLine);
                    textBox.AppendText($"Frame size: {frameMat.Width.ToString()} x {frameMat.Height.ToString()}" +
                                       Environment.NewLine);
                    textBox.AppendText("Frame length:" + frameMat.Total().ToString() + Environment.NewLine);
                }

                var frameBitmap = BitmapConverter.ToBitmap(frameMat);
                pictureBox.Image?.Dispose();
                pictureBox.Image = frameBitmap;

                byte[] buf;
                bool ret = Cv2.ImEncode(".jpg", frameMat, out buf);
                if (Verbose)
                {
                    textBox.AppendText("Size of encoded image: " + buf.Length.ToString() + Environment.NewLine);
                    textBox.AppendText("Size of encoded image: " + buf.Count().ToString() + Environment.NewLine);
                }

                // Google.Protobuf.ByteString.CopyFrom is reverting Base64 format, so it is not being converted
                if (false)
                {
                    var base64buf = Convert.ToBase64String(buf);
                    textBox.AppendText("Size of base64 encoded: " + base64buf.Length.ToString() + Environment.NewLine);
                    // var EncodedFrame = Google.Protobuf.ByteString.FromBase64(base64buf);
                }

                var EncodedFrame = Google.Protobuf.ByteString.CopyFrom(buf);
                if (Verbose)
                {
                    textBox.AppendText("First encoded" + EncodedFrame.Count() + Environment.NewLine);
                    textBox.AppendText("Size of encoded: " + EncodedFrame.Length.ToString() + Environment.NewLine);
                }
                
                Video msg = new Video { Data = EncodedFrame };
                Cv2.WaitKey(10);

                await call.RequestStream.WriteAsync(msg);
            }
            await call.RequestStream.CompleteAsync();
            textBox.AppendText(Environment.NewLine + "Disconecting" + Environment.NewLine);
            await readTask;
        }
        
        private void buttonStopCamera_Click(object sender, EventArgs e)
        {
            camDevice.stop = true;
            camDevice.isRunning = false;
        }


        // ------------------------------------------------------------------------------------
        // Background workers are not being used
        // ------------------------------------------------------------------------------------

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Executes operation on a separate thread
            var bgWorker = (BackgroundWorker)sender;

            while (!bgWorker.CancellationPending)
            {
                // Get the frame from the device
                using (var frameMat = capture.RetrieveMat())
                {
                    // Draw a rectangle
                    //int x = 10; int y = 10; int width = 100; int height = 50;
                    //Rect rect = new Rect(x, y, width, height);
                    //Cv2.Rectangle(frameMat, rect, Scalar.Green, 2);

                    // Convert the frame to a bitmap
                    var frameBitmap = BitmapConverter.ToBitmap(frameMat);
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = frameBitmap;
                }
                Thread.Sleep(10);
            }
        }

    }
}