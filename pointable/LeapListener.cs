/******************************************************************************\
* Copyright (C) 2012-2013 Leap Motion, Inc. All rights reserved.               *
* Leap Motion proprietary and confidential. Not for distribution.              *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement         *
* between Leap Motion and you, your company or other organization.             *
\******************************************************************************/
using System;
using System.Threading;
using Leap;
using LeapProject;
using System.Diagnostics;
using System.Windows.Forms;

class LeapListener : Listener
{
	private Object thisLock = new Object ();
    private LeapEventListener mainListener;
    private bool gestureStarted = false;

	private void SafeWriteLine (String line)
	{
        try
        {
            lock (thisLock)
            {
                Debug.WriteLine(line);
            }
        }
        catch { }
	}

    public LeapListener(LeapEventListener listener)
    {
        mainListener = listener;
    }
	public override void OnInit (Controller controller)
	{
		SafeWriteLine ("Initialized");
	}

	public override void OnConnect (Controller controller)
	{
		SafeWriteLine ("Connected");
        try
        {
            controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            //controller.EnableGesture (Gesture.GestureType.TYPESWIPE);

            mainListener.leapConnected();
        }
        catch { }
	}

	public override void OnDisconnect (Controller controller)
    {
        SafeWriteLine("Disconnected");
        try
        {
            mainListener.leapDisconnected();
        }
        catch { }
        //MessageBox.Show("Leap Motion Controller has been disconnected", "Pointable");
	}

	public override void OnExit (Controller controller)
	{
		SafeWriteLine ("Exited");
	}

    int currentCircleID = 0;

	public override void OnFrame (Controller controller)
	{
        try
        {

            if (!Form1.detectionActivated) return;

            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();
            FingerList fingers = frame.Fingers;
            PointableList pointables = frame.Pointables;

            if (pointables.IsEmpty)
            {
                // if (gestureStarted)
                if (frame.Hands.IsEmpty)
                {//no palm no fingers
                    mainListener.leapEndGesture();
                }
                else
                {//got palm without fingers
                    Debug.WriteLine("Hand without fingers");
                    mainListener.leapPalmClosed();
                }
            }
            else
            {
                  mainListener.leapPointableAll(frame);

                if (!frame.Hands.IsEmpty)
                {
                    Vector palmPos = frame.Hands.Frontmost.PalmPosition;
                    Vector palmDir = frame.Hands.Frontmost.Direction;
                    Vector palmNormal = frame.Hands.Frontmost.PalmNormal;
                    mainListener.leapPalmData(new Vector3(palmPos.x, palmPos.y, palmPos.z), new Vector3(palmDir.x, palmDir.y, palmDir.z), new Vector3(palmNormal.x, palmNormal.y, palmNormal.z));
                }
                else
                {
                    mainListener.leapPalmNotFound();
                }
                // mainListener.leapPalm(
            }

            if (!frame.Hands.IsEmpty)
            { //sometimes fingers not detected as same hand
                if (frame.Hands[0].Fingers.Count >= 4 && frame.Hands.Frontmost.PalmVelocity.Magnitude < 40) //30
                {
                    gestureStarted = true;
                    mainListener.leapStartGesture();
                }
            }


            if (currentCircleID !=0 && !frame.Gesture(currentCircleID).IsValid)
            {
                currentCircleID = 0;
                mainListener.leapCircleGestureEnd();
            }

            // Get gestures
            GestureList gestures = frame.Gestures();
            for (int i = 0; i < gestures.Count; i++)
            {
                Gesture gesture = gestures[i];
                
                switch (gesture.Type)
                {
                    case Gesture.GestureType.TYPECIRCLE:

                        CircleGesture circle = new CircleGesture(gesture);

                        bool isHand = !(gesture.Hands.IsEmpty);

                        if (currentCircleID != 0 && circle.Id != currentCircleID)
                        {
                        }
                        else
                        {
                            // Calculate clock direction using the angle between circle normal and pointable
                           // String clockwiseness;
                            bool isClockwise = false;
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {
                                //Clockwise if angle is less than 90 degrees
                                isClockwise = true;
                                // clockwiseness = "clockwise";
                            }
                            else
                            {
                                //clockwiseness = "counterclockwise";
                            }

                            float sweptAngle = 0;

                            // Calculate angle swept since last frame

                            if (circle.State == Gesture.GestureState.STATESTOP)
                            {
                                currentCircleID = 0;
                                mainListener.leapCircleGestureEnd();
                            }
                            else if (circle.State == Gesture.GestureState.STATESTART && currentCircleID==0)
                            {
                                currentCircleID = circle.Id;
                                mainListener.leapCircleGestureStart(isClockwise, frame.Fingers.Count, isHand);
                            }
                            else
                            {
                                CircleGesture previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
                                sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;

                                mainListener.leapCircleGesture(circle, sweptAngle, frame.Fingers.Count, isHand);
                            }

                            SafeWriteLine("Circle id: " + circle.Id
                                           + ", " + circle.State
                                           + ", progress: " + circle.Progress
                                           + ", radius: " + circle.Radius
                                           + ", angle: " + sweptAngle
                                           + ", " + isClockwise);
                        }
                        //if (circle.State != Gesture.GestureState.STATESTART )
                        //{
                        //    CircleGesture previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
                        //    sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;

                        //    mainListener.leapCircleGesture(circle, sweptAngle);
                        //}
                        //else
                        //{

                        //    mainListener.leapCircleGestureStart();
                        //}

                        //Console.WriteLine("Circle id: " + circle.Id
                        //               + ", " + circle.State
                        //               + ", progress: " + circle.Progress);
                        break;
                    case Gesture.GestureType.TYPESWIPE:
                        SwipeGesture swipe = new SwipeGesture(gesture);
                        SafeWriteLine("Swipe id: " + swipe.Id
                                       + ", " + swipe.State
                                       + ", position: " + swipe.Position
                                       + ", direction: " + swipe.Direction
                                       + ", speed: " + swipe.Speed);
                        break;
                    case Gesture.GestureType.TYPEKEYTAP:
                        
                        KeyTapGesture keytap = new KeyTapGesture(gesture);

                        if (keytap.Direction.y < -0.92)
                            mainListener.leapTapGesture(true);
                        //check distance travelled within 100ms

                       // Console.WriteLine(keytap.Id + "  direction=" + keytap.Direction);

                        SafeWriteLine("----------------Tap id: " + keytap.Id
                                       + ", " + keytap.State
                                       + ", position: " + keytap.Position
                                       + ", direction: " + keytap.Direction + "----------------------------------------");
                        break;
                    case Gesture.GestureType.TYPESCREENTAP:
                        ScreenTapGesture screentap = new ScreenTapGesture(gesture);


                        mainListener.leapTapGesture(false);

                        SafeWriteLine("Screen Tap id: " + screentap.Id
                                       + ", " + screentap.State
                                       + ", position: " + screentap.Position
                                       + ", direction: " + screentap.Direction);
                        break;
                    default:
                        SafeWriteLine("Unknown gesture type.");
                        break;
                }
            }

        }
        catch (Exception ex) { int i = 0; }
	}
}
