import socket
import random
import time
import pyautogui

UDP_IP = "127.0.0.1"
UDP_PORT = 5000

# Initialize spatial values
x = 0
y = 0
z = 0
x_theta = 0
y_theta = 0
z_theta = 0

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Aggregate values into a string
# (x, y, z, x_theta, y_theta, z_theta)
def aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta):
	return str(x) + "," + str(y) + "," + str(z) + "," + str(x_theta) + "," + str(y_theta) + "," + str(z_theta)

# Send values according to mouth position
while True:
	x = pyautogui.position()[0]
	y = pyautogui.position()[1]
	data = aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta)
	sock.sendto(data.encode(), (UDP_IP, UDP_PORT))
	print("Sent: " + data)