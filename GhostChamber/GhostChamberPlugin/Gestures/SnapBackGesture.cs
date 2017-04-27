using Microsoft.Kinect;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Gestures
{
    /**
     * Checks if the Lasso (pointer finger outstretched upwards.) gesture was performed by user and if so, brings the model back to centre of camera, if out of view.
     */
    class SnapBackGesture
    {
        private Body activeBody = null;             /**< The body that we are currently reading the gestures of. */

        /**
        * Implementation of Gesture.IsActive. Checks if gesture is active and if so, initializes the gesture.
        * @param skeletons is the list of Body objects found by the Kinect.
        * @param bodyCount is the number of bodies found in the skeletons list.
        * @return true if the gesture is active.
        */
        internal bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; i++)
                {
                    Body body = skeletons[i];
                    if (GestureUtils.IsSnapBackGestureActive(body))
                    {
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("SNAP_BACK Gesture detected.\n");
                        activeBody = body;
                        break;
                    }
                }
            }

            return (activeBody != null);
        }

        /**
         * Sets the activeBody to null after movement is performed.
         */
        public void Update()
        {
            activeBody = null;
        }
    }
}
