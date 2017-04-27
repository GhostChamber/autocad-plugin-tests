using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;

namespace GhostChamberPlugin.Gestures
{
    /**
     * Gesture that facilitates 'grabbing' the model and panning about it with your left hand. Implements gesture.
     */
    class GrabGesture : Gesture
	{
		private Body activeBody = null;                     /**< The body that we are currently reading the gestures of. */
		private CameraSpacePoint startPosition;             /**< The position of camera when initializing the gesture. */
		private CameraSpacePoint previousPosition;          /**< The position of camera in the previous frame. */
        private CameraSpacePoint currentPosition;           /**< The position of camera in the current frame. */
        private double PAN_COMMAND_THRESHOLD = 0.0005f;     /**< Minimum distance for left hand to move to be registered as a movement. */
		private float PAN_COMMAND_MULTIPLIER = -50.0f;      /**< Multiplier to convert from real world distance to AutoCAD viewport distance. */

        /**
         * Implementation of Gesture.IsActive. Checks if gesture is active and if so, initializes the gesture.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
         * @return true if the gesture is active.
         */
        public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			if (activeBody == null && skeletons != null)
			{
				for (int i = 0; i < bodyCount; i++)
				{
					Body body = skeletons[i];
					if (GestureUtils.IsGrabGestureActive(body))
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

        /**
         * The Update method that runs every frame to return the calculated Vector2d based on gesture input.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
         * @return the calculated Vector2d for this frame with respect to the gesture movement read.
         */
        public Vector2d Update(IList<Body> skeletons, int bodyCount)
		{
			Vector2d movement = new Vector2d(0.0, 0.0);

			if (activeBody != null)
			{
				currentPosition = activeBody.Joints[JointType.HandLeft].Position;

				// Get the change of position of the left hand from the previous frame and calculate distance.
				double dX = (currentPosition.X - previousPosition.X);
				double dY = (currentPosition.Y - previousPosition.Y);

				double distance = Math.Sqrt((dX * dX) + (dY * dY));

				if (distance > PAN_COMMAND_THRESHOLD)
				{
					previousPosition = currentPosition;
					movement = new Vector2d(dX * PAN_COMMAND_MULTIPLIER, dY * PAN_COMMAND_MULTIPLIER);
				}

				if (activeBody.HandLeftState != HandState.Closed)
				{
					activeBody = null;
				}
			}

			return movement;
		}

	}
}