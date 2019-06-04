import serial
import serial.tools
import time


# python3 -m serial.tools.list_ports

ser = serial.Serial('/dev/cu.usbserial-AK06RGGT')
ser.baudrate = 57600
ser.bytesize = serial.EIGHTBITS
ser.parity = serial.PARITY_NONE
ser.stopbits = serial.STOPBITS_ONE
ser.rtscts = True
ser.dsrdtr = True
ser.write_timeout = 100
print(ser.name)
while True:
    #ser.write(b"Clark Aerospace")
    ser.write(bytes(input(), 'utf-8'))
