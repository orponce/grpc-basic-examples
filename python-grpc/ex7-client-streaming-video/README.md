## Example: client streaming video to the server in Python

This example shows a client that opens a camera using `OpenCV` and sends a stream of frames to the server. The server shows the images it receives as a continuous video.

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