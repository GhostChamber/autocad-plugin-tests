using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace GhostChamberPlugin.Gestures
{
    /**
     * ZoomGesture implements the Gesture interface. This gesture uses both the arms to zoom into or out of the image.
     */
    public sealed class ZoomGesture : Gesture
	{
		private Body activeBody = null;         /**< The body that we are currently reading the gestures of. */
        private double currentZoom = 1.0;       /**< The zoom value in this frame. */
		private float zoomScale = 1.0f;         /**< Scale used to change the speed at which you zoom in and out. Currently set to 1*/
        private double minHandDistance = 0.15;  /**< Minimum valid distance between hands */
        private double maxHandDistance = 0.85;  /**< Maximum valid distance between hands */
        private double zoomRightStart;          /**< The position of right hand at the start of calculation. */
		private double zoomRight;               /**< The position of right hand moved during the frame. */

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
                    if (GestureUtils.IsZoomGestureActive(body))
					{
						activeBody = body;
						currentZoom = 1.0;
                        zoomRightStart = activeBody.Joints[JointType.HandRight].Position.X;
                        zoomRight = zoomRightStart;
                        //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ZOOM\n");
						break;
					}
				}
			}
			return (activeBody != null);
		}

        /**
         * The Update method that runs every frame to return the calculated double based on gesture input.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
         * @return the calculated double for this frame with respect to the gesture movement read.
         */
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

                double zoomFraction = calculateZoomFactor(handDistance, zoomOut);

				double returnValue = (zoomFraction / currentZoom);
				currentZoom = zoomFraction;

				if (!GestureUtils.IsZoomGestureActive(activeBody))
				{
					activeBody = null;
					//Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DEACTIVATED\n");
				}
				return returnValue;
			}
			return 1;
		}

        /**
         * Helper method to calculate the zoomFactor.
         * @param handDistance distance between the hands.
         * @param zoomOut if user is zooming in or out.
         */
        private double calculateZoomFactor(double handDistance, bool zoomOut)
        {
            double zoomFraction = ((handDistance - minHandDistance) / (maxHandDistance - minHandDistance));
            if (zoomOut)
            {
                zoomFraction = (1 - zoomFraction * zoomScale);
            }
            else
            {
                zoomFraction = (1 + zoomFraction * zoomScale);
            }

            return zoomFraction;
        }
	}
}
