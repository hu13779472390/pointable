using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapProject
{
    interface LeapEventListener
    {
        void leapFingerTipData(Vector3 fingerTip, Vector3 fingerTipOut);
       // void leapFingerTipAll(Leap.Finger finger);
        void leapPointableAll(Leap.Frame pointables);
        void leapPalmData(Vector3 palmPosition, Vector3 palmDirection, Vector3 palmNormal);
        void leapPalmNotFound();
        void leapStartGesture();
        void leapEndGesture();
        void leapGesture(Vector3 handCoord);
        void leapCircleGestureStart(bool isClockwise, int fingerCount,bool isHand);
        void leapCircleGestureEnd();
        void leapCircleGesture(CircleGesture circle, double sweptAngle, int fingerCount,bool isHand);
        void leapTapGesture(bool checkDistance);
        void leapPalmClosed();

        void leapConnected();
        void leapDisconnected();
    }
}
