## Example: client streaming video to the server and receiving a stream of data

This example shows a client that opens a camera using `OpenCV` and sends a stream of frames (video) to the server. The client also shows the video. The server takes the stream of frames and returns a row of the frame as a 1D double numpy array. The client then shows the first 5 elements of some of the received messages.

This example needs OpenCV. If it is not installed, install it with `conda install opencv` (assuming that conda is being used to manage Python packages).

### Generate from proto

Compile the `.proto` file using either:

	$ python -m grpc_tools.protoc -I=proto --python_out=. --grpc_python_out=. proto/video.proto

or:

     $ python generate_protos.py

### Execute the example

First run the server:

     $ python server.py

Then, from another terminal, run the client:

     $ python client.py

Note that the server needs the client to exit the infinite loop