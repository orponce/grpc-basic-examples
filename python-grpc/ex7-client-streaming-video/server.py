from concurrent import futures
import time
import base64
import sys

import grpc
import video_pb2
import video_pb2_grpc

import numpy as np
import cv2


show_image_in_server = True

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


class FrameRateCounter:
    def __init__(self, frames_to_count = 30):
        self.frames_to_count = frames_to_count    # Count time for every 30 frames
        self.last_frame_time = 0
        self.flag_first_frame = True
        self.time_stored = np.zeros(self.frames_to_count)
        self.num_frame = 0

    def count(self, update_every_frame = False):
        if (self.flag_first_frame):
            self.last_frame_time = time.time()
            self.flag_first_frame = False
        else:
            # Compute the time
            frame_time_now = time.time()
            fps = 1.0 / (frame_time_now - self.last_frame_time)
            self.time_stored[self.num_frame] = fps
            self.num_frame += 1
            if (update_every_frame):
                print("FPS:{:.3}".format(fps))
            if (self.num_frame == self.frames_to_count):
                mean = np.mean(self.time_stored)
                std = np.std(self.time_stored)
                print("fps: {:.2f}, std: {:.2f}".format(mean, std))
                self.num_frame = 0
            self.last_frame_time = frame_time_now
            

class MainServerServicer(video_pb2_grpc.MainServerServicer):
    """Main server servicer: displays the video received as a stream 
    """
    def __init__(self):
        self.fpscounter = FrameRateCounter()
        self.fcounter = 0
        self.language = 1

    def ConfigureClient(self, request, context):
        self.language = request.language
        return video_pb2.Reply(data = self.language)


    def SendStream(self, request_iterator, context):
        verbose = False
        timer = 0
        for video in request_iterator:
            
            if (verbose):
                print('process time = ' + str(time.process_time() - timer))
                timer = time.process_time()
            
            if (self.language == 0):
                # Decode stream of bytes using Base64 (only if client is Python)
                b64decoded = base64.b64decode(video.data)
            else:
                b64decoded = video.data
            self.fcounter += 1
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
            
            # Compute and show time (fps)
            if (True):
                self.fpscounter.count()
                
            # Send the image to a thread to be displayed
            if (show_image_in_server):
                show.set(image)
            if (verbose):
                print("Image has been shown")
            # Success
            yield video_pb2.Reply(data = self.fcounter)


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
    if (show_image_in_server):
        show.start()
    serve()
