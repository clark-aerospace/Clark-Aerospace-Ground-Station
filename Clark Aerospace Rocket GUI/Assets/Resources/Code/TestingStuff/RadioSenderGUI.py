import serial, struct, time, threading
from serial.tools import list_ports
import sys
from PyQt5 import QtCore, QtGui, QtWidgets#, QtMacExtras
Qt = QtCore.Qt
import binascii

"""
/dev/cu.usbserial-AK06RGGT
/dev/cu.usbserial-AL02UU3Z
/dev/cu.usbserial-AI0691QA

"""
class Window(QtWidgets.QMainWindow):
    def __init__(self, parent=None):
        super(Window, self).__init__(parent)



        self.serialConnected = False
        self.closeThread = False
        self.simThread = threading.Thread(target=self.DoRocketSim)
        try:
            self.ser = serial.Serial()
            self.ser.baudrate = 57600
            self.ser.bytesize = serial.EIGHTBITS
            self.ser.parity = serial.PARITY_NONE
            self.ser.stopbits = serial.STOPBITS_ONE
            self.ser.rtscts = True
            self.ser.dsrdtr = True
            self.ser.write_timeout = 0
            self.serialConnected = True
        except serial.serialutil.SerialException as e:
            print(e)

        self.posInfo = QtWidgets.QGroupBox("Position")
        self.posInfo_lat = QtWidgets.QDoubleSpinBox()
        self.posInfo_lat.setRange(-9999,9999)
        self.posInfo_long = QtWidgets.QDoubleSpinBox()
        self.posInfo_long.setRange(-9999,9999)
        self.posInfo_alt = QtWidgets.QDoubleSpinBox()
        self.posInfo_alt.setRange(-9999,99999)

        self.posInfo_para = QtWidgets.QCheckBox()


        self.posInfoLayout = QtWidgets.QFormLayout()
        self.posInfoLayout.addRow(QtWidgets.QLabel("Latitude"), self.posInfo_lat)
        self.posInfoLayout.addRow(QtWidgets.QLabel("Longitude"), self.posInfo_long)
        self.posInfoLayout.addRow(QtWidgets.QLabel("Altitude"), self.posInfo_alt)
        self.posInfoLayout.addRow(QtWidgets.QLabel("Parachute"), self.posInfo_para)
        self.posInfo.setLayout(self.posInfoLayout)
        self.posInfoLayout.setAlignment(Qt.AlignLeft)


        self.accelInfo = QtWidgets.QGroupBox("Acceleration")
        self.accelInfo_x = QtWidgets.QDoubleSpinBox()
        self.accelInfo_x.setRange(-9999,9999)
        self.accelInfo_y = QtWidgets.QDoubleSpinBox()
        self.accelInfo_y.setRange(-9999,9999)
        self.accelInfo_z = QtWidgets.QDoubleSpinBox()
        self.accelInfo_z.setRange(-9999,9999)

        self.accelInfoLayout = QtWidgets.QFormLayout()
        self.accelInfoLayout.addRow(QtWidgets.QLabel("X Acceleration"), self.accelInfo_x)
        self.accelInfoLayout.addRow(QtWidgets.QLabel("Y Acceleration"), self.accelInfo_y)
        self.accelInfoLayout.addRow(QtWidgets.QLabel("Z Acceleration"), self.accelInfo_z)
        self.accelInfo.setLayout(self.accelInfoLayout)
        self.accelInfoLayout.setAlignment(Qt.AlignLeft)

        self.rotInfo = QtWidgets.QGroupBox("Rotation")
        self.rotInfo_x = QtWidgets.QDoubleSpinBox()
        self.rotInfo_x.setRange(-9999,9999)
        self.rotInfo_y = QtWidgets.QDoubleSpinBox()
        self.rotInfo_y.setRange(-9999,9999)
        self.rotInfo_z = QtWidgets.QDoubleSpinBox()
        self.rotInfo_z.setRange(-9999,9999)
        self.rotInfo_w = QtWidgets.QDoubleSpinBox()
        self.rotInfo_w.setRange(-9999,9999)

        self.rotInfoLayout = QtWidgets.QFormLayout()
        self.rotInfoLayout.addRow(QtWidgets.QLabel("X Rotation"), self.rotInfo_x)
        self.rotInfoLayout.addRow(QtWidgets.QLabel("Y Rotation"), self.rotInfo_y)
        self.rotInfoLayout.addRow(QtWidgets.QLabel("Z Rotation"), self.rotInfo_z)
        self.rotInfoLayout.addRow(QtWidgets.QLabel("Z Rotation"), self.rotInfo_w)
        self.rotInfo.setLayout(self.rotInfoLayout)
        self.rotInfoLayout.setAlignment(Qt.AlignLeft)


        self.airbrakesInfoGroup = QtWidgets.QGroupBox("Airbrakes")
        self.airbrakesInfoLayout = QtWidgets.QFormLayout()
        self.airbrakesAngle = QtWidgets.QDoubleSpinBox()
        self.airbrakesInfoLayout.addRow(QtWidgets.QLabel("Angle"), self.airbrakesAngle)
        self.airbrakesInfoGroup.setLayout(self.airbrakesInfoLayout)

        self.tempInfo = QtWidgets.QGroupBox("Temperature")
        self.tempInfo_payload = QtWidgets.QDoubleSpinBox()
        self.tempInfo_payload.setRange(-9999,9999)
        self.tempInfo_avionics = QtWidgets.QDoubleSpinBox()
        self.tempInfo_avionics.setRange(-9999,9999)
        self.tempInfo_airbrakesCurr = QtWidgets.QDoubleSpinBox()
        self.tempInfo_airbrakesCurr.setRange(-9999,9999)
        self.tempInfo_ambient = QtWidgets.QDoubleSpinBox()
        self.tempInfo_ambient.setRange(-9999,9999)


        self.battery = QtWidgets.QGroupBox("Battery")
        self.batteryLayout = QtWidgets.QFormLayout()

        self.batteryPayload = QtWidgets.QSpinBox()
        self.batteryPayload.setRange(0, 100)

        self.batteryAvionics = QtWidgets.QSpinBox()
        self.batteryAvionics.setRange(0, 100)

        self.batteryAirbrakes = QtWidgets.QSpinBox()
        self.batteryAirbrakes.setRange(0, 100)


        self.batteryLayout.addRow(QtWidgets.QLabel("Payload"), self.batteryPayload)
        self.batteryLayout.addRow(QtWidgets.QLabel("Avionics"), self.batteryAvionics)
        self.batteryLayout.addRow(QtWidgets.QLabel("Airbrakes"), self.batteryAirbrakes)

        self.battery.setLayout(self.batteryLayout)


        self.tempInfoLayout = QtWidgets.QFormLayout()
        self.tempInfoLayout.addRow(QtWidgets.QLabel("Payload"), self.tempInfo_payload)
        self.tempInfoLayout.addRow(QtWidgets.QLabel("Avionics"), self.tempInfo_avionics)
        #self.tempInfoLayout.addRow(QtWidgets.QLabel("Airbrakes Curr"), self.tempInfo_airbrakesCurr)
        self.tempInfoLayout.addRow(QtWidgets.QLabel("Ambient"), self.tempInfo_ambient)
        self.tempInfo.setLayout(self.tempInfoLayout)
        self.tempInfoLayout.setAlignment(Qt.AlignLeft)

        self.listOfSerials = QtWidgets.QComboBox()
        for i in list_ports.comports():
            print(i.device)
            self.listOfSerials.addItem(i.device)

        self.listOfSerials.currentTextChanged.connect(self.UpdateSerialPort)

        

        self.autoButton = QtWidgets.QPushButton("Simulate rocket")
        self.autoButton.clicked.connect(self.SimRocket)

        self.sendButton = QtWidgets.QPushButton("Send data")
        self.sendButton.clicked.connect(self.SendData)
        self.sendButton.setEnabled(self.serialConnected)


        self.layout = QtWidgets.QGridLayout()
        self.layout.addWidget(self.posInfo, 0, 0)
        self.layout.addWidget(self.accelInfo, 1, 0)
        self.layout.addWidget(self.rotInfo, 0, 1)
        self.layout.addWidget(self.airbrakesInfoGroup, 1, 1)
        self.layout.addWidget(self.tempInfo, 2, 0)
        self.layout.addWidget(self.battery, 2, 1)
        self.layout.addWidget(self.listOfSerials, 3, 0)
        self.layout.addWidget(self.autoButton, 4, 0)
        self.layout.addWidget(self.sendButton, 5, 0)


        self.mainWidget = QtWidgets.QWidget()
        self.setCentralWidget(self.mainWidget)
        self.mainWidget.setLayout(self.layout)

        

    def UpdateSerialPort(self, text):
        self.ser.port = text

    def SimRocket(self):
        if (self.simThread.isAlive()):
            self.autoButton.setEnabled(True)
            self.posInfo_alt.setReadOnly(False)
            self.posInfo_para.setEnabled(True)
            self.closeThread = True

        else:
            self.simThread = threading.Thread(target=self.DoRocketSim)
            self.closeThread = False
            self.simThread.start()



    def DoRocketSim(self):
        height = 0
        ctime = 0
        self.posInfo_alt.setReadOnly(True)
        #self.autoButton.setEnabled(False)
        self.posInfo_para.setEnabled(False)

        while (ctime < 1100):
            if (self.closeThread):
                break
            height = self.CalcRocketAlt(ctime)
            self.posInfo_alt.setValue(float(height))

            if (ctime > 32):
                self.posInfo_para.setChecked(True)

            ctime += 3
            if (self.serialConnected): self.SendData()
            time.sleep(0.75)

        #self.autoButton.setEnabled(True)
        self.posInfo_alt.setReadOnly(False)
        self.posInfo_para.setEnabled(True)


    def CalcRocketAlt(self, time_pt):
        base_eq = ((time_pt - 32)**2)
        if (time_pt < 32):
            return self.minzero(-10 * base_eq + 10000)
        else:
            return self.minzero(-0.01 * base_eq + 10000)

    def minzero(self, val):
        if (val > 0): return val
        else: return 0

    def SendData(self):
        print("Send data")

        """
        Data format
        Start of transmission "STR"

        Latitude - double
        Longitude - double
        Altitude - integer

        Acceleration X - double
        Acceleration Y - double
        Acceleration Z - double

        Rotation W - double
        Rotation X - double
        Rotation Y - double
        Rotation Z - double

        Airbrakes Angle - double

        Payload Temperature - double
        Avionics Temperature - double
        Ambient Temperature - double

        Drogue Deployed - Boolean

        Payload Battery - double
        Avionics Battery - double
        Airbrakes Battery - double

        Time epoch sent - long?

        End marker "END"

        """
        dataStruct = struct.Struct('< 3s 2d d 3d 4d d 3d ? 3d L 3s')

        timeEpoch = int(time.time())

        info = (
            b"STR",
            self.posInfo_lat.value(),
            self.posInfo_long.value(),
            self.posInfo_alt.value(),

            self.accelInfo_x.value(),
            self.accelInfo_y.value(),
            self.accelInfo_z.value(),

            self.rotInfo_x.value(),
            self.rotInfo_y.value(),
            self.rotInfo_z.value(),
            self.rotInfo_w.value(),

            self.airbrakesAngle.value(),

            self.tempInfo_payload.value(),
            self.tempInfo_avionics.value(),
            self.tempInfo_ambient.value(),

            self.posInfo_para.isChecked(),

            float(self.batteryPayload.value()) / 100.0, # Payload battery
            float(self.batteryAvionics.value()) / 100.0, # AV battery
            float(self.batteryAirbrakes.value()) / 100.0, # Airbrakes battery

            timeEpoch,
            b"END"
        )

        # lilInfo = (
        #     #b"STRC",
        #     self.posInfo_alt.value(),
        #     self.posInfo_para.isChecked(),
        #     timeEpoch
        #     #b"EDRC"
        # )
        packedData = dataStruct.pack(*info)

        if not self.ser.is_open:
            self.ser.open()
        self.ser.write(packedData)

        print("Length of packed data is " + str(len(packedData)))
        print(binascii.hexlify(packedData))

    def closeEvent(self, event):
        self.closeThread = True
        #return super().closeEvent(self, event)


        

        


if __name__ == '__main__':
    #global app
    app = QtWidgets.QApplication(sys.argv)

    window = Window()
    window.show()
    sys.exit(app.exec_())