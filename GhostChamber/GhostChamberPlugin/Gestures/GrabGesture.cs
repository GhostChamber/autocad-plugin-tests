using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Gestures
{
	class GrabGesture : Gesture
	{
		private Body activeBody = null;

		private CameraSpacePoint startPosition;
		private CameraSpacePoint previousPosition;
		private double PAN_COMMAND_THRESHOLD = 0.0005f;// Minimum distance for left hand to move to be registered as a movement.
		private float PAN_COMMAND_MULTIPLIER = -50.0f; // Multiplier to convert from real world distance to AutoCAD viewport distance.

		public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			if (activeBody == null && skeletons != null)
			{
				for (int i = 0; i < bodyCount; i++)
				{
					Body body = skeletons[i];
					if (GestureUtils.isCommandPositionActive(body, 0.2f) && (body?.HandLeftState == HandState.Closed))
					{
						activeBody = body;
						startPosition = activeBody.Joints[JointType.HandLeft].Position;
						previousPosition = startPosition;
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
				CameraSpacePoint currentPosition = activeBody.Joints[JointType.HandLeft].Position;

				// Get the change of position of the left hand from the previous frame and calculate distance.
				double dX = (currentPosition.X - previousPosition.X);
				double dY = (currentPosition.Y - previousPosition.Y);
				double distance = Math.Sqrt(dX * dX + dY * dY);

				if (distance > PAN_COMMAND_THRESHOLD)
				{
					previousPosition = currentPosition;
					movement = new Vector3d(dX * PAN_COMMAND_MULTIPLIER, dY * PAN_COMMAND_MULTIPLIER, 0);
				}

				if (activeBody.HandLeftState != HandState.Closed)
				{
					Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATE GRAB\n");
					activeBody = null;
				}
			}

			return movement;
		}
	}
}