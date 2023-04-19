from PyQt5 import uic
from PyQt5.QtMultimedia import QCameraInfo
from PyQt5.QtWidgets import QApplication, QMainWindow
from PyQt5.QtCore import QThread, pyqtSignal, QTimer, QDateTime, Qt
from PyQt5.QtGui import QImage, QCursor, QPixmap
import cv2, time, sys
import numpy as np
import face_recognition
from tensorflow import keras
import psutil
import socket
import json
import winsound


def getCPU():
    return psutil.cpu_percent(interval=1)

def getRAM():
    return psutil.virtual_memory()

eye_model = keras.models.load_model('yuiyongmodel.h5')

class ThreadClass(QThread):
    ImageUpdate = pyqtSignal(np.ndarray)
    EyeCrimination = pyqtSignal(int)

    def __init__(self, parent=None):
        super(ThreadClass, self).__init__(parent)
        self.parent = parent

    def run(self):
        Capture = cv2.VideoCapture(0, cv2.CAP_DSHOW)
        Capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
        Capture.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        self.ThreadActive = True

        skip_frames = 15
        frame_count = 0

        while self.ThreadActive:
            ret, frame = Capture.read()
            flip_frame = cv2.flip(src=frame, flipCode=1)

            if ret != True:
                break;
            if frame_count % skip_frames == 0:
                image_for_prediction = self.eye_cropper(frame)
                try:
                    image_for_prediction = image_for_prediction / 255.0
                except Exception as error:
                    print(error)
                    continue

                # 모델에서 예측 가져오기.
                prediction = eye_model.predict(image_for_prediction)
                if prediction < 0.5:
                    counter = 0
                    status = 'Open'
                    self.EyeCrimination.emit(counter)

                else:
                    counter = counter + 1
                    status = 'Closed'
                    self.EyeCrimination.emit(counter)

                    # 카운터가 3보다 크면 사용자가 잠들었다는 경고를 재생하고 표시한다.
                    if counter > 3 and not self.parent.sent_signal:
                        # 소리 재생
                        winsound.PlaySound('detect.wav', winsound.SND_FILENAME | winsound.SND_ASYNC)

                        self.EyeCrimination.emit(counter)
                        counter = 0
                        self.parent.alarmbtn.setEnabled(True)

                    elif counter > 3:
                        counter = 0
                print(counter)

            self.ImageUpdate.emit(flip_frame)
            frame_count += 1
            time.sleep(0.033)  # 30fps

    def stop(self):
        self.ThreadActive = False
        self.quit()

    # 웹캠 프레임이 함수에 입력된다.
    def eye_cropper(self, frame):

        # 얼굴 특징 좌표에 대한 변수 생성
        facial_features_list = face_recognition.face_landmarks(frame)
        # print('1번째 테스트',facial_features_list)

        # 눈 좌표에 대한 자리 표시 목록을 만듦.
        try:
            eye = facial_features_list[0]['left_eye']
            # print('2번째 테스트',eye)
        except:
            try:
                eye = facial_features_list[0]['right_eye']
                # print('3번째 테스트',eye)
            except:
                # 안면 인식으로 찾지 못한 경우
                return

        # 눈의 최대 x및 y 좌표 설정
        x_max = max([coordinate[0] for coordinate in eye])
        # print(x_max)
        x_min = min([coordinate[0] for coordinate in eye])
        # print(x_min)
        y_max = max([coordinate[1] for coordinate in eye])
        y_min = min([coordinate[1] for coordinate in eye])

        # x와 y 좌표의 범위 설정
        x_range = x_max - x_min
        y_range = y_max - y_min

        # 전체 눈이 캡처되었는지 확인하기 위해
        # 사각형의 좌표를 계산한다.
        # 더 넓은 범위로 x축에 50% 추가
        # 그런 다음 더 작은 범위를 완충된 더 큰 범위에 일치시키는 작업을 수행해야 한다.
        if x_range > y_range:
            right = round(.5 * x_range) + x_max
            left = x_min - round(.5 * x_range)
            bottom = round((((right - left) - y_range)) / 2) + y_max
            top = y_min - round((((right - left) - y_range)) / 2)
        else:
            bottom = round(.5 * y_range) + y_max
            top = y_min - round(.5 * y_range)
            right = round((((bottom - top) - x_range)) / 2) + x_max
            left = x_min - round((((bottom - top) - x_range)) / 2)

        # 위에서 결정한 좌표에 따라 이미지 자르기
        cropped = frame[top:(bottom + 1), left:(right + 1)]

        # 이미지 크기 조정
        cropped = cv2.resize(cropped, (80, 80))
        image_for_prediction = cropped.reshape(-1, 80, 80, 3)

        return image_for_prediction


class boardInfo(QThread):
    cpu = pyqtSignal(float)
    ram = pyqtSignal(tuple)

    def run(self):
        self.ThreadActive = True
        while self.ThreadActive:
            cpu = getCPU()
            ram = getRAM()

            self.cpu.emit(cpu)
            self.ram.emit(ram)

    def stop(self):
        self.ThreadActive = False
        self.quit()


class server_connect(QThread):
    checkconn = pyqtSignal(int)

    def __init__(self, parent=None):
        super(server_connect, self).__init__(parent)
        self.parent = parent

    def run(self):
        self.ThreadActive = True
        try:
            checkconn = 1
            self.parent.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.parent.client_socket.connect((self.parent.ip, int(self.parent.port)))
            self.parent.client_socket.settimeout(None)
            self.checkconn.emit(checkconn)
        except ConnectionRefusedError as e:
            checkconn = 2
            self.checkconn.emit(checkconn)
        except TimeoutError as e:
            checkconn = 3
            self.checkconn.emit(checkconn)

    def stop(self):
        self.ThreadActive = False
        self.quit()


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.ui = uic.loadUi("untitled.ui", self)
        self.setWindowFlags(Qt.WindowStaysOnTopHint | Qt.FramelessWindowHint)
        self.serverofflabel.setStyleSheet("background-color: rgb(255, 0, 0); border-radius:30px")

        self.ip = "10.10.21.103"
        self.port = "9000"
        self.myipaddress = socket.gethostbyname(socket.gethostname())

        self.serverconnectionbtn.clicked.connect(self.request)
        self.closeconnectionbtn.setEnabled(False)
        self.closeconnectionbtn.clicked.connect(self.closeconn)
        self.closebtn.clicked.connect(self.Close_software)

        self.online_cam = QCameraInfo.availableCameras()
        self.camlistcb.addItems([c.description() for c in self.online_cam])
        self.startcambtn.clicked.connect(self.StartWebCam)
        self.stopcambtn.clicked.connect(self.StopWebCam)
        self.stopcambtn.setEnabled(False)

        self.resource_usage = boardInfo()
        self.resource_usage.start()
        self.resource_usage.cpu.connect(self.getCPU_usage)
        self.resource_usage.ram.connect(self.getRAM_usage)

        self.lcd_timer = QTimer()
        self.lcd_timer.timeout.connect(self.clock)
        self.lcd_timer.start()

        self.sent_signal = False
        self.alarmbtn.setEnabled(False)
        self.alarmbtn.clicked.connect(self.alarm_stop)

    def __del__(self):
        self.client_socket.close()

    def mousePressEvent(self, event):
        if event.button() == Qt.LeftButton:
            self.m_flag = True
            self.m_Position = event.globalPos() - self.pos()
            event.accept()
            self.setCursor(QCursor(Qt.OpenHandCursor))

    def mouseMoveEvent(self, QMouseEvent):
        if Qt.LeftButton and self.m_flag:
            self.move(QMouseEvent.globalPos() - self.m_Position)
            QMouseEvent.accept()

    def mouseReleaseEvent(self, QMouseEvent):
        self.m_flag = False
        self.setCursor(QCursor(Qt.ArrowCursor))

    def closeconn(self):
        message = {'signal': f'연결종료'}
        message_json = json.dumps(message)
        self.client_socket.sendall(message_json.encode('utf-8'))
        self.client_socket.close()
        self.serverofflabel.setText("Off")
        self.serverofflabel.setStyleSheet("background-color: rgb(255, 0, 0); border-radius:30px")
        self.serverconnectionbtn.setEnabled(True)
        self.closeconnectionbtn.setEnabled(False)
        self.statusTextEdit.append("서버와 연결이 종료되었습니다.")

    def alarm_stop(self):
        self.sent_signal = False
        message = {'signal': '졸음종료'}
        message_json = json.dumps(message)
        self.client_socket.sendall(message_json.encode('utf-8'))
        self.openeyelabel.setPixmap(QPixmap('openeye.png'))
        winsound.PlaySound(None, winsound.SND_PURGE)
        self.alarmbtn.setEnabled(False)

    def connectconn(self, check):
        if check == 1:
            message = {'signal': f'서버연결'}
            message_json = json.dumps(message)
            self.client_socket.sendall(message_json.encode('utf-8'))
            self.statusTextEdit.append("서버와 연결 되었습니다.")

            self.server_conn.stop()
            self.serverconnectionbtn.setEnabled(False)
            self.closeconnectionbtn.setEnabled(True)
            self.serverofflabel.setText("Con")
            self.serverofflabel.setStyleSheet("background-color: rgb(85, 255, 0); border-radius:30px")

        elif check == 2:
            self.statusTextEdit.append("서버에서 연결을 거부했습니다.")
            self.server_conn.stop()
            self.serverconnectionbtn.setEnabled(True)

        elif check == 3:
            self.statusTextEdit.append("서버와 연결되지 않았습니다")
            self.server_conn.stop()
            self.serverconnectionbtn.setEnabled(True)

    def request(self):
        print(self.myipaddress)
        self.serverofflabel.setText("Ready")
        self.server_conn = server_connect(self)
        self.server_conn.start()
        self.server_conn.checkconn.connect(self.connectconn)
        self.serverconnectionbtn.setEnabled(False)

    def getCPU_usage(self, cpu):
        self.CPUlabel.setText(str(cpu) + " %")
        if cpu > 15: self.CPUlabel.setStyleSheet("color: rgb(23, 63, 95);")
        if cpu > 25: self.CPUlabel.setStyleSheet("color: rgb(32, 99, 155);")
        if cpu > 45: self.CPUlabel.setStyleSheet("color: rgb(60, 174, 163);")
        if cpu > 65: self.CPUlabel.setStyleSheet("color: rgb(246, 213, 92);")
        if cpu > 85: self.CPUlabel.setStyleSheet("color: rgb(237, 85, 59);")

    def getRAM_usage(self, ram):
        self.RAMlabel.setText(str(ram[2]) + " %")
        if ram[2] > 15: self.RAMlabel.setStyleSheet("color: rgb(23, 63, 95);")
        if ram[2] > 25: self.RAMlabel.setStyleSheet("color: rgb(32, 99, 155);")
        if ram[2] > 45: self.RAMlabel.setStyleSheet("color: rgb(60, 174, 163);")
        if ram[2] > 65: self.RAMlabel.setStyleSheet("color: rgb(246, 213, 92);")
        if ram[2] > 85: self.RAMlabel.setStyleSheet("color: rgb(237, 85, 59);")

    def opencv_emit(self, Image):

        original = self.cvt_opencv_qt(Image)
        self.CopyImage = Image[20:2000,
                         20:2000]

        self.mainwebcam.setPixmap(original)
        self.mainwebcam.setScaledContents(True)

    def cvt_opencv_qt(self, Image):

        rgb_img = cv2.cvtColor(src=Image, code=cv2.COLOR_BGR2RGB)
        h, w, ch = rgb_img.shape
        bytes_per_line = ch * w
        cvt2QtFormat = QImage(rgb_img.data, w, h, bytes_per_line, QImage.Format_RGB888)
        pixmap = QPixmap.fromImage(cvt2QtFormat)

        return pixmap

    def clock(self):
        self.DateTime = QDateTime.currentDateTime()
        self.timelcd.display(self.DateTime.toString('hh:mm:ss'))

    def get_drowsiness(self, counter):
        if counter == 0:
            self.openeyelabel.setStyleSheet("background-color: rgb(85, 255, 0); border-radius:30px")
            self.openeyelabel.setPixmap(QPixmap('openeye.png'))

        if counter > 0 and counter < 4:
            self.openeyelabel.setStyleSheet("background-color: rgb(255, 0, 0); border-radius:30px")
            self.openeyelabel.setPixmap(QPixmap('closeeye.png'))

        if counter > 3 and not self.sent_signal:
            message = {'signal': f'졸음시작'}
            message_json = json.dumps(message)
            self.client_socket.sendall(message_json.encode('utf-8'))
            self.drowsiness_count += 1
            self.drowsinesslcd.display(self.drowsiness_count)
            self.sent_signal = True
            self.openeyelabel.setStyleSheet("background-color: rgb(255, 0, 0); border-radius:30px")
            self.openeyelabel.setPixmap(QPixmap('sleeping.png'))

        if self.drowsiness_count > 3:
            self.drowsiness_count = 0
            message = {'signal': f'구역알림'}
            message_json = json.dumps(message)
            self.client_socket.sendall(message_json.encode('utf-8'))
            pass

    def StartWebCam(self):

        try:
            self.statusTextEdit.append(
                f"{self.DateTime.toString('yy년 MMM월 d일 hh:mm:ss')}: Start Webcam ({self.camlistcb.currentText()})")
            self.startcambtn.setEnabled(False)
            self.stopcambtn.setEnabled(True)

            self.Worker_Opencv = ThreadClass(self)  # self를 넣었음.
            self.Worker_Opencv.ImageUpdate.connect(self.opencv_emit)
            self.Worker_Opencv.EyeCrimination.connect(self.get_drowsiness)
            self.Worker_Opencv.start()
            self.drowsiness_count = 0
            self.drowsinesslcd.display(self.drowsiness_count)
            self.camlistcb.setEnabled(False)

        except Exception as error:
            pass

    def StopWebCam(self):
        try:
            self.statusTextEdit.append(
                f"{self.DateTime.toString('yy년 MMM월 d일 hh:mm:ss')}: Stop Webcam ({self.camlistcb.currentText()})")
            self.startcambtn.setEnabled(True)
            self.stopcambtn.setEnabled(False)
            self.Worker_Opencv.stop()
            self.camlistcb.setEnabled(True)

        except Exception as error:
            pass

    def Close_software(self):
        if not self.serverconnectionbtn.isEnabled():
            self.closeconn()
        if not self.startcambtn.isEnabled():
            self.Worker_Opencv.stop()
        self.resource_usage.stop()
        self.lcd_timer.stop()
        sys.exit(app.exec_())


if __name__ == "__main__":
    app = QApplication([])
    window = MainWindow()
    window.show()
    app.exec_()