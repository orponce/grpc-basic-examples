from concurrent import futures
import time
import base64
import sys

import grpc
import video_pb2
import video_pb2_grpc

import numpy as np
import cv2


class ShowVideoStream:
    def __init__(self):
        # Image to be displayed using OpenCV
        self.img = None
        # Thread object
        self.thread = futures.ThreadPoolExecutor(max_workers=1)

    def start(self):
        # The showWindow function will be threaded
        self.thread.submit(self.showWindow)

    def set(self, img):
        self.img = img

    def showWindow(self):
        start_flag = True
        # Thread
        while True:
            if self.img is not None:
                if start_flag:
                    print("Showing an image ...")
                    print("Press ESC on the image window to stop showing")
                    start_flag = False
                
                # Show the decoded image
                cv2.imshow('Image at server', self.img)
                k = cv2.waitKey(30)
                # If 'ESC' is pressed, break the loop and end the thread
                if k == 27:
                    print("Image window has been closed. Press CTRL+C to exit.")
                    cv2.destroyAllWindows()
                    break


class MainServerServicer(video_pb2_grpc.MainServerServicer):
    """Main server servicer: displays the video received as a stream 
    """
    def __init__(self):
        pass

    def getStream(self, request_iterator, context):
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
                print("image 1D (buffer) size :", sys.getsizeof(image1D))			
            
            # Decode 1d numpy array to opencv image
            image = cv2.imdecode(image1D, cv2.IMREAD_COLOR)
            if (verbose):
                print("image size : ", sys.getsizeof(image))
            
            # Send the image to a thread to be displayed
            show.set(image)
            if (verbose):
                print("Image has been shown")
            # Success
            yield video_pb2.Reply(reply = 1)


show = ShowVideoStream()


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
    show.start()
    serve()
