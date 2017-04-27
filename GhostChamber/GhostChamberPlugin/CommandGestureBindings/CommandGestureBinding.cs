using System.Collections.Generic;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    /**
     * @interface
     * Interface to bind the gesture and the corresponding command.
     */
    interface CommandGestureBinding
	{
		/** Checks if the current gesture is active or not
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
		bool IsGestureActive(IList<Body> skeletons, int bodyCount);

        /** Calls the Update method of the Gesture and pass the return value to the bound command.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        void Update(IList<Body> skeletons, int bodyCount);
	}
}
