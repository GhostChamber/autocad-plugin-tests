using Autodesk.AutoCAD.ApplicationServices;
using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace GhostChamberPlugin.Gestures
{
	public sealed class ZoomGesture : Gesture
	{
		private Microsoft.Kinect.Body activeBody = null;
		private double minHandDistance = 0.15;
		private double maxHandDistance = 0.85;
		private double currentZoom = 1.0;
		private float zoomScale = 1.0f;
		private double zoomRightStart;
		private double zoomRight;

        public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			if (activeBody == null && skeletons != null)
			{
				for (int i = 0; i < bodyCount; i++)
				{
					Microsoft.Kinect.Body body = skeletons[i];
                    if (IsGestureActive(body))
					{
						activeBody = body;
						currentZoom = 1.0;
                        zoomRightStart = activeBody.Joints[JointType.HandRight].Position.X;
                        zoomRight = zoomRightStart;
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ZOOM\n");
						break;
					}
				}
			}
			return (activeBody != null);
		}

        public bool IsGestureActive(Body body)
        {
            if (body.Joints[JointType.Head].Position.Y == 0.0f)
            {
                return false;
            }

            if ((Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD) &&
                (Math.Abs(body.Joints[JointType.HandRight].Position.Y - body.Joints[JointType.Head].Position.Y) < GestureUtils.CAPTURE_THRESHOLD))
            {
                if (body.Joints[JointType.HandRight].Position.Z > body.Joints[JointType.Head].Position.Z - 0.5)
                {
                    return true;
                }
                //if (GestureUtils.GetJointDistance(body.Joints[JointType.ThumbRight], body.Joints[JointType.HandTipRight]) > GestureUtils.CLAMP_THRESHOLD)
                //{
                //    return true;
                //}
            }

            return false;
        }

        public double Update(IList<Body> skeletons, int bodyCount)
		{

			if (activeBody != null)
			{
				zoomRight = activeBody.Joints[JointType.HandRight].Position.X;

				// kinect units are in meters. Hence left - right is scaled from minHandDistance to maxHandDistance
				double handDistance = (zoomRightStart - zoomRight);
				bool zoomOut = (handDistance < 0);
				handDistance = Math.Abs(handDistance);
				handDistance = handDistance.Clamp(minHandDistance, maxHandDistance);

				double zoomFraction = ((handDistance - minHandDistance) / (maxHandDistance - minHandDistance));
				if (zoomOut)
				{
					zoomFraction = (1 - zoomFraction*zoomScale);
				}
				else
				{
					zoomFraction = (1 + zoomFraction*zoomScale);
				}
				double returnValue = (zoomFraction / currentZoom);
				currentZoom = zoomFraction;

				if (!IsGestureActive(activeBody))
				{
					activeBody = null;
					Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
				}
				return returnValue;
			}
			return 1;
		}
	}
}
