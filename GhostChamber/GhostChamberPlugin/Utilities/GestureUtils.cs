using Microsoft.Kinect;
using System;

namespace GhostChamberPlugin.Utilities
{
    public static class GestureUtils
    {
        internal static bool isJointLessThan( Body activeBody, JointType jointA, JointType jointB )
        {
            if(activeBody.Joints[jointA].Position.X <= activeBody.Joints[jointB].Position.X)
            {
                if(activeBody.Joints[jointA].Position.Y <= activeBody.Joints[jointB].Position.Y)
                {
                    if(activeBody.Joints[jointA].Position.Z <= activeBody.Joints[jointB].Position.Z)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool isCommandPositionActive( Body activeBody, float threshhold )
        {
            if(Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold)
            {
                return true;
            }
            return false;
        }

        internal static void ReleaseGesture( Body activeBody, GestureType gestureType)
        {
            gestureType = GestureType.NONE;
            activeBody = null;
        }

        internal static bool IsToolPositionActive( Body activeBody, float threshhold )
        {
            if(Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold)
            {
                return true;
            }
            return false;
        }
    }
}
