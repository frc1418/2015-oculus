FRC1418 2015 Oculus Rift driver station interface
=================================================

* Code: [Robot](https://github.com/frc1418/2015-robot) | [UI](https://github.com/frc1418/2015-ui) | [Image Processing](https://github.com/frc1418/2015-vision) | **Oculus Rift**
* Factsheet: [Google Doc](https://docs.google.com/document/d/1irbUm-Qfxz_Ua2XiB5KzYWG2Ec5Xhr038NqL-k4FveA)
* Oculus Rift Whitepaper: [Google Doc](https://docs.google.com/document/d/1-8BB0rzydTxpMA9buoe7J2LLSpy6g8wTbeJXNPeNb_0/)

We're really excited to show off the details of our Oculus Rift interface and
release the Oculus code for 2015!

During the build season our team came up with the idea of using the 
Oculus to help drive our robot. We all put in some money to get it.
We developed our code in a 3d game development platform called Unity
that already had very good integration with the Oculus.

Although ultimately FIRST disallowed usage of the Oculus in competition,
technologies like this have a lot of potential. We think that this type
of technology can potentially change the way that we interact with and
control our robots in the future.

This project requires Unity Pro for compilation. Unity Pro licenses were
generously provided to our team by Unity Technologies.

**Checkout our whitepaper for pictures and video: [here](https://docs.google.com/document/d/1-8BB0rzydTxpMA9buoe7J2LLSpy6g8wTbeJXNPeNb_0/)**

If you do not own a Unity Pro license you are in luck! See our 1.0 release
that has **compiled binaries: [here](https://github.com/frc1418/2015-oculus/releases/tag/1.0)** 

Features
================

Camera mounted on the Oculus
------------

![ScreenShot](Pictures/CameraWindow.png)

We used a 3d printer to create a mount for a camera on the front of the
Oculus. This was intended to allow the operator to see their surroundings.

Code can be seen at:

    ./Assets/StreamScreen.cs
	
Simulation of the masts on our robot
-----------------------------

![ScreenShot](Pictures/RobotMasts.JPG)
![ScreenShot](Pictures/Mast.png)

This can be seen in the picture above as the green spots inside the
highlighted red. These move up and down according to our robots
position. There is only one thing on the left mast while there are
three on the right one.

This simulation was done by grabbing the encoder values from the robot.
Once we have the encoder values we compare them to the ones that have
been recorded of the top and the bottom and gives the representations
some scaled coordinates to go to.

Code for the right mast can be seen at:

    ./Assets/RobotControl/ToteForkliftControler.cs
    
Code for the Left mast can be seen at:

    ./Assets/RobotControl/CanForkliftControler.cs
	
Simulation of totes in front of robot
--------------------

![ScreenShot](Pictures/Tote.png)

This can be seen in the picture above. There is a small box that changes
angle and position based off of two distance sensors on the robot:

![ScreenShot](Pictures/DistanceSensors.JPG)

The tote will only show half of itself if only one of the distance sensors
is actively detecting something. This can only show position and not distance.

![ScreenShot](Pictures/HalfTote.png)

The tote will also turn green when two limit switches are pressed in front of
the robot.

![ScreenShot](Pictures/ToteGreen.png)

The full math and code can be seen at:

    ./Assets/RobotControl/ToteControler.cs
    
Simulation
================

If you would like to run this program to see some moving parts it does work with
the PyFRC simulator. You can download **compiled binaries: [here](https://github.com/frc1418/2015-oculus/releases/tag/1.0)**. To make this work you must have pyfrc installed (through pip3),
python3, and you must clone our UI and Robot code. Once you have all of this installed
you can run our simulated robot code with:

    cd 2015-robot/robot/
    python3 robot.py sim

You must also run our driverstation client/server with:

	cd 2015-ui/
    python3 driverStationServer.py
    
Once that is run you can open the VR-UI while moving values with buttons 5 and 4 on
joystick 1 and 2. You should see the simulation of the Masts moving.

If you would like this simulation to work with joysticks you must install a python
module called PyGame. This any joysticks plugged into the computer to be accessed.

Authors
=======

Students

* Carter Fendley

Dustin Spicuzza, mentor
