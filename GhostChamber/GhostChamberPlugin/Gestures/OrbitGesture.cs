﻿using System;
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
        private bool previouslyActive = false;

        const double ROTATION_COMMAND_THRESHOLD = 0.005f;
        const int SMOOTHING_WINDOW = 5;

        public OrbitGesture()
        {

        }

        public bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; ++i)
                {
                    Microsoft.Kinect.Body body = skeletons[i];

                    if (IsGestureActive(body))
                    {
                        activeBody = body;
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ORBIT\n");

                        // Record right hand location
                        toolStartPosition = activeBody.Joints[JointType.HandLeft].Position;
                        toolPreviousPosition = toolStartPosition;
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

            if(body.HandRightState == HandState.Closed)
            {
                if (previouslyActive)
                {
                    return true;
                }
                else
                {
                    if (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public Vector3d Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = new Vector3d(0.0, 0.0, 0.0);

            if (activeBody != null)
            {
                toolPosition = activeBody.Joints[JointType.HandLeft].Position;

                double dX = (toolPosition.X - toolPreviousPosition.X);
                double dY = (toolPosition.Y - toolPreviousPosition.Y);
                double dZ = (toolPosition.Z - toolPreviousPosition.Z);

                double distance = Math.Sqrt(dX*dX + dY*dY + dZ*dZ);

                if (distance > ROTATION_COMMAND_THRESHOLD)
                {
                    toolPreviousPosition = toolPosition;
                    movement = new Vector3d(dX, dY, dZ);
                }

                if (!IsGestureActive(activeBody))
                {
                    activeBody = null;
                    //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
                }
            }

            return movement;
        }
    }
}
