using System;
using Microsoft.Kinect;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Utilities
{
    public static class GestureUtils
    {
        public const double CLAMP_THRESHOLD = 0.09;
        public const double CAPTURE_THRESHOLD = 0.2;
        public const double CAPTURE_DEPTH_OFFSET = 0.4;

        //Currently unused, but useful.
        public static double GetJointDistance(Joint j1, Joint j2)
        {
            double dX = j1.Position.X - j2.Position.X;
            double dY = j1.Position.Y - j2.Position.Y;
            double dZ = j1.Position.Z - j2.Position.Z;
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"CLAMP SEPARATION : {Math.Sqrt(dX * dX + dY * dY + dZ * dZ)}\n");
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        //Currently unused.
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
            if(Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold &&
               activeBody.Joints[JointType.HandLeft].Position.Z < activeBody.Joints[JointType.Head].Position.Z - 0.4)
            {
                return true;
            }
            return false;
        }

        //Currently unused
        internal static void ReleaseGesture(Body activeBody)
        {
            activeBody = null;
        }

        //Used by Pan Gesture
        internal static bool IsToolPositionActive( Body activeBody, float threshhold )
        {
            if(Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold)
            {
                return true;
            }

            return false;
        }

        internal static bool IsGrabGestureActive(Body activeBody)
        {
            if ((Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > GestureUtils.CAPTURE_THRESHOLD) // check if hand is above the shoulder
                        && (activeBody?.HandLeftState == HandState.Closed))
            {
                return true;
            }
            return false;
        }

        internal static bool IsOrbitGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD
                && body?.HandRightState == HandState.Closed)
            {
                return true;
            }
            return false;
        }

        internal static bool IsZoomGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if ((Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD) &&
                (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD))
            {
                //if (body.Joints[JointType.HandRight].Position.Z > body.Joints[JointType.Head].Position.Z - GestureUtils.CAPTURE_DEPTH_OFFSET &&
                //    body.Joints[JointType.HandLeft].Position.Z > body.Joints[JointType.Head].Position.Z - GestureUtils.CAPTURE_DEPTH_OFFSET)
                //{
                    return true;
                //}
                //if (GestureUtils.GetJointDistance(body.Joints[JointType.ThumbRight], body.Joints[JointType.HandTipRight]) > GestureUtils.CLAMP_THRESHOLD)
                //{
                //    return true;
                //}
            }

            return false;
        }
    }
}
