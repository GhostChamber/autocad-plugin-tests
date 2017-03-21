using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;


namespace GhostChamberPlugin.Gestures
{
    public sealed class OrbitGesture : Gesture
    {
        private Microsoft.Kinect.Body activeBody = null;
        private CameraSpacePoint toolStartPosition;
        private CameraSpacePoint toolPreviousPosition;
        private CameraSpacePoint toolPosition;

        const double ROTATION_COMMAND_THRESHOLD = 0.005f;
        const int SMOOTHING_WINDOW = 5;

        public bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; ++i)
                {
                    Microsoft.Kinect.Body body = skeletons[i];

                    if (GestureUtils.IsOrbitGestureActive(body))
                    {
                        activeBody = body;
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ORBIT\n");

                        // Record right hand location
                        toolStartPosition = activeBody.Joints[JointType.HandRight].Position;
                        toolPreviousPosition = toolStartPosition;
                        break;
                    }
                }
            }
            return (activeBody != null);
        }

        public Vector3d Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = new Vector3d(0.0, 0.0, 0.0);

            if (activeBody != null)
            {
                toolPosition = activeBody.Joints[JointType.HandRight].Position;

                double dX = (toolPosition.X - toolPreviousPosition.X);
                double dY = (toolPosition.Y - toolPreviousPosition.Y);
                double dZ = (toolPosition.Z - toolPreviousPosition.Z);

                double distance = Math.Sqrt(dX*dX + dY*dY + dZ*dZ);

                if (distance > ROTATION_COMMAND_THRESHOLD)
                {
                    toolPreviousPosition = toolPosition;
                    movement = new Vector3d(dX, dY, dZ);
                }

                if (activeBody.HandRightState != HandState.Closed)
                {
                    activeBody = null;
                    //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
                }
            }

            return movement;
        }
    }
}
