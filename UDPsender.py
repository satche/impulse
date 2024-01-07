import socket
import random
import time
import pyautogui

UDP_IP = "127.0.0.1"
UDP_PORT = 5000

# File to read data from
# DATA = "data-30sec.txt"
DATA = "data-30sec.txt"
refreshRate = 0.02

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
# Output format: (x, y, z, x_theta, y_theta, z_theta)
def aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta):
	return str(x) + "," + str(y) + "," + str(z) + "," + str(x_theta) + "," + str(y_theta) + "," + str(z_theta)


# Read data from file
if DATA != "":

	# If a file with data exist, let's use it
	with open(DATA) as file:

		for line in file:

			# Split the line into strings (it's seperated by spaces)
			strings = line.split('\x00')

			# Split each string into values (seperated by _)
			for string in strings:

				values = string.split('_')
					
				# Assign values
				timestamp, x, y, z, x_theta, y_theta, z_theta = values

				# Send values
				data = aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta)
				sock.sendto(data.encode(), (UDP_IP, UDP_PORT))

				print(data)
			
				# Sleep for 0.1 seconds
				time.sleep(refreshRate)

# If no file with data exist, let's use mouse position
else:
	while True:
		# Use mouse position as data instead
		x = pyautogui.position()[0]
		z = pyautogui.position()[1]
		data = aggregateValuesToString(x, y, z, x_theta, y_theta, z_theta)
		sock.sendto(data.encode(), (UDP_IP, UDP_PORT))
		print("Sent: " + data)
		time.sleep(refreshRate)