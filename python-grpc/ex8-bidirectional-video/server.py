from concurrent import futures
import time
import base64
import sys

import grpc
import video_pb2
import video_pb2_grpc

import numpy as np
import cv2


class MainServerServicer(video_pb2_grpc.MainServerServicer):
    """Main server servicer: displays the video received as a stream 
    """
    def __init__(self):
        pass

    def getVideoStream(self, request_iterator, context):
        verbose = False
        timer = 0
        for video in request_iterator:
            
            if (verbose):
                print('process time = ' + str(time.process_time() - timer))
                timer = time.process_time()
            
            # Decode stream of bytes using Base64
            b64decoded = base64.b64decode(video.data)
            if (verbose):
                print("base64 decoded size:", sys.getsizeof(b64decoded))
            
            # Convert decoded stream (buffer) to 1D numpy array of uint8
            image1D = np.frombuffer(b64decoded, dtype = np.uint8)
            if (verbose):
                print("image 1D (buffer) size:", sys.getsizeof(image1D))			
            
            # Decode 1d numpy array to opencv image
            image = cv2.imdecode(image1D, cv2.IMREAD_COLOR)
            if (verbose):
                print("image size:", sys.getsizeof(image))
                print("image shape:", image.shape)
            
            # Send a row of the R channel of the image (as doubles)
            xvalues = np.double(image[100,:,0])
            xvalues = xvalues.copy(order='C')
            if (verbose):
                print("Shape of values to send:", xvalues.shape)
            b64data = base64.b64encode(xvalues)
            yield video_pb2.Data1D(data = b64data)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    video_pb2_grpc.add_MainServerServicer_to_server(MainServerServicer(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    print('\n--------------- Server started -------------')
    try:
        while True:
            time.sleep(0)
    except KeyboardInterrupt:
        server.stop(0)


if __name__ == '__main__':
    serve()
