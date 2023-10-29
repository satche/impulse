import socket
import random
import time

UDP_IP = "127.0.0.1"  # Use the IP address of your Unity application
UDP_PORT = 5000  # Use the same port number as in your Unity script

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Values
positionDelta_min = -10
positionDelta_max = 10
positionMin = -100
positionMax = 100
x = 0.0
y = 0.0
z = 0.0

angleDelta_min = -2
angleDelta_max = 2
angleMin = -180
angleMax = 180
x_theta = 0.0
y_theta = 0.0
z_theta = 0.0

# Aggregate values into a string
def aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta):
	 return str(x) + "," + str(y) + "," + str(z) + "," + str(x_theta) + "," + str(y_theta) + "," + str(z_theta)

# Random value between two numbers
def randomValue(x,y):
	return random.uniform(x,y)

# Modify values by a random delta
while True:
	if 
	x = x + positionDelta_max
	y = y + positionDelta_max
	z = z + positionDelta_max
	x_theta = x_theta + randomValue(angleDelta_min,angleDelta_max)
	y_theta = y_theta + randomValue(angleDelta_min,angleDelta_max)
	z_theta = z_theta + randomValue(angleDelta_min,angleDelta_max)

	# Send a sample UDP packet (replace with your data format)
	data = aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta)
	sock.sendto(data.encode(), (UDP_IP, UDP_PORT))
	print("Sent: " + data)
