using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using System.Diagnostics;

namespace GhostChamberPlugin.Gestures
{
    class PanGesture : Gesture
	{
        private Body activeBody = null;
        private Vector3d centerPosition = new Vector3d(0,0,0);
        int count = 0;

        public bool IsActive(IList<Body> skeletons)
        {
            if(activeBody == null && skeletons != null)
            {
                for(int i = 0; i < skeletons.Count; i++)
                {
                    Body body = skeletons[i];
                    if(body.Joints[JointType.Head].Position.Y != 0.0f &&
                        GestureUtils.isCommandPositionActive(body, 0.2f))
                    {
                        centerPosition = new Vector3d(body.Joints[JointType.HandLeft].Position.X, body.Joints[JointType.HandLeft].Position.Y, 0);
                        activeBody = body;
                        break;
                    }
                }
            }

            return (activeBody != null);
        }

        public Vector3d Update()
        {
            if (activeBody != null)
            {
                if (GestureUtils.isCommandPositionActive(activeBody, 0.5f) && !GestureUtils.IsToolPositionActive(activeBody, 0.2f))
                {
                    if(activeBody.Joints[JointType.HandLeft].Position.X != centerPosition.X || activeBody.Joints[JointType.HandLeft].Position.Y != centerPosition.Y)
                    {
                        Vector3d acceleration = new Vector3d(centerPosition.X - activeBody.Joints[JointType.HandLeft].Position.X, centerPosition.Y - activeBody.Joints[JointType.HandLeft].Position.Y,0);
                        return acceleration;
                    }

                }
            }

            if (activeBody != null)
            {
                // kinect units are in meters. Hence left - right is scaled from minHandDistance to maxHandDistance
                if (!GestureUtils.isCommandPositionActive(activeBody, 0.2f))
                {
                    count++;
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"DEACTIVATE PAN {count}\n");
                    activeBody = null;
                }
            }

            return new Vector3d(0,0,0);
        }
    }
}
