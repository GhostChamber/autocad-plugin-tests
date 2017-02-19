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
		private bool zoomRightCaptured = false;
		private double zoomRightStart;
		private float zoomRight;

		public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			if (activeBody == null && skeletons != null)
			{
				for (int i = 0; i < bodyCount; i++)
				{
					Microsoft.Kinect.Body body = skeletons[i];
					if (body.Joints[JointType.Head].Position.Y != 0.0f &&
						(Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < 0.2f))
					{
						activeBody = body;
						currentZoom = 1.0;
						//Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Activated\n");
						break;
					}
				}
			}
			return (activeBody != null);
		}

		public double Update(IList<Body> skeletons, int bodyCount)
		{
			if (activeBody != null && !zoomRightCaptured)
			{
				if (Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y - activeBody.Joints[JointType.Head].Position.Y) < 0.2f)
				{
					zoomRightCaptured = true;
					zoomRightStart = activeBody.Joints[JointType.HandRight].Position.X;
				}
			}

			if (activeBody != null && zoomRightCaptured)
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

				if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > 0.2f)
				{
					zoomRightCaptured = false;
					activeBody = null;
					//Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Deactivated\n");
				}
				return returnValue;
			}
			return 1;
		}
	}
}
