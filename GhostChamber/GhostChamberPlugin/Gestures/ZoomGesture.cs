using Autodesk.AutoCAD.ApplicationServices;
using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace GhostChamberPlugin.Gestures
{
	public sealed class ZoomGesture : Gesture
	{
		private float zoomRight = 0.0f;
		private double minHandDistance = 0.15;
		private double maxHandDistance = 0.85;
		private double zoomRightStart;
		private double currentZoom = 1.0;
		private bool zoomRightCaptured = false;
		private Microsoft.Kinect.Body activeBody = null;

		public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			// TODO: activeBody is being set to null at the end of Update method.
			// But for some reason we still had to loop through all the bodies every frame to detect end of gesture
			// Find a way to avoid this
			if (skeletons != null)
			{
				bool activeBodyFound = false;
				for (int i = 0; i < bodyCount; i++)
				{
					Microsoft.Kinect.Body body = skeletons[i];
					if (body.Joints[JointType.Head].Position.Y != 0.0f &&
						(Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < 0.2f))
					{
						activeBody = body;
						currentZoom = 1.0;
						Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Activated\n");
						activeBodyFound = true;
						break;
					}
				}
				if (!activeBodyFound)
				{
					activeBody = null;
					zoomRightCaptured = false;
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
					zoomFraction += 1;
				}
				else
				{
					zoomFraction = 1 - zoomFraction;
				}
				double returnValue = (zoomFraction / currentZoom);
				currentZoom = zoomFraction;

				if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > 0.2f)
				{
					zoomRightCaptured = false;
					activeBody = null;
					Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Deactivated\n");
				}
				return returnValue;
			}
			return 1;
		}
	}
}
