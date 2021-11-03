import logging
import random

import grpc
import route_guide_pb2
import route_guide_pb2_grpc
import resources


def guide_list_features(stub):
    rectangle = route_guide_pb2.Rectangle(
            lo=route_guide_pb2.Point(latitude=400000000, longitude=-750000000),
            hi=route_guide_pb2.Point(latitude=420000000, longitude=-730000000))
    print("Looking for features between 40, -75 and 42, -73")

    features = stub.ListFeatures(rectangle)

    for feature in features:
        print("Feature called %s at %s" % (feature.name, feature.location))


def run():
    with grpc.insecure_channel('localhost:50051') as channel:
        stub = route_guide_pb2_grpc.RouteGuideStub(channel)
        print("------ Server streaming RPC: ListFeatures ------")
        guide_list_features(stub)
        

if __name__ == '__main__':
    logging.basicConfig()
    run()
