### R3G / Unity

## The aims of the project

This repository is the Unity part of the R3G project. The R3G project is a software suite for gesture recognition. This project was carried out by seven students from INSA Rennes during their fourth year. This software suite aims to help an INSA/IRISA doctoral student to develop a gesture recognition system. The Unity part of this project allows the user to create a user profile and a gesture database. It also allows him to test the performance of his recognition system. The project also includes a web application allowing him to explore, annotate the database and train the recognition system.

## Run the project

To launch the project, you need the 2019 version of Unity and a gesture sensor such as the LeapMotion, the first version of the Kinect or the second version.

## The scenes

The Unity part of R3G consists of four main scenes.

### UsersManagement

This scene allows you to create and manage a user profile. It allows you to record information such as age, weight, height and handiness.

### DBManagement

This scene allows you to create and manage the databases. It allosw you to associate a path from your hard drive, a recognition system and a gesture sensor.

### Acquisition

This scene allows the user to create gestures. First, it is necessary to choose a user profile, a gesture sensor and a database. The database contains a list of example gestures for the user and the gestures performed. You can first create a list of example gestures that will be the references of gestures in the database. The user can name these gestures and view them. Then, he can give instructions for example gestures to be performed for himself or another user. The user starts the gesture recording, follows the example gesture, and then can view the recording.

### Demonstration

This scene allows the user to test their recognition system. To do this, the user must first choose a user profile, a recognition system and a database. Then, the sample gestures from the database are displayed on his screen. He can then start the session and perform the gestures. The predictions of the recognition system are displayed in green. At the end of the session, he can see a summary of his session with the predictions made by the recognition system.
