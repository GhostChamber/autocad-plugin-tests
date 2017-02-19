using Autodesk.AutoCAD.ApplicationServices;
using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace GhostChamberPlugin.Gestures
{
	public sealed class ZoomGesture : Gesture
	{
		private GestureType gestureType;
		private bool[] zooming;
		private float zoomLeft = 0.0f;
		private float zoomRight = 0.0f;
		private bool zoomRightCaptured = false;
		private const float ZOOM_SCALE = 10.0f;
		private double minHandDistance = 0.15;
		private double maxHandDistance = 0.85;
		private Microsoft.Kinect.Body activeBody = null;
		private double zoomRightStart;
		private Commands.Camera cam = new Commands.Camera(Application.DocumentManager.MdiActiveDocument);
		double currentZoom = 1.0;

		public void HandleZoomGesture(IList<Body> skeletons, int bodyCount)
		{

			if (gestureType == GestureType.NONE && skeletons != null)
			{
				if (zooming == null)
				{
					zooming = new bool[bodyCount];
				}

				for (int i = 0; i < bodyCount; i++)
				{
					Microsoft.Kinect.Body body = skeletons[i];

					if (body.Joints[JointType.Head].Position.Y != 0.0f &&
						(Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.Head].Position.Y) < 0.2f))
					{
						gestureType = GestureType.ZOOM;
						zoomLeft = body.Joints[JointType.HandLeft].Position.X;
						activeBody = body;
						zooming[i] = true;
						currentZoom = 1.0;
					}
					else
					{
						zooming[i] = false;
					}
				}
			}
			else if (gestureType == GestureType.ZOOM)
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
					cam.Zoom(zoomFraction / currentZoom);
					currentZoom = zoomFraction;

					if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > 0.2f)
					{
						gestureType = GestureType.NONE;
						zoomRightCaptured = false;
						activeBody = null;
					}
				}
			}
		}

		public bool IsActive()
		{
			throw new NotImplementedException();
		}
	}
}
