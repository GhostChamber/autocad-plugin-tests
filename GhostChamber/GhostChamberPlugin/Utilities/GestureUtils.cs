using System;
using Microsoft.Kinect;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Utilities
{
    /**
     * Class of Util methods for the Gestures.
     * @static
     */
    public static class GestureUtils
    {
        public const double CAPTURE_THRESHOLD = 0.2;            /**< Default threshold to check if limb is above shoulder height. */

        /** Currently unused, but useful. May be used in future expansion of project */
        public static double GetJointDistance(Joint j1, Joint j2)
        {
            double dX = j1.Position.X - j2.Position.X;
            double dY = j1.Position.Y - j2.Position.Y;
            double dZ = j1.Position.Z - j2.Position.Z;
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"CLAMP SEPARATION : {Math.Sqrt(dX * dX + dY * dY + dZ * dZ)}\n");
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        /** Currently unused, but useful. May be used in future expansion of project */
        internal static bool isJointLessThan(Body activeBody, JointType jointA, JointType jointB)
        {
            if (activeBody.Joints[jointA].Position.X <= activeBody.Joints[jointB].Position.X)
            {
                if (activeBody.Joints[jointA].Position.Y <= activeBody.Joints[jointB].Position.Y)
                {
                    if (activeBody.Joints[jointA].Position.Z <= activeBody.Joints[jointB].Position.Z)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /** Currently unused. */
        internal static bool isCommandPositionActive(Body activeBody, float threshhold)
        {
            if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold &&
               activeBody.Joints[JointType.HandLeft].Position.Z < activeBody.Joints[JointType.Head].Position.Z - 0.4)
            {
                return true;
            }
            return false;
        }

        /** Currently unused, but useful. May be used in future expansion of project */
        internal static void ReleaseGesture(Body activeBody)
        {
            activeBody = null;
        }

        /** 
         * @static
         * @deprecated
         * Used by Pan Gesture. Used to check if the Right Hand is above the shoulder.
         * @param activeBody Kinect.Body value of the body being detected.
         * @param threshold value to check Y position relative to Head.
         * @return true if Right Hand is less than the threshold in the Y from Head.
         */
        internal static bool IsToolPositionActive(Body activeBody, float threshhold)
        {
            if (Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < threshhold)
            {
                return true;
            }

            return false;
        }

        /**
         * @static
         * Check if Grab gesture is Active.
         * @param body Kinect.Body currently being detected.
         * @return true if left hand is above shoulder height and the hand is grabbing.
         */
        internal static bool IsGrabGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if (Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < CAPTURE_THRESHOLD
                && body.HandLeftState == HandState.Closed)
            {
                return true;
            }
            return false;
        }

        /**
         * @static
         * Check if Orbit gesture is Active.
         * @param body Kinect.Body currently being detected.
         * @return true if right hand is above shoulder height and the hand is grabbing.
         */
        internal static bool IsOrbitGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < CAPTURE_THRESHOLD
                && body.HandRightState == HandState.Closed)
            {
                return true;
            }
            return false;
        }

        /**
         * @static
         * Check if Zoom gesture is Active.
         * @param body Kinect.Body currently being detected.
         * @return true if Left and right hands are above shoulder height.
         */
        internal static bool IsZoomGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if ((Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD) &&
                (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD))
            {
                return true;
            }

            return false;
        }

        /**
         * @static
         * Check if SnapBack gesture is Active.
         * @param body Kinect.Body currently being detected.
         * @return true if Left hand is above the shoulder height and the hand is grabbing.
         */
        internal static bool IsSnapBackGestureActive(Body body)
        {
            
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD
                && body.HandRightState == HandState.Lasso)
            {
                return true;
            }
            return false;
        }
    }
}
