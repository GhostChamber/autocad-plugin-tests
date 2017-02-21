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
        private CameraSpacePoint rightStartPosition;
        private CameraSpacePoint rightPreviousPosition;
        private CameraSpacePoint rightPosition;

        public const double CAPTURE_THRESHOLD = 0.2;
        public const double CLAMP_THRESHOLD = 0.1;
        const double ROTATION_COMMAND_THRESHOLD = 0.05f;
        const int SMOOTHING_WINDOW = 5;

        public OrbitGesture()
        {

        }

        public bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; i++)
                {
                    Microsoft.Kinect.Body body = skeletons[i];

                    double distance = GestureUtils.GetJointDistance(body.Joints[JointType.ThumbLeft], body.Joints[JointType.HandTipLeft]);
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"{distance}\n");

                    if (IsGestureActive(body))
                    {
                        activeBody = body;
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ORBIT\n");

                        // Record right hand location
                        rightStartPosition = activeBody.Joints[JointType.HandRight].Position;
                        rightPreviousPosition = rightStartPosition;
                        break;
                    }
                }
            }
            return (activeBody != null);
        }

        public bool IsGestureActive(Body body)
        {
            if  (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if(Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < CAPTURE_THRESHOLD)
            {
                if (GestureUtils.GetJointDistance(body.Joints[JointType.ThumbLeft], body.Joints[JointType.HandTipLeft]) < CLAMP_THRESHOLD)
                {
                    return true;
                }
            }

            return false;
        }

        public Vector3d Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = new Vector3d(0.0, 0.0, 0.0);

            if (activeBody != null)
            {
                rightPosition = activeBody.Joints[JointType.HandRight].Position;

                double dX = (rightPosition.X - rightPreviousPosition.X);
                double dY = (rightPosition.Y - rightPreviousPosition.Y);
                double dZ = (rightPosition.Z - rightPreviousPosition.Z);

                double distance = Math.Sqrt(dX*dX + dY*dY + dZ*dZ);

                if (distance > ROTATION_COMMAND_THRESHOLD)
                {
                    rightPreviousPosition = rightPosition;
                    movement = new Vector3d(dX, dY, dZ);
                }

                if (!IsGestureActive(activeBody))
                {
                    activeBody = null;
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
                }
            }

            return movement;
        }
    }
}
