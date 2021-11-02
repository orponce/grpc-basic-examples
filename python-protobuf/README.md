# Protocol Buffers - Python Example

This directory contains an example that uses Protocol Buffers with Python

## Pre-requisites

Install the compiler for protobuf

	$ sudo apt install protobuf-compiler 

Install the google client for Python (assuming that conda is being used as a package manager)

	$ conda install -c conda-forge google-api-python-client

## Compilation

The destination folder will be `build` (create the folder: `mkdir build`). Then use protoc to compile the .proto file

	$ protoc -I=proto --python_out=build proto/addressbook.proto

This generates the `addressbook_pb2.py` inside the `build` folder.



