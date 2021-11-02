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

## Usage

To use the protocol buffer from Python:

	$ python write_message.py addressbook.data 

This will create a file called `addressbook.data` and you will be prompted to fill the data. After the `addressbook.data` has been created, its content can be read using:

	$ python read_message.py addressbook.data

