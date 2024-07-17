# Attendance Monitoring System
Final Outcomes-Based Education (OBE) Project for Interfacing Machine-to-Machine Communications (IT154P) course.
## Overview
The system monitors attendance of users by using NFC-RFID cards. It uses a combination of microcontrollers, web service, desktop application, and mobile application.
## Hardware
The microcontrollers used are Arduino Uno, Arduino Leonardo, and WeMos D1 R2. The component used for RFID capture is a Mifare RC-522 RFID module.
## Software
### Web Service
Data is stored in a MySQL database. The web service uses a REST API that the applications and the microcontrollers interface with. The API is written purely in PHP.
### Desktop Application
The program is a Universal Windows Platform (UWP) application compatible in Windows 10. The app is used by the systems administrators to manage user accounts and view attendance logs. Management of user accounts include creating user accounts, pairing RFID card, updating information, and reseting password.
### Mobile Application
The mobile application is created using Xamarin Android and is used by the end users to view their attendance. The user can also change his account password from the app.
