## Basic Example

Compile the `.proto` file

	$ python -m grpc_tools.protoc -I=protos --python_out=. --grpc_python_out=. protos/helloworld.proto
	
Then execute the example as:

     $ python server.py

From another terminal:

     $ python client.py
