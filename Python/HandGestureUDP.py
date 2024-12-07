import socket
from cvzone.HandTrackingModule import HandDetector
import cv2

# Camera Setup
cap = cv2.VideoCapture(0)

# Hand Detector
detectorHand = HandDetector(detectionCon=0.8, maxHands=1)

# Socket setup (UDP)
client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ('localhost', 6060)

previous_gesture = None

while True:
    # Get image frame
    success, img = cap.read()
    img = cv2.flip(img, 1)

    # Find the hand and its landmarks
    hands, _ = detectorHand.findHands(img, draw=False)  # with no draw

    if hands:
        hand = hands[0]
        fingers = detectorHand.fingersUp(hand)  # List of which fingers are up
        handType = hand["type"]  # "Left" or "Right"

        if fingers == [0, 0, 0, 0, 0]:  # Nắm tay
            if previous_gesture != "FIST":
                client_socket.sendto(b'SPACE_DOWN', server_address)  # Gửi tín hiệu nhấn space
                previous_gesture = "FIST"
        elif fingers == [1, 1, 1, 1, 1]:  # Mở bàn tay
            if previous_gesture != "OPEN":
                client_socket.sendto(b'SPACE_UP', server_address)  # Gửi tín hiệu thả space
                previous_gesture = "OPEN"
        elif fingers == [0, 1, 0, 0, 0]:  # Chỉ giơ ngón trỏ
            if previous_gesture != "INDEX_FINGER":
                client_socket.sendto(b'PLAY', server_address)  # Gửi tín hiệu PLAY
                previous_gesture = "INDEX_FINGER"
        elif fingers == [1, 0, 0, 0, 0]:
            if handType == "Right" and previous_gesture != "RIGHT_THUMB":
                client_socket.sendto(b'RIGHT', server_address)
                previous_gesture = "RIGHT_THUMB"
            elif handType == "Left" and previous_gesture != "LEFT_THUMB":
                client_socket.sendto(b'LEFT', server_address)
                previous_gesture = "LEFT_THUMB"
        elif fingers == [0, 0, 0, 0, 1]:
            if handType == "Right" and previous_gesture != "RIGHT_PINKY":
                client_socket.sendto(b'LEFT', server_address)
                previous_gesture = "RIGHT_PINKY"
            elif handType == "Left" and previous_gesture != "LEFT_PINKY":
                client_socket.sendto(b'RIGHT', server_address)
                previous_gesture = "LEFT_PINKY"

    key = cv2.waitKey(1)
    if key == ord('q'):
        break

cap.release()
client_socket.close()