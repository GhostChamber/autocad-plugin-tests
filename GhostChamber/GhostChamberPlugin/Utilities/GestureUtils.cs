using System;
using Microsoft.Kinect;

namespace GhostChamberPlugin.Utilities
{
    public static class GestureUtils
    {
        public static double GetJointDistance(Joint j1, Joint j2)
        {
            double dX = j1.Position.X - j2.Position.X;
            double dY = j1.Position.Y - j2.Position.Y;
            double dZ = j1.Position.Z - j2.Position.Z;
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"CLAMP SEPARATION : {Math.Sqrt(dX * dX + dY * dY + dZ * dZ)}\n");
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

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

        internal static void ReleaseGesture(Body activeBody)
        {
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
