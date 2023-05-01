# The-15th-anti-drowsiness-project
# As soon as the overall contents of cnn model learning are organized, the source code should be added and attached.
# By detecting drowsiness with a CNN model, an alarm system for preventing workers' drowsiness is built.
# GUI Correction
![image](https://user-images.githubusercontent.com/115389450/233039468-f96a9c00-2597-4d19-8bba-30e671f6ee65.png)

connection | disconnection
:-: | :-:
<video src='https://user-images.githubusercontent.com/115389450/232941075-2737c127-4fc7-4275-8f46-1aa8534b1b46.mp4' width=180/> | <video src='https://user-images.githubusercontent.com/115389450/232941086-867953c9-1a1a-4e74-b1e5-2c4fd6e67c72.mp4' width=180/></video>
drowsiness alarm | client test
<video src='https://user-images.githubusercontent.com/115389450/232821423-18700401-f7c3-4cb8-b4da-557ec1f0854e.mp4' width=180/> | <video src='https://user-images.githubusercontent.com/115389450/232821643-81aae768-3344-4669-81e1-2cad7c0a503f.mp4' width=180/></video>
finally | graph
<video src='https://user-images.githubusercontent.com/115389450/232819011-21ec83e3-0839-4906-899d-137c08b5412b.mp4' width=180/></video> | <video src='https://user-images.githubusercontent.com/115389450/232821503-eccbaecc-d755-437a-bf41-7a0ef8d6d401.mp4' width=180/></video>

![15thone](https://user-images.githubusercontent.com/115389450/232813192-936b8e8a-18c6-4e66-9a42-5fabc4826c00.png)
![15thtwo](https://user-images.githubusercontent.com/115389450/232814940-f81d4efe-001b-4eef-9b0a-d4338cbaca07.png)
![15ththree](https://user-images.githubusercontent.com/115389450/232816212-b82aef1e-a18b-423c-82ef-543c2e9adc3b.png)
![15thfour](https://user-images.githubusercontent.com/115389450/232816830-9e8c1087-9910-4edc-91b7-b54aaf6d6453.png)

![image](https://user-images.githubusercontent.com/115389450/235433767-22e19261-d690-4a6a-be92-7175785e937b.png)
![image](https://user-images.githubusercontent.com/115389450/235433816-f0a41aa0-9d7a-41fe-96d7-810d4897429f.png)
![image](https://user-images.githubusercontent.com/115389450/235433844-0cb6205b-b1f8-4c5a-94ac-1d5fae3ee31e.png)
![image](https://user-images.githubusercontent.com/115389450/235433866-a94611d7-155a-4d1e-bb57-863a868a5125.png)
```
def eye_cropper(folders):
    
    count = 0
    
    for folder in os.listdir(folders):
        for file in os.listdir(folders + '/' + folder):
            
            image = face_recognition.load_image_file(folders + '/' + folder + '/' + file)
            face_landmarks_list = face_recognition.face_landmarks(image)
            
            eyes = []
            
            try:
                eyes.append(face_landmarks_list[0]['left_eye'])
                eyes.append(face_landmarks_list[0]['right_eye'])
            
            except:
                continue
            
            for eye in eyes:
                x_max = max([coordinate[0] for coordinate in eye])
                x_min = min([coordinate[0] for coordinate in eye])
                y_max = max([coordinate[1] for coordinate in eye])
                y_min = min([coordinate[1] for coordinate in eye])
                
                x_range = x_max - x_min
                y_range = y_max - y_min
                
                if x_range > y_range:
                    right = round(.5*x_range) + x_max
                    left = x_min - round(.5*x_range)
                    bottom = round(((right-left) - y_range))/2 + y_max
                    top = y_min - round(((right - left) - y_range))/2
                else:
                    bottom = round(.5*y_range) + y_max
                    top = y_min - round(.5*y_range)
                    right = round(((bottom-top) - x_range))/2 + x_max
                    left = x_min - round(((bottom-top) - x_range))/2
                
                im = Image.open(folders + '/' + folder + '/' + file)
                im = im.crop((left, top, right, bottom))
                
                im = im.resize((80, 80))
                
                im.save(f'C:/Users/Kiot/dataset_eye_save/{count}.png')
                
                count += 1
                
                if count % 200 ==0:
                    print(count)
                    
eye_cropper("C:/Users/Kiot/data_open_eye")
```
```
def eye_cropper(folders):
    
    count = 0
    
    for folder in os.listdir(folders):
            
            image = face_recognition.load_image_file(folders + '/' + folder)
            face_landmarks_list = face_recognition.face_landmarks(image)
            
            eyes = []
            
            try:
                eyes.append(face_landmarks_list[0]['left_eye'])
                eyes.append(face_landmarks_list[0]['right_eye'])
            
            except:
                continue
            
            for eye in eyes:
                x_max = max([coordinate[0] for coordinate in eye])
                x_min = min([coordinate[0] for coordinate in eye])
                y_max = max([coordinate[1] for coordinate in eye])
                y_min = min([coordinate[1] for coordinate in eye])
                
                x_range = x_max - x_min
                y_range = y_max - y_min
                
                if x_range > y_range:
                    right = round(.5*x_range) + x_max
                    left = x_min - round(.5*x_range)
                    bottom = round(((right-left) - y_range))/2 + y_max
                    top = y_min - round(((right - left) - y_range))/2
                else:
                    bottom = round(.5*y_range) + y_max
                    top = y_min - round(.5*y_range)
                    right = round(((bottom-top) - x_range))/2 + x_max
                    left = x_min - round(((bottom-top) - x_range))/2
                
                im = Image.open(folders + '/' + folder)
                im = im.crop((left, top, right, bottom))
                
                im = im.resize((80, 80))
                
                im.save(f'C:/Users/Kiot/dataset_close_save/{count}.png')
                
                count += 1
                
                if count % 200 ==0:
                    print(count)
                    
eye_cropper("C:/Users/Kiot/data_close_eye")
```
```
import cv2
import tensorflow as tf
from tensorflow import keras
from sklearn.model_selection import train_test_split
from tensorflow.keras.wrappers.scikit_learn import KerasClassifier
from keras.models import Sequential
from keras.layers import Dense, Flatten, Conv2D, MaxPooling2D,Dropout
import numpy as np
```
```
def load_images_from_folder(folder, eyes =0):
    count = 0
    error_count = 0
    images = []
    for filename in os.listdir(folder):
        try:
            img = cv2.imread(os.path.join(folder,filename))
            img = cv2.resize(img, (80, 80))
            images.append([img, eyes])
        except:
            error_count += 1
            print('ErrorCount = ' + str(error_count))
            continue
        
        count += 1
        if count % 500 == 0:
            print('Successful Image Import Count = ' + str(count))
    
    return images

folder = "C:/Users/Kiot/dataset_eye_save"
open_eyes = load_images_from_folder(folder, 0)

folder = "C:/Users/Kiot/dataset_close_save"
closed_eyes = load_images_from_folder(folder, 1)
eyes = closed_eyes + open_eyes
```
```
X = []
Y = []
for features, label in eyes:
    X.append(features)
    Y.append(label)
```
```
X = np.array(X).reshape(-1, 80, 80, 3)
Y = np.array(Y)
X = X/255.0
```
```
X_train, X_test, Y_train, Y_test = train_test_split(X, Y, stratify = Y)
```
```
model = Sequential()

model.add(Conv2D(
                 filters = 32,
                 kernel_size = (3, 3),
                 activation = 'relu',
                 input_shape = (80,80,3)
                 ))
model.add(Conv2D(
                 filters = 32,
                 kernel_size = (3,3),
                 activation = 'relu'
                 ))
model.add(Conv2D(
                 filters = 32,
                 kernel_size = (3,3),
                 activation = 'relu'
                 ))

model.add(MaxPooling2D(pool_size = (2,2)))

model.add(Conv2D(
                 filters = 32,
                 kernel_size = (3,3),
                 activation = 'relu'
                 ))
model.add(Conv2D(
                 filters = 32,
                 kernel_size = (3,3),
                 activation = 'relu'
                 ))

model.add(MaxPooling2D(pool_size=(2,2)))

model.add(Flatten())

model.add(Dense(256, activation='relu'))

model.add(Dropout(0.3))

model.add(Dense(128, activation='relu'))
model.add(Dropout(0.3))

model.add(Dense(64, activation='relu'))
model.add(Dropout(0.3))

model.add(Dense(1, activation = 'sigmoid'))

model.compile(loss='binary_crossentropy',
                 optimizer = 'adam',
                 metrics=[tf.keras.metrics.AUC(curve = 'PR')])

model.fit(X_train,
           Y_train,
           batch_size = 800,
           validation_data = (X_test, Y_test),
           epochs=24)

model.evaluate(X_test, Y_test, verbose=1)
```
![image](https://user-images.githubusercontent.com/115389450/235434043-6bab4722-9921-4067-a877-cb03850dcc67.png)
```
!pip install keras
```
```
%tensorflow_version 2.x
import tensorflow as tf
device_name = tf.test.gpu_device_name()
if device_name != '/device:GPU:0':
  raise SystemError('GPU device not found')
print('Found GPU at: {}'.format(device_name))
```
```
from tensorflow.keras import Sequential
from tensorflow.keras import layers
from tensorflow.keras.optimizers import RMSprop
```
```
!pip install -q keras
import keras
```
```
!nvidia-smi
```
```
from google.colab import drive
drive.mount('/content/drive')
```
```
import cv2
import tensorflow as tf
from tensorflow import keras
from sklearn.model_selection import train_test_split
from tensorflow.keras.wrappers.scikit_learn import KerasClassifier
from keras.models import Sequential
from keras.layers import Dense, Flatten, Conv2D, MaxPooling2D,Dropout
import numpy as np
import os
```
```
gpu_info = !nvidia-smi
gpu_info = '\n'.join(gpu_info)
if gpu_info.find('failed') >= 0:
  print('Not connected to a GPU')
else:
  print(gpu_info)
```
```
from psutil import virtual_memory
ram_gb = virtual_memory().total / 1e9
print('Your runtime has {:.1f} gigabytes of available RAM\n'.format(ram_gb))

if ram_gb < 20:
  print('Not using a high-RAM runtime')
else:
  print('You are using a high-RAM runtime!')
```
```
import os
def load_images_from_folder(folder, eyes =0):
    count = 0
    error_count = 0
    images = []
    for filename in os.listdir(folder):
        try:
            img = cv2.imread(os.path.join(folder,filename))
            img = cv2.resize(img, (80, 80))
            images.append([img, eyes])
        except:
            error_count += 1
            print('ErrorCount = ' + str(error_count))
            continue
        
        count += 1
        if count % 500 == 0:
            print('Successful Image Import Count = ' + str(count))
    
    return images

folder = "/content/drive/MyDrive/Colab Notebooks/dataset_eye_save"
open_eyes = load_images_from_folder(folder, 0)

folder = "/content/drive/MyDrive/Colab Notebooks/dataset_close_save"
closed_eyes = load_images_from_folder(folder, 1)
eyes = closed_eyes + open_eyes
```
```
Successful Image Import Count = 500
Successful Image Import Count = 1000
Successful Image Import Count = 1500
Successful Image Import Count = 2000
Successful Image Import Count = 2500
Successful Image Import Count = 3000
Successful Image Import Count = 3500
Successful Image Import Count = 4000
Successful Image Import Count = 4500
Successful Image Import Count = 5000
Successful Image Import Count = 5500
Successful Image Import Count = 6000
Successful Image Import Count = 6500
Successful Image Import Count = 7000
Successful Image Import Count = 7500
Successful Image Import Count = 8000
Successful Image Import Count = 8500
Successful Image Import Count = 9000
Successful Image Import Count = 9500
Successful Image Import Count = 10000
Successful Image Import Count = 10500
Successful Image Import Count = 11000
Successful Image Import Count = 11500
Successful Image Import Count = 12000
Successful Image Import Count = 12500
Successful Image Import Count = 13000
Successful Image Import Count = 13500
Successful Image Import Count = 14000
Successful Image Import Count = 14500
Successful Image Import Count = 15000
Successful Image Import Count = 15500
Successful Image Import Count = 16000
Successful Image Import Count = 16500
Successful Image Import Count = 17000
Successful Image Import Count = 17500
Successful Image Import Count = 18000
Successful Image Import Count = 18500
Successful Image Import Count = 19000
Successful Image Import Count = 19500
Successful Image Import Count = 20000
Successful Image Import Count = 20500
Successful Image Import Count = 21000
Successful Image Import Count = 21500
Successful Image Import Count = 22000
Successful Image Import Count = 22500
Successful Image Import Count = 23000
Successful Image Import Count = 23500
Successful Image Import Count = 24000
Successful Image Import Count = 24500
Successful Image Import Count = 25000
Successful Image Import Count = 25500
Successful Image Import Count = 26000
Successful Image Import Count = 500
Successful Image Import Count = 1000
Successful Image Import Count = 1500
Successful Image Import Count = 2000
```
```
X = []
Y = []
for features, label in eyes:
    X.append(features)
    Y.append(label)
```
```
X = np.array(X).reshape(-1, 80, 80, 3)
Y = np.array(Y)
X = X/255.0
```
```
X_train, X_test, Y_train, Y_test = train_test_split(X, Y, stratify = Y)
```
```

model = Sequential()

model.add(Conv2D(
                filters = 32,
                kernel_size = (3, 3),
                activation = 'relu',
                input_shape = (80,80,3)
                ))
model.add(Conv2D(
                filters = 32,
                kernel_size = (3,3),
                activation = 'relu'
                ))
model.add(Conv2D(
                filters = 32,
                kernel_size = (3,3),
                activation = 'relu'
                ))

model.add(Conv2D(
                filters = 32,
                kernel_size = (3,3),
                activation = 'relu'
                ))
model.add(MaxPooling2D(pool_size = (2,2)))
model.add(Conv2D(
                filters = 32,
                kernel_size = (3,3),
                activation = 'relu'
                ))
model.add(MaxPooling2D(pool_size=(2,2)))

model.add(Flatten())

model.add(Dense(256, activation='relu'))

model.add(Dropout(0.3))

model.add(Dense(128, activation='relu'))
model.add(Dropout(0.3))

model.add(Dense(64, activation='relu'))
model.add(Dropout(0.3))

model.add(Dense(1, activation = 'sigmoid'))

model.compile(loss='binary_crossentropy',
                optimizer = 'adam',
                metrics=[tf.keras.metrics.AUC(curve = 'PR')])

model.fit(X_train,
        Y_train,
        batch_size = 800,
        validation_data = (X_test, Y_test),
        epochs=24)

model.evaluate(X_test, Y_test, verbose=1)
```
```
Epoch 1/24
27/27 [==============================] - 23s 227ms/step - loss: 0.3423 - auc: 0.0684 - val_loss: 0.2518 - val_auc: 0.1301
Epoch 2/24
27/27 [==============================] - 2s 82ms/step - loss: 0.2471 - auc: 0.1842 - val_loss: 0.2173 - val_auc: 0.3202
Epoch 3/24
27/27 [==============================] - 2s 82ms/step - loss: 0.1980 - auc: 0.4024 - val_loss: 0.1493 - val_auc: 0.6481
Epoch 4/24
27/27 [==============================] - 2s 81ms/step - loss: 0.1334 - auc: 0.6487 - val_loss: 0.1247 - val_auc: 0.7612
Epoch 5/24
27/27 [==============================] - 2s 82ms/step - loss: 0.1016 - auc: 0.7855 - val_loss: 0.0833 - val_auc: 0.8642
Epoch 6/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0821 - auc: 0.8527 - val_loss: 0.1091 - val_auc: 0.8553
Epoch 7/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0759 - auc: 0.8708 - val_loss: 0.0728 - val_auc: 0.8865
Epoch 8/24
27/27 [==============================] - 2s 83ms/step - loss: 0.0615 - auc: 0.9102 - val_loss: 0.0668 - val_auc: 0.9006
Epoch 9/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0561 - auc: 0.9233 - val_loss: 0.0669 - val_auc: 0.9189
Epoch 10/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0521 - auc: 0.9335 - val_loss: 0.0719 - val_auc: 0.9202
Epoch 11/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0498 - auc: 0.9398 - val_loss: 0.0608 - val_auc: 0.9197
Epoch 12/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0483 - auc: 0.9419 - val_loss: 0.0597 - val_auc: 0.9273
Epoch 13/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0416 - auc: 0.9557 - val_loss: 0.0614 - val_auc: 0.9286
Epoch 14/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0434 - auc: 0.9526 - val_loss: 0.0738 - val_auc: 0.8830
Epoch 15/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0474 - auc: 0.9439 - val_loss: 0.0640 - val_auc: 0.9172
Epoch 16/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0336 - auc: 0.9700 - val_loss: 0.0573 - val_auc: 0.9278
Epoch 17/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0300 - auc: 0.9750 - val_loss: 0.0594 - val_auc: 0.9317
Epoch 18/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0268 - auc: 0.9809 - val_loss: 0.0603 - val_auc: 0.9260
Epoch 19/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0235 - auc: 0.9827 - val_loss: 0.0654 - val_auc: 0.9247
Epoch 20/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0203 - auc: 0.9877 - val_loss: 0.0705 - val_auc: 0.9234
Epoch 21/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0213 - auc: 0.9861 - val_loss: 0.0751 - val_auc: 0.9104
Epoch 22/24
27/27 [==============================] - 2s 81ms/step - loss: 0.0170 - auc: 0.9900 - val_loss: 0.0718 - val_auc: 0.9206
Epoch 23/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0168 - auc: 0.9909 - val_loss: 0.0714 - val_auc: 0.9146
Epoch 24/24
27/27 [==============================] - 2s 82ms/step - loss: 0.0150 - auc: 0.9927 - val_loss: 0.0732 - val_auc: 0.9023
222/222 [==============================] - 1s 4ms/step - loss: 0.0732 - auc: 0.9024
[0.07322415709495544, 0.9023501873016357]
```
```
model.save('/content/gdrive/My Drive/yuiyongmodel.h5')
```
![image](https://user-images.githubusercontent.com/115389450/235434385-7f431492-8559-43b1-8812-889b691f88d2.png)

![image](https://user-images.githubusercontent.com/115389450/235434422-b8ab36cc-8a76-4845-bc52-5ef941a6b8a7.png)
![image](https://user-images.githubusercontent.com/115389450/235434461-ab647c4a-0a89-42e3-903c-cd2fc8752dd7.png)
![image](https://user-images.githubusercontent.com/115389450/235434491-f1fc78aa-3e24-4a47-8fe2-f728cbbd4ba6.png)
![image](https://user-images.githubusercontent.com/115389450/235434520-8cd7ba3e-90bd-485c-812c-e3f486454f0d.png)
