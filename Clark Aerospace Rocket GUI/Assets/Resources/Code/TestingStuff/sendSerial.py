import serial
import serial.tools
import time
import struct
import binascii

# python3 -m serial.tools.list_ports

ser = serial.Serial('/dev/cu.usbserial-AI0691QA')
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

    #amount = struct.pack('d', float(input()))
    #ser.write(bytearray.fromhex("535452" + binascii.hexlify(amount) + "454E44"))
    stuff = ser.read(4)
    print(stuff)
    #ser.write(binascii.unhexlify("53545200000000000000000000000000000000302500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001454e44"))
    time.sleep(1)
    #ser.write(bytearray.fromhex(input()))
