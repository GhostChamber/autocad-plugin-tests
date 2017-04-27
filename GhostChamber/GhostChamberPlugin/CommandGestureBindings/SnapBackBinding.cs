using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    /**
     * Implements CommandGestureBinding for the SnapBack gesture.
     * SnapBack is used so that if while Pan, the object goes out of view, we can bring it back in view using the 'lasso' (fist with pointer finger outstretched upwards) gesture.
     */
    class SnapBackBinding : CommandGestureBinding
    {
        private SnapBackCommand command = new SnapBackCommand();    /**< The SnapBackCommand that the Gesture will use. */
        private SnapBackGesture gesture = new SnapBackGesture();    /**< The SnapBackOrbitGesture that the Command requires. */

        /** Checks if this gesture is active or not. Calls the IsActive Method on gesture.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
        {
            return gesture.IsActive(skeletons, bodyCount);
        }

        /** Calls the Update method of the Gesture and pass the return value to the bound command. Calls the Do method on command and then the Update.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public void Update(IList<Body> skeletons, int bodyCount)
        {
            command.Do();
            gesture.Update();
        }
    }
}
