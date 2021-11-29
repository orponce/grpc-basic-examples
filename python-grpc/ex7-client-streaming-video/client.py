from concurrent import futures
import sys
import base64

import grpc
import video_pb2
import video_pb2_grpc

import cv2


show_image_in_client = True
cap = cv2.VideoCapture(0)
if (show_image_in_client):
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)


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


def run():
    channel = grpc.insecure_channel('localhost:50051')
    stub = video_pb2_grpc.MainServerStub(channel)

    # Send information about the client language 
    options = video_pb2.Options(language=0)
    stub.ConfigureClient(options)

    print("-------- Client started streaming --------")
    while True:
        try:
            # Open the camera
            ret, frame = cap.read()
            if ret != 1:
                continue
            if (show_image_in_client):
                cv2.imshow('Capture Image', frame)
                k = cv2.waitKey(1)
                if k == 27:
                    break

            # Send the stream to the server
            encoded_frame = generate_encoded_frame(frame)
            responses = stub.SendStream(encoded_frame)
            for res in responses:
                if (res.data % 100) == 0:
                    print(f"Sent and received {res.data} frames")
        
        except grpc.RpcError as e:
            print(e.details())
            break


if __name__ == '__main__':
    run()


if (show_image_in_client):
    cap.release()
    cv2.destroyAllWindows()