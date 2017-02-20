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
        private double currentRotation = 0.0;
        private float orbitRotation = 0.0f;
        private bool rightHandCaptured = false;
        private CameraSpacePoint rightStartPosition;
        private CameraSpacePoint rightPreviousPosition;
        private CameraSpacePoint rightPosition;

        private Queue clampDistances;
        private double averageClampDistance;

        public const double CAPTURE_THRESHOLD = 0.2;
        public const double CLAMP_THRESHOLD = 0.1;
        const double ROTATION_COMMAND_THRESHOLD = 0.05f;
        const int SMOOTHING_WINDOW = 5;

        public OrbitGesture()
        {
            clampDistances = new Queue();

            for (int i = 0; i < SMOOTHING_WINDOW; i++)
            {
                clampDistances.Enqueue(0.0);
            }

            //double sum = 0.0;

            //foreach (Object obj in clampDistances)
            //{
            //    double value = (double)obj;
            //    sum += value;
            //}

            //averageClampDistance = sum / SMOOTHING_WINDOW;
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

                    if (IsGestureStarted(body))
                    {
                        activeBody = body;
                        currentRotation = 0.0f;
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ORBIT\n");
                        break;
                    }
                }
            }
            return (activeBody != null);
        }

        public bool IsGestureStarted(Body body)
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

            if (activeBody != null && !rightHandCaptured)
            {
                if (Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < CAPTURE_THRESHOLD)// &&
                    //GetJointDistance(activeBody.Joints[JointType.ThumbRight], activeBody.Joints[JointType.HandTipRight]) < CLAMP_THRESHOLD)
                {
                    rightHandCaptured = true;
                    rightStartPosition = activeBody.Joints[JointType.HandRight].Position;
                    rightPreviousPosition = rightStartPosition;
                }
            }

            if (activeBody != null && rightHandCaptured)
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

                if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > CAPTURE_THRESHOLD)
                {
                    rightHandCaptured = false;
                    activeBody = null;
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
                }
            }

            return movement;
        }
    }
}
