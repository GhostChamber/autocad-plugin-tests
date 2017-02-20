using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static bool isCommandPositionActive(Body body, float threshold)
        {
            return false;
        }

        public static void ReleaseGesture(Body body, GestureType type)
        {

        }

        public static bool IsToolPositionActive(Body body, float threshold)
        {
            return false;
        }
    }
}
