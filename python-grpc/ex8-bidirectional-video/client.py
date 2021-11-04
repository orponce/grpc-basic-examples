from concurrent import futures
import sys
import base64
import time

import grpc
import video_pb2
import video_pb2_grpc

import numpy as np
import cv2


class FrameStreamer:
    """Object to handle the frame streamer
    """
    def __init__(self, path_to_video=0):
        """If a video is to be streamed, set its path. Otherwise, 
        the camera is used by default
        """
        if (path_to_video==0):
            print("No path to video provided ... opening camera")
        self.cap = cv2.VideoCapture(path_to_video)
        self.cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

    def read(self):
        return self.cap.read()

    def close(self):
        self.cap.release()  
        cv2.destroyAllWindows()


def generate_encoded_frame(frame):
    verbose = False
    if (verbose):
        print("Original frame size:", sys.getsizeof(frame))
    
    # Encode the image frame
    ret, buf = cv2.imencode('.jpg', frame)
    if ret != 1:
            return

    # Encode to base64
    b64encoded = base64.b64encode(buf)
    if (verbose):
        print("Base64 encoded size:", sys.getsizeof(b64encoded))

    yield video_pb2.Video(data = b64encoded)


def run(frame_streamer):
    channel = grpc.insecure_channel('localhost:50051')
    stub = video_pb2_grpc.MainServerStub(channel)
    print("-------- Client started streaming --------")

    counter = 0	
    while (frame_streamer.cap.isOpened()):
        try:
            # Open the camera
            ret, frame = frame_streamer.read()
            if ret != 1:
                print("Video ended")
                break
            cv2.imshow('Capture Image', frame)
            k = cv2.waitKey(1)
            if k == 27:
                break

            # Encode the image frame
            encoded_frame = generate_encoded_frame(frame)
            # Send the frame (video stream) to the server and get the response
            responses = stub.getVideoStream(encoded_frame)
            for res in responses:
                # Decode the response and convert to a numpy array (assuming double)
                xval = np.frombuffer(base64.b64decode(res.data), dtype=np.double)
                counter += 1
                if (counter%100) == 0:
                    print(f"Size of received message: {xval.shape}. First 5 values: {xval[:5]}")
        
        except grpc.RpcError as e:
            print(e.details())
            break

if __name__ == '__main__':
    use_camera = False
    if use_camera:
        # Use camera
        frame_streamer = FrameStreamer()
    else:
        # Use a video
        frame_streamer = FrameStreamer("../data/sample.mp4")
    run(frame_streamer)
    frame_streamer.close()