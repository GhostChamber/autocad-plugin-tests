using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;


namespace GhostChamberPlugin.Gestures
{
    /**
     * Checks if user used the Grab gesture with their right hand, and if so, performs the orbit based on the gesture.
     */
    public sealed class OrbitGesture : Gesture
    {
        private Body activeBody = null;                     /**< The body that we are currently reading the gestures of. */
        private CameraSpacePoint toolStartPosition;         /**< The position of camera at the start of the gesture. */
        private CameraSpacePoint toolPreviousPosition;      /**< The position of the camera in the previous frame. */
        private CameraSpacePoint toolPosition;              /**< The position of camera in the current frame. */

        const double ROTATION_COMMAND_THRESHOLD = 0.005f;   /**< Const double value used to check if the movement is at least this much to account for jitter. */

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
                for (int i = 0; i < bodyCount; ++i)
                {
                    Body body = skeletons[i];

                    if (GestureUtils.IsOrbitGestureActive(body))
                    {
                        activeBody = body;
                        //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("ORBIT\n");

                        // Record right hand location
                        toolStartPosition = activeBody.Joints[JointType.HandRight].Position;
                        toolPreviousPosition = toolStartPosition;
                        break;
                    }
                }
            }
            return (activeBody != null);
        }

        /**
         * The Update method that runs every frame to return the calculated Vector3d based on gesture input.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
         * @return the calculated Vector3d for this frame with respect to the gesture movement read.
         */
        public Vector3d Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = new Vector3d(0.0, 0.0, 0.0);

            if (activeBody != null)
            {
                toolPosition = activeBody.Joints[JointType.HandRight].Position;

                double dX = (toolPosition.X - toolPreviousPosition.X);
                double dY = (toolPosition.Y - toolPreviousPosition.Y);
                double dZ = (toolPosition.Z - toolPreviousPosition.Z);

                double distance = Math.Sqrt(dX*dX + dY*dY + dZ*dZ);

                if (distance > ROTATION_COMMAND_THRESHOLD)
                {
                    toolPreviousPosition = toolPosition;
                    movement = new Vector3d(dX, dY, dZ);
                }

                if (activeBody.HandRightState != HandState.Closed)
                {
                    activeBody = null;
                }
            }

            return movement;
        }
    }
}
