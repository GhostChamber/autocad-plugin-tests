
using System.Collections.Generic;
using Microsoft.Kinect;

namespace GhostChamberPlugin.Gestures
{
    /**
     * @interface
     * The Gesture interface is used to define the Gesture classes.
     */
	interface Gesture
	{
        /**
         * Check to see if the Gesture is active.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
         * @return true if the gesture is active.
		 */
        bool IsActive(IList<Body> skeletons, int bodyCount);
	}
}
