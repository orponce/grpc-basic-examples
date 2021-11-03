# Protocol Buffers - C# Example

This directory contains an example that uses Protocol Buffers with C#

## Pre-requisites

Install the compiler for protobuf. In Windows it can be downloaded from the [release page](https://github.com/protocolbuffers/protobuf/releases/tag/v3.19.1). Decompress it, and add the bin folder to the `path` environmental variable.

## Protocol Buffer Message Compilation

The destination folder will be the `proto` folder itself. Use protoc to compile the .proto file for C#

	$ protoc -I=proto --csharp_out=proto proto/addressbook.proto

This generates the `Addressbook.cs` inside the `proto` folder.

## Usage

Compile the C# code from a terminal using:

	$ dotnet build

To run the example, execute:

	$ dotnet run 

This will prompt some options. First create some data (option A) and then load the data (option L). The data will be stored in a file called `addressbook.data`.
